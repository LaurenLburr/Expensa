using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace CodexExpensa.App.WinForms.Navigation;

internal sealed class NavigationNode
{
    public NavigationNode(string id, string text)
    {
        if (string.IsNullOrWhiteSpace(id)) throw new ArgumentException("Navigation node id is required.", nameof(id));
        if (string.IsNullOrWhiteSpace(text)) throw new ArgumentException("Navigation node text is required.", nameof(text));

        Id = id;
        Text = text;
    }

    public string Id { get; }
    public string Text { get; }

    // If null, node is just a category/header node
    public Func<Form>? CreateScreen { get; private set; }

    // If true, reuse the same Form instance while app is running
    public bool SingleInstance { get; private set; } = true;

    public List<NavigationNode> Children { get; } = new();

    public NavigationNode WithScreen(Func<Form> createScreen, bool singleInstance = true)
    {
        CreateScreen = createScreen ?? throw new ArgumentNullException(nameof(createScreen));
        SingleInstance = singleInstance;
        return this;
    }

    public NavigationNode AddChild(NavigationNode child)
    {
        if (child is null) throw new ArgumentNullException(nameof(child));
        Children.Add(child);
        return this;
    }
}