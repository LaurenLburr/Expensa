using System.Windows.Forms;

namespace CodexExpensa.ExtensionDevHost.Commands.Abstractions;

public static class CommandContextNodeExtensions
{
    public static TreeNode? GetSelectedNode(this ICommandContext context)
    {
        if (context == null)
            throw new ArgumentNullException(nameof(context));

        return context.SelectedNode as TreeNode;
    }
}
