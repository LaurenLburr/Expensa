using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace CodexExpensa.App.WinForms.Navigation;

internal sealed class ScreenHost : IDisposable
{
    private readonly Panel _hostPanel;
    private readonly Dictionary<string, Form> _singletons = new(StringComparer.OrdinalIgnoreCase);

    public ScreenHost(Panel hostPanel)
    {
        _hostPanel = hostPanel ?? throw new ArgumentNullException(nameof(hostPanel));
    }

    public void Show(string nodeId, Func<Form> createScreen, bool singleInstance)
    {
        if (string.IsNullOrWhiteSpace(nodeId)) throw new ArgumentException("nodeId is required.", nameof(nodeId));
        if (createScreen is null) throw new ArgumentNullException(nameof(createScreen));

        Form form;

        if (singleInstance && _singletons.TryGetValue(nodeId, out var existing) && !existing.IsDisposed)
        {
            form = existing;
        }
        else
        {
            form = createScreen();

            if (singleInstance)
            {
                _singletons[nodeId] = form;
            }
        }

        DockIntoHost(form);
    }

    private void DockIntoHost(Form form)
    {
        // If already hosted, just bring to front.
        if (form.TopLevel == false && form.Parent == _hostPanel)
        {
            form.BringToFront();
            form.Focus();
            return;
        }

        // Clear existing screen controls
        _hostPanel.SuspendLayout();
        try
        {
            _hostPanel.Controls.Clear();

            form.TopLevel = false;
            form.FormBorderStyle = FormBorderStyle.None;
            form.Dock = DockStyle.Fill;

            _hostPanel.Controls.Add(form);
            form.Show();
            form.BringToFront();
            form.Focus();
        }
        finally
        {
            _hostPanel.ResumeLayout();
        }
    }

    public void Dispose()
    {
        foreach (var kvp in _singletons)
        {
            try
            {
                if (!kvp.Value.IsDisposed)
                    kvp.Value.Dispose();
            }
            catch
            {
                // Intentionally swallow: we're shutting down
            }
        }

        _singletons.Clear();
    }
}