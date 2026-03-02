using System;
using System.ComponentModel;
using System.Runtime.InteropServices;
using System.Text;
using CodexExpensa.Core.Abstractions;

namespace CodexExpensa.App.WinForms.Infrastructure;

/// <summary>
/// Windows Credential Manager implementation of ICredentialStore.
/// Stores credentials as Generic credentials.
/// </summary>
public sealed class WindowsCredentialStore : ICredentialStore
{
    private const uint CRED_TYPE_GENERIC = 1;

    // Persistence:
    // - CRED_PERSIST_LOCAL_MACHINE: accessible to the user on this machine
    // - CRED_PERSIST_ENTERPRISE: roaming (domain/enterprise)
    // - CRED_PERSIST_SESSION: session only
    private const uint CRED_PERSIST_LOCAL_MACHINE = 2;

    public bool TryGet(string key, out string? username, out string? password)
    {
        if (string.IsNullOrWhiteSpace(key))
            throw new ArgumentException("key is required.", nameof(key));

        username = null;
        password = null;

        if (!CredRead(key, CRED_TYPE_GENERIC, 0, out var pcred))
            return false;

        try
        {
            var cred = Marshal.PtrToStructure<CREDENTIAL>(pcred);

            username = string.IsNullOrWhiteSpace(cred.UserName) ? null : cred.UserName;

            if (cred.CredentialBlobSize == 0 || cred.CredentialBlob == IntPtr.Zero)
            {
                password = null;
                return true;
            }

            // Stored as UTF-16 bytes (Unicode).
            var blob = new byte[cred.CredentialBlobSize];
            Marshal.Copy(cred.CredentialBlob, blob, 0, blob.Length);

            password = Encoding.Unicode.GetString(blob).TrimEnd('\0');
            return true;
        }
        finally
        {
            CredFree(pcred);
        }
    }

    public void Save(string key, string username, string password)
    {
        if (string.IsNullOrWhiteSpace(key))
            throw new ArgumentException("key is required.", nameof(key));
        if (string.IsNullOrWhiteSpace(username))
            throw new ArgumentException("username is required.", nameof(username));
        if (password is null)
            throw new ArgumentNullException(nameof(password));

        // CredWrite expects the secret as a byte[] blob.
        // We store as UTF-16 (Encoding.Unicode).
        var pwBytes = Encoding.Unicode.GetBytes(password);

        var cred = new CREDENTIAL
        {
            Type = CRED_TYPE_GENERIC,
            TargetName = key,
            UserName = username,
            Persist = CRED_PERSIST_LOCAL_MACHINE,
            CredentialBlobSize = (uint)pwBytes.Length,
            AttributeCount = 0,
            Attributes = IntPtr.Zero,
            Comment = null,
            TargetAlias = null
        };

        cred.CredentialBlob = Marshal.AllocHGlobal(pwBytes.Length);

        try
        {
            Marshal.Copy(pwBytes, 0, cred.CredentialBlob, pwBytes.Length);

            if (!CredWrite(ref cred, 0))
                throw new Win32Exception(Marshal.GetLastWin32Error(), "CredWrite failed.");
        }
        finally
        {
            Marshal.FreeHGlobal(cred.CredentialBlob);
        }
    }

    public void Delete(string key)
    {
        if (string.IsNullOrWhiteSpace(key))
            throw new ArgumentException("key is required.", nameof(key));

        // If it doesn't exist, treat as no-op.
        CredDelete(key, CRED_TYPE_GENERIC, 0);
    }

    [DllImport("advapi32.dll", SetLastError = true, CharSet = CharSet.Unicode)]
    private static extern bool CredRead(string target, uint type, uint reservedFlag, out IntPtr credentialPtr);

    [DllImport("advapi32.dll", SetLastError = true, CharSet = CharSet.Unicode)]
    private static extern bool CredWrite([In] ref CREDENTIAL userCredential, [In] uint flags);

    [DllImport("advapi32.dll", SetLastError = true, CharSet = CharSet.Unicode)]
    private static extern bool CredDelete(string target, uint type, uint flags);

    [DllImport("advapi32.dll", SetLastError = true)]
    private static extern void CredFree([In] IntPtr buffer);

    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
    private struct CREDENTIAL
    {
        public uint Flags;
        public uint Type;

        public string TargetName;
        public string? Comment;

        public System.Runtime.InteropServices.ComTypes.FILETIME LastWritten;

        public uint CredentialBlobSize;
        public IntPtr CredentialBlob;

        public uint Persist;

        public uint AttributeCount;
        public IntPtr Attributes;

        public string? TargetAlias;
        public string? UserName;
    }
}