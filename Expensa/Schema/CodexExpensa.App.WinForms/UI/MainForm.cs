// FULL FILE: CodexExpensa.App.WinForms/UI/MainForm.cs
// Websites tree now loads real nodes

using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Windows.Forms;
using CodexExpensa.App.WinForms.Infrastructure;
using CodexExpensa.App.WinForms.UI.Websites;
using CodexExpensa.Core.Abstractions;

namespace CodexExpensa.App.WinForms.UI;

public partial class MainForm : Form
{
    private const string RootWebsites = "root:websites";
    private const string NodeAllWebsites = "websites:all";

    private readonly IDatabaseSession _dbSession;
    private readonly ScreenHost _screenHost;

    public MainForm(IDatabaseSession dbSession)
    {
        _dbSession = dbSession;
        InitializeComponent();
        _screenHost = new ScreenHost(panelHost);

        BuildTree();
    }

    private void BuildTree()
    {
        treeNav.Nodes.Clear();
        treeNav.Nodes.Add(BuildWebsitesRootNode());
    }

    private IReadOnlyList<(string Id, string Name, string Url)> LoadWebsiteRows()
    {
        DataTable table = _dbSession.QueryDataTable("Website.GetAllActive");

        var rows = new List<(string, string, string)>();

        foreach (DataRow row in table.Rows)
        {
            string id = Convert.ToString(row["WebsiteId"]) ?? "";
            string name = Convert.ToString(row["Name"]) ?? "";
            string url = Convert.ToString(row["Url"]) ?? "";

            if (!string.IsNullOrWhiteSpace(id))
                rows.Add((id, name, url));
        }

        return rows;
    }

    private TreeNode BuildWebsitesRootNode()
    {
        TreeNode root = new TreeNode("Websites") { Tag = RootWebsites };

        root.Nodes.Add(new TreeNode("All Websites") { Tag = NodeAllWebsites });

        var websites = LoadWebsiteRows();

        foreach (var (id, name, _) in websites)
        {
            string text = string.IsNullOrWhiteSpace(name) ? id : name;
            root.Nodes.Add(new TreeNode(text) { Tag = $"website:{id}" });
        }

        return root;
    }

    private void treeNav_AfterSelect(object sender, TreeViewEventArgs e)
    {
        if (e.Node?.Tag is not string id) return;

        if (id == NodeAllWebsites)
        {
            ShowWebsitesLanding();
            return;
        }

        if (id.StartsWith("website:"))
        {
            string websiteId = id.Substring("website:".Length);
            ShowWebsiteDetails(websiteId);
        }
    }

    private void ShowWebsitesLanding()
    {
        var rows = LoadWebsiteRows();

        _screenHost.Show("websites:list", () =>
        {
            var form = new WebsitesLandingForm(id => ShowWebsiteDetails(id));
            form.SetRows(rows);
            return form;
        }, false);
    }

    private void ShowWebsiteDetails(string id)
    {
        _screenHost.Show($"website:{id}", () =>
        {
            var form = new WebsiteDetailsForm(() => BuildTree());
            form.LoadWebsite(id, "", "", "");
            return form;
        }, false);
    }
}
