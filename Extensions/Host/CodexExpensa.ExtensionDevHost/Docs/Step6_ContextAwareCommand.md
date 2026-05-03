# Step 6 – Context Aware Commands

## New helper

Use:

var node = context.GetSelectedNode();

## Example

if (node == null)
{
    MessageBox.Show("No node selected");
    return;
}

MessageBox.Show(node.Text);

## Goal

Commands now react to selected node instead of being static.