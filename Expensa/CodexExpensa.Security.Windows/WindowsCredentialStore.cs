using System.ComponentModel;
using System.Runtime.InteropServices;
using System.Text;
using CodexExpensa.Core.Abstractions;

namespace CodexExpensa.Security.Windows;

public sealed class WindowsCredentialStore : ICredentialStore
{
    private const uint CredTypeGeneric = 1;
    private const uint CredPersistLocalMachine = 2;

    public bool TryGet(string key, out string? username, out string? password)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(key);

        username = null;
        password = null;

        if (!CredRead(key, CredTypeGeneric, 0, out IntPtr credentialPointer))
        {
            return false;
        }

        try
        {
            Credential credential =
                Marshal.PtrToStructure<Credential>(credentialPointer);

            username = string.IsNullOrWhiteSpace(credential.UserName)
                ? null
                : credential.UserName;

            if (credential.CredentialBlobSize == 0 ||
                credential.CredentialBlob == IntPtr.Zero)
            {
                return true;
            }

            byte[] blob = new byte[credential.CredentialBlobSize];
            Marshal.Copy(credential.CredentialBlob, blob, 0, blob.Length);

            password = Encoding.Unicode.GetString(blob).TrimEnd('\0');
            return true;
        }
        finally
        {
            CredFree(credentialPointer);
        }
    }

    public void Save(string key, string username, string password)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(key);
        ArgumentException.ThrowIfNullOrWhiteSpace(username);
        ArgumentNullException.ThrowIfNull(password);

        byte[] passwordBytes = Encoding.Unicode.GetBytes(password);

        Credential credential = new()
        {
            Type = CredTypeGeneric,
            TargetName = key,
            UserName = username,
            Persist = CredPersistLocalMachine,
            CredentialBlobSize = (uint)passwordBytes.Length,
            AttributeCount = 0,
            Attributes = IntPtr.Zero,
            Comment = null,
            TargetAlias = null
        };

        credential.CredentialBlob =
            Marshal.AllocHGlobal(passwordBytes.Length);

        try
        {
            Marshal.Copy(
                passwordBytes,
                0,
                credential.CredentialBlob,
                passwordBytes.Length);

            if (!CredWrite(ref credential, 0))
            {
                throw new Win32Exception(
                    Marshal.GetLastWin32Error(),
                    "CredWrite failed.");
            }
        }
        finally
        {
            Marshal.FreeHGlobal(credential.CredentialBlob);
        }
    }

    public void Delete(string key)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(key);

        CredDelete(key, CredTypeGeneric, 0);
    }

    [DllImport("advapi32.dll", SetLastError = true, CharSet = CharSet.Unicode)]
    private static extern bool CredRead(
        string target,
        uint type,
        uint reservedFlag,
        out IntPtr credentialPointer);

    [DllImport("advapi32.dll", SetLastError = true, CharSet = CharSet.Unicode)]
    private static extern bool CredWrite(
        [In] ref Credential userCredential,
        [In] uint flags);

    [DllImport("advapi32.dll", SetLastError = true, CharSet = CharSet.Unicode)]
    private static extern bool CredDelete(
        string target,
        uint type,
        uint flags);

    [DllImport("advapi32.dll", SetLastError = true)]
    private static extern void CredFree([In] IntPtr buffer);

    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
    private struct Credential
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
