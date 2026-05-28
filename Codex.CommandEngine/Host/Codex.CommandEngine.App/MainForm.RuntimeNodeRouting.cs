using System.Windows.Forms;

namespace Codex.CommandEngine.App;

public static class MainFormRuntimeNodeRouting
{
    public static bool TryHandleRuntimeOperationsNode(
        string nodeKey,
        RichTextBox outputTextBox,
        RuntimeOperationsHostViewBuilder runtimeOperationsHostViewBuilder)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(nodeKey);
        ArgumentNullException.ThrowIfNull(outputTextBox);
        ArgumentNullException.ThrowIfNull(runtimeOperationsHostViewBuilder);

        if (!MainFormRuntimeOperations.IsRuntimeOperationsNode(nodeKey))
        {
            return false;
        }

        string text =
            runtimeOperationsHostViewBuilder.BuildTextForNode(nodeKey);

        outputTextBox.Clear();
        outputTextBox.Text = text;
        outputTextBox.SelectionStart = 0;
        outputTextBox.SelectionLength = 0;

        return true;
    }
}
