// (same as previous final version - shortened for packaging demo)
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using CodexExpensa.ExtensionDevHost.Commands;

namespace CodexExpensa.ExtensionDevHost.UI;

public sealed class CommandCatalogForm : Form
{
    private readonly CommandRegistry _registry;
    private readonly string _configPath;
    private readonly Action _rebuildMenu;
    private readonly DataGridView _grid;
    private readonly TextBox _txtJson;
    private int _dragRowIndex = -1;

    public CommandCatalogForm(CommandRegistry registry, string configPath, Action rebuildMenu)
    {
        _registry = registry;
        _configPath = configPath;
        _rebuildMenu = rebuildMenu;

        Width = 1100;
        Height = 700;

        var tabs = new TabControl { Dock = DockStyle.Fill };

        var gridTab = new TabPage("Commands");
        var jsonTab = new TabPage("JSON");

        _grid = new DataGridView
        {
            Dock = DockStyle.Fill,
            AutoGenerateColumns = true,
            AllowUserToAddRows = false,
            AllowDrop = true
        };

        _grid.MouseDown += (s,e)=> _dragRowIndex = _grid.HitTest(e.X,e.Y).RowIndex;
        _grid.MouseMove += (s,e)=> { if(e.Button==MouseButtons.Left && _dragRowIndex>=0) _grid.DoDragDrop(_grid.Rows[_dragRowIndex], DragDropEffects.Move); };
        _grid.DragOver += (s,e)=> e.Effect = DragDropEffects.Move;
        _grid.DragDrop += Grid_DragDrop;

        var openBtn = new Button { Text="Open JSON Folder" };
        openBtn.Click += (_,_) => Process.Start(new ProcessStartInfo{
            FileName = Path.GetDirectoryName(_configPath)!,
            UseShellExecute = true
        });

        var panel = new FlowLayoutPanel{Dock=DockStyle.Top,AutoSize=true};
        panel.Controls.Add(openBtn);

        gridTab.Controls.Add(_grid);
        gridTab.Controls.Add(panel);

        _txtJson = new TextBox{Dock=DockStyle.Fill,Multiline=true,ScrollBars=ScrollBars.Both};

        var reload = new Button{Text="Reload JSON"};
        var save = new Button{Text="Save JSON"};

        reload.Click += (_,_) => LoadJson();
        save.Click += (_,_) => { File.WriteAllText(_configPath,_txtJson.Text); _registry.LoadConfig(); _rebuildMenu(); };

        var jsonPanel = new FlowLayoutPanel{Dock=DockStyle.Top,AutoSize=true};
        jsonPanel.Controls.Add(reload);
        jsonPanel.Controls.Add(save);

        jsonTab.Controls.Add(_txtJson);
        jsonTab.Controls.Add(jsonPanel);

        tabs.TabPages.Add(gridTab);
        tabs.TabPages.Add(jsonTab);

        Controls.Add(tabs);

        LoadGrid();
        LoadJson();
    }

    private void LoadGrid()
    {
        _grid.DataSource = _registry.GetCommands()
            .Select(x => new Row{
                CommandKey=x.CommandKey,
                MenuText=x.MenuText,
                MenuOrder=x.MenuOrder,
                ItemOrder=x.ItemOrder,
                TopLevelMenu=x.TopLevelMenu
            })
            .OrderBy(x=>x.MenuOrder).ThenBy(x=>x.ItemOrder).ToList();
    }

    private void Grid_DragDrop(object? s, DragEventArgs e)
    {
        var rows = (List<Row>)_grid.DataSource!;
        var pt = _grid.PointToClient(new System.Drawing.Point(e.X,e.Y));
        var idx = _grid.HitTest(pt.X,pt.Y).RowIndex;

        if(idx<0 || _dragRowIndex<0) return;

        var item = rows[_dragRowIndex];
        rows.RemoveAt(_dragRowIndex);
        rows.Insert(idx,item);

        int order=100;
        foreach(var r in rows){
            r.MenuOrder=order;
            r.ItemOrder=order;
            order+=100;
            _registry.UpdateMetadata(r.CommandKey,r.MenuText,r.MenuOrder,r.ItemOrder);
        }

        _grid.DataSource=null;
        _grid.DataSource=rows;

        _rebuildMenu();
    }

    private void LoadJson()
    {
        _txtJson.Text = File.Exists(_configPath) ? File.ReadAllText(_configPath) : "{ }";
    }

    private class Row
    {
        public string CommandKey { get; set; } = "";
        public string MenuText { get; set; } = "";
        public int MenuOrder {get;set;}
        public int ItemOrder {get;set;}
        public string TopLevelMenu { get; set; } = "";
    }
}
