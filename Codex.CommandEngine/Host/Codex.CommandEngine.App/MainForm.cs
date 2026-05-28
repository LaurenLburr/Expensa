using Codex.CommandEngine.Data;
using Microsoft.Data.Sqlite;

namespace Codex.CommandEngine.App;

public sealed class MainForm : Form
{
    private const string DbSchemaNodeKey = "db-schema";
    private const string FullDbSchemaNodeKey = "full-db-schema";
    private const string RegisteredCommandsNodeKey = "registered-commands";
    private const string CommandDefinitionsNodeKey = "command-definitions";
    private const string CommandParametersNodeKey = "command-parameters";
    private const string WorkflowDefinitionsNodeKey = "workflow-definitions";
    private const string WorkflowStepsNodeKey = "workflow-steps";
    private const string ExecutionRunsNodeKey = "execution-runs";
    private const string ExecutionStepsNodeKey = "execution-steps";

    private readonly TreeView _tree = new();
    private readonly Panel _workspace = new();
    private readonly SplitContainer _split = new();
    private readonly string _databasePath;

    public MainForm()
    {
        Text = "Codex Command Engine";
        Width = 1400;
        Height = 900;
        StartPosition = FormStartPosition.CenterScreen;

        _databasePath = CommandEngineDatabasePaths.DefaultDatabasePath;

        ConfigureLayout();
        BuildTree();
        InitializeDatabase();
    }

    private void ConfigureLayout()
    {
        _split.Dock = DockStyle.Fill;
        _split.SplitterDistance = 320;

        _tree.Dock = DockStyle.Fill;
        _tree.AfterSelect += Tree_AfterSelect;

        _workspace.Dock = DockStyle.Fill;

        _split.Panel1.Controls.Add(_tree);
        _split.Panel2.Controls.Add(_workspace);

        Controls.Add(_split);
    }

    private void BuildTree()
    {
        TreeNode root = new("Command Engine");

        root.Nodes.Add(new TreeNode("Projects"));
        TreeNode commandsNode = new("Commands");
        commandsNode.Nodes.Add(new TreeNode("Registered Commands")
        {
            Tag = RegisteredCommandsNodeKey
        });
        commandsNode.Nodes.Add(new TreeNode("Command Definitions")
        {
            Tag = CommandDefinitionsNodeKey
        });
        commandsNode.Nodes.Add(new TreeNode("Command Parameters")
        {
            Tag = CommandParametersNodeKey
        });
        root.Nodes.Add(commandsNode);
        TreeNode workflowsNode = new("Workflows");
        workflowsNode.Nodes.Add(new TreeNode("Workflow Definitions")
        {
            Tag = WorkflowDefinitionsNodeKey
        });
        workflowsNode.Nodes.Add(new TreeNode("Workflow Steps")
        {
            Tag = WorkflowStepsNodeKey
        });
        root.Nodes.Add(workflowsNode);
        root.Nodes.Add(new TreeNode("Contexts"));
        root.Nodes.Add(new TreeNode("Connections"));
        root.Nodes.Add(new TreeNode("AI Providers"));
        TreeNode executionHistoryNode = new("Execution History");
        executionHistoryNode.Nodes.Add(new TreeNode("Execution Runs")
        {
            Tag = ExecutionRunsNodeKey
        });
        executionHistoryNode.Nodes.Add(new TreeNode("Execution Steps")
        {
            Tag = ExecutionStepsNodeKey
        });
        root.Nodes.Add(executionHistoryNode);

        TreeNode databaseNode = new("Database");
        databaseNode.Nodes.Add(new TreeNode("DB Schema")
        {
            Tag = DbSchemaNodeKey
        });

        databaseNode.Nodes.Add(new TreeNode("Full DB Schema")
        {
            Tag = FullDbSchemaNodeKey
        });

        databaseNode.Nodes.Add(new TreeNode("SQL Catalog"));
        root.Nodes.Add(databaseNode);

        root.Nodes.Add(new TreeNode("Settings"));

        _tree.Nodes.Add(root);
        root.Expand();
        commandsNode.Expand();
        workflowsNode.Expand();
        executionHistoryNode.Expand();
        databaseNode.Expand();
    }

    private void InitializeDatabase()
    {
        CommandEngineDatabaseOptions options = CommandEngineDatabaseOptions.ForFile(_databasePath);
        CommandEngineConnectionFactory connectionFactory = new(options);
        CommandEngineDatabaseInitializer initializer = new(connectionFactory);

        initializer.EnsureCreated();
    }

    private void Tree_AfterSelect(object? sender, TreeViewEventArgs e)
    {
        if (e.Node is null)
        {
            return;
        }

        _workspace.Controls.Clear();

        if (string.Equals(e.Node.Tag as string, DbSchemaNodeKey, StringComparison.Ordinal))
        {
            ShowDatabaseSchemaView();
            return;
        }

        if (string.Equals(e.Node.Tag as string, FullDbSchemaNodeKey, StringComparison.Ordinal))
        {
            ShowFullDatabaseSchemaView();
            return;
        }

        if (string.Equals(e.Node.Tag as string, RegisteredCommandsNodeKey, StringComparison.Ordinal))
        {
            ShowRegisteredCommandsView();
            return;
        }

        if (string.Equals(e.Node.Tag as string, CommandDefinitionsNodeKey, StringComparison.Ordinal))
        {
            ShowCommandDefinitionsView();
            return;
        }

        if (string.Equals(e.Node.Tag as string, CommandParametersNodeKey, StringComparison.Ordinal))
        {
            ShowCommandParametersView();
            return;
        }

        if (string.Equals(e.Node.Tag as string, WorkflowDefinitionsNodeKey, StringComparison.Ordinal))
        {
            ShowWorkflowDefinitionsView();
            return;
        }

        if (string.Equals(e.Node.Tag as string, WorkflowStepsNodeKey, StringComparison.Ordinal))
        {
            ShowWorkflowStepsView();
            return;
        }

        if (string.Equals(e.Node.Tag as string, ExecutionRunsNodeKey, StringComparison.Ordinal))
        {
            ShowExecutionRunsView();
            return;
        }

        if (string.Equals(e.Node.Tag as string, ExecutionStepsNodeKey, StringComparison.Ordinal))
        {
            ShowExecutionStepsView();
            return;
        }

        ShowDefaultSelectionView(e.Node.Text);
    }

    private void ShowDefaultSelectionView(string selectedText)
    {
        Label label = new()
        {
            Dock = DockStyle.Fill,
            TextAlign = ContentAlignment.MiddleCenter,
            Font = new Font("Segoe UI", 18F, FontStyle.Bold),
            Text = $"Selected: {selectedText}{Environment.NewLine}{Environment.NewLine}Engine DB:{Environment.NewLine}{_databasePath}"
        };

        _workspace.Controls.Add(label);
    }

    private void ShowDatabaseSchemaView()
    {
        TableLayoutPanel layout = new()
        {
            Dock = DockStyle.Fill,
            ColumnCount = 1,
            RowCount = 3,
            Padding = new Padding(16)
        };

        layout.RowStyles.Add(new RowStyle(SizeType.AutoSize));
        layout.RowStyles.Add(new RowStyle(SizeType.AutoSize));
        layout.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));

        Label title = new()
        {
            AutoSize = true,
            Dock = DockStyle.Top,
            Font = new Font("Segoe UI", 18F, FontStyle.Bold),
            Text = "Database Schema"
        };

        Label details = new()
        {
            AutoSize = true,
            Dock = DockStyle.Top,
            Font = new Font("Segoe UI", 10F, FontStyle.Regular),
            Padding = new Padding(0, 8, 0, 12),
            Text = $"Engine DB: {_databasePath}{Environment.NewLine}Schema Version: {CommandEngineSchema.CurrentSchemaVersion}"
        };

        ListView tableList = new()
        {
            Dock = DockStyle.Fill,
            FullRowSelect = true,
            GridLines = true,
            View = View.Details
        };

        tableList.Columns.Add("Object", 260);
        tableList.Columns.Add("Type", 120);
        tableList.Columns.Add("SQL", 900);

        foreach (DatabaseSchemaObject schemaObject in LoadDatabaseSchemaObjects())
        {
            ListViewItem item = new(schemaObject.Name);
            item.SubItems.Add(schemaObject.Type);
            item.SubItems.Add(schemaObject.Sql);
            tableList.Items.Add(item);
        }

        layout.Controls.Add(title, 0, 0);
        layout.Controls.Add(details, 0, 1);
        layout.Controls.Add(tableList, 0, 2);

        _workspace.Controls.Add(layout);
    }



    private void ShowRegisteredCommandsView()
    {
        TextBox textBox = CreateReadOnlyTextBox();
        textBox.Text = "Registered runtime commands will appear here once command handlers are wired into the host.\r\n\r\nThis node is intentionally separate from persisted command definitions because the database stores metadata, not executable code.";
        _workspace.Controls.Add(textBox);
    }

    private void ShowCommandDefinitionsView()
    {
        CommandDefinitionRepository repository = CreateCommandDefinitionRepository();
        IReadOnlyList<CommandDefinitionRecord> definitions = repository.ListAll();

        ListView listView = new()
        {
            Dock = DockStyle.Fill,
            FullRowSelect = true,
            GridLines = true,
            View = View.Details
        };

        listView.Columns.Add("Command", 240);
        listView.Columns.Add("Display Name", 240);
        listView.Columns.Add("Category", 160);
        listView.Columns.Add("Version", 80);
        listView.Columns.Add("Enabled", 80);
        listView.Columns.Add("Handler Type", 320);

        foreach (CommandDefinitionRecord definition in definitions)
        {
            ListViewItem item = new(definition.CommandName);
            item.SubItems.Add(definition.DisplayName);
            item.SubItems.Add(definition.Category);
            item.SubItems.Add(definition.Version.ToString(System.Globalization.CultureInfo.InvariantCulture));
            item.SubItems.Add(definition.IsEnabled ? "Yes" : "No");
            item.SubItems.Add(definition.HandlerType);
            listView.Items.Add(item);
        }

        _workspace.Controls.Add(listView);
    }

    private void ShowCommandParametersView()
    {
        CommandDefinitionRepository repository = CreateCommandDefinitionRepository();
        IReadOnlyList<CommandDefinitionRecord> definitions = repository.ListAll();

        TextBox textBox = CreateReadOnlyTextBox();
        System.Text.StringBuilder builder = new();

        foreach (CommandDefinitionRecord definition in definitions)
        {
            builder.AppendLine($"Command: {definition.CommandName}");
            IReadOnlyList<CommandParameterDefinitionRecord> parameters = repository.ListParameters(definition.CommandDefinitionId);

            if (parameters.Count == 0)
            {
                builder.AppendLine("  No parameters defined.");
            }

            foreach (CommandParameterDefinitionRecord parameter in parameters)
            {
                string required = parameter.IsRequired ? "required" : "optional";
                builder.AppendLine($"  - {parameter.ParameterName} ({parameter.ParameterType}, {required}) Sort: {parameter.SortOrder}");
                builder.AppendLine($"    {parameter.Description}");
            }

            builder.AppendLine();
        }

        if (builder.Length == 0)
        {
            builder.AppendLine("No command definitions are currently stored in the engine database.");
        }

        textBox.Text = builder.ToString();
        _workspace.Controls.Add(textBox);
    }


    private void ShowWorkflowDefinitionsView()
    {
        WorkflowDefinitionRepository repository = CreateWorkflowDefinitionRepository();
        IReadOnlyList<WorkflowDefinitionRecord> definitions = repository.ListAll();

        ListView listView = new()
        {
            Dock = DockStyle.Fill,
            FullRowSelect = true,
            GridLines = true,
            View = View.Details
        };

        listView.Columns.Add("Workflow", 240);
        listView.Columns.Add("Display Name", 240);
        listView.Columns.Add("Version", 80);
        listView.Columns.Add("Enabled", 80);
        listView.Columns.Add("Description", 600);

        foreach (WorkflowDefinitionRecord definition in definitions)
        {
            ListViewItem item = new(definition.WorkflowName);
            item.SubItems.Add(definition.DisplayName);
            item.SubItems.Add(definition.Version.ToString(System.Globalization.CultureInfo.InvariantCulture));
            item.SubItems.Add(definition.IsEnabled ? "Yes" : "No");
            item.SubItems.Add(definition.Description);
            listView.Items.Add(item);
        }

        _workspace.Controls.Add(listView);
    }

    private void ShowWorkflowStepsView()
    {
        WorkflowDefinitionRepository repository = CreateWorkflowDefinitionRepository();
        IReadOnlyList<WorkflowDefinitionRecord> definitions = repository.ListAll();

        TextBox textBox = CreateReadOnlyTextBox();
        System.Text.StringBuilder builder = new();

        foreach (WorkflowDefinitionRecord definition in definitions)
        {
            builder.AppendLine($"Workflow: {definition.WorkflowName} v{definition.Version}");
            IReadOnlyList<WorkflowStepDefinitionRecord> steps = repository.ListSteps(definition.WorkflowDefinitionId);

            if (steps.Count == 0)
            {
                builder.AppendLine("  No steps defined.");
            }

            foreach (WorkflowStepDefinitionRecord step in steps)
            {
                string enabled = step.IsEnabled ? "enabled" : "disabled";
                builder.AppendLine($"  {step.StepOrder}. {step.StepName} ({enabled})");
                builder.AppendLine($"     CommandId: {step.CommandDefinitionId}");
                builder.AppendLine($"     InputMapJson: {step.InputMapJson}");
            }

            builder.AppendLine();
        }

        if (builder.Length == 0)
        {
            builder.AppendLine("No workflow definitions are currently stored in the engine database.");
        }

        textBox.Text = builder.ToString();
        _workspace.Controls.Add(textBox);
    }


    private void ShowExecutionRunsView()
    {
        ExecutionHistoryRepository repository = CreateExecutionHistoryRepository();
        IReadOnlyList<ExecutionHistoryRecord> executions = repository.ListRecent(100);

        ListView listView = new()
        {
            Dock = DockStyle.Fill,
            FullRowSelect = true,
            GridLines = true,
            View = View.Details
        };

        listView.Columns.Add("Started UTC", 180);
        listView.Columns.Add("Status", 120);
        listView.Columns.Add("Kind", 120);
        listView.Columns.Add("Target", 240);
        listView.Columns.Add("Correlation", 240);
        listView.Columns.Add("Completed UTC", 180);
        listView.Columns.Add("Error", 500);

        foreach (ExecutionHistoryRecord execution in executions)
        {
            ListViewItem item = new(execution.StartedUtc);
            item.SubItems.Add(execution.Status);
            item.SubItems.Add(execution.ExecutionKind);
            item.SubItems.Add(execution.TargetName);
            item.SubItems.Add(execution.CorrelationId);
            item.SubItems.Add(execution.CompletedUtc ?? string.Empty);
            item.SubItems.Add(execution.ErrorMessage);
            listView.Items.Add(item);
        }

        _workspace.Controls.Add(listView);
    }

    private void ShowExecutionStepsView()
    {
        ExecutionHistoryRepository repository = CreateExecutionHistoryRepository();
        IReadOnlyList<ExecutionHistoryRecord> executions = repository.ListRecent(100);

        TextBox textBox = CreateReadOnlyTextBox();
        System.Text.StringBuilder builder = new();

        foreach (ExecutionHistoryRecord execution in executions)
        {
            builder.AppendLine($"Execution: {execution.TargetName} [{execution.Status}] {execution.ExecutionId}");
            IReadOnlyList<ExecutionStepHistoryRecord> steps = repository.ListSteps(execution.ExecutionId);

            if (steps.Count == 0)
            {
                builder.AppendLine("  No execution steps recorded.");
            }

            foreach (ExecutionStepHistoryRecord step in steps)
            {
                builder.AppendLine($"  {step.StepOrder}. {step.Status} Started: {step.StartedUtc} Completed: {step.CompletedUtc ?? string.Empty}");
                builder.AppendLine($"     CommandId: {step.CommandDefinitionId ?? string.Empty}");
                builder.AppendLine($"     WorkflowStepId: {step.WorkflowStepDefinitionId ?? string.Empty}");
                builder.AppendLine($"     Error: {step.ErrorMessage}");
            }

            builder.AppendLine();
        }

        if (builder.Length == 0)
        {
            builder.AppendLine("No execution history is currently stored in the engine database.");
        }

        textBox.Text = builder.ToString();
        _workspace.Controls.Add(textBox);
    }

    private TextBox CreateReadOnlyTextBox()
    {
        return new TextBox
        {
            Dock = DockStyle.Fill,
            Multiline = true,
            ScrollBars = ScrollBars.Both,
            ReadOnly = true,
            Font = new Font("Consolas", 10F, FontStyle.Regular),
            WordWrap = false
        };
    }

    private CommandDefinitionRepository CreateCommandDefinitionRepository()
    {
        CommandEngineDatabaseOptions options = CommandEngineDatabaseOptions.ForFile(_databasePath);
        CommandEngineConnectionFactory connectionFactory = new(options);
        return new CommandDefinitionRepository(connectionFactory);
    }

    private WorkflowDefinitionRepository CreateWorkflowDefinitionRepository()
    {
        CommandEngineDatabaseOptions options = CommandEngineDatabaseOptions.ForFile(_databasePath);
        CommandEngineConnectionFactory connectionFactory = new(options);
        return new WorkflowDefinitionRepository(connectionFactory);
    }

    private ExecutionHistoryRepository CreateExecutionHistoryRepository()
    {
        CommandEngineDatabaseOptions options = CommandEngineDatabaseOptions.ForFile(_databasePath);
        CommandEngineConnectionFactory connectionFactory = new(options);
        return new ExecutionHistoryRepository(connectionFactory);
    }

    private void ShowFullDatabaseSchemaView()
    {
        TextBox schemaTextBox = new()
        {
            Dock = DockStyle.Fill,
            Multiline = true,
            ScrollBars = ScrollBars.Both,
            ReadOnly = true,
            Font = new Font("Consolas", 10F, FontStyle.Regular),
            WordWrap = false,
            Text = BuildFullDatabaseSchemaText()
        };

        _workspace.Controls.Add(schemaTextBox);
    }

    private string BuildFullDatabaseSchemaText()
    {
        IReadOnlyList<DatabaseSchemaObject> schemaObjects = LoadDatabaseSchemaObjects();

        System.Text.StringBuilder builder = new();

        builder.AppendLine($"Database Path: {_databasePath}");
        builder.AppendLine($"Schema Version: {CommandEngineSchema.CurrentSchemaVersion}");
        builder.AppendLine();

        foreach (DatabaseSchemaObject schemaObject in schemaObjects)
        {
            builder.AppendLine($"-- {schemaObject.Type.ToUpperInvariant()}: {schemaObject.Name}");

            if (!string.IsNullOrWhiteSpace(schemaObject.Sql))
            {
                builder.AppendLine(schemaObject.Sql.Trim());
            }

            builder.AppendLine();
        }

        return builder.ToString();
    }

    private IReadOnlyList<DatabaseSchemaObject> LoadDatabaseSchemaObjects()
    {
        CommandEngineDatabaseOptions options = CommandEngineDatabaseOptions.ForFile(_databasePath);
        CommandEngineConnectionFactory connectionFactory = new(options);

        using SqliteConnection connection = connectionFactory.OpenConnection();
        using SqliteCommand command = connection.CreateCommand();

        command.CommandText = """
SELECT name, type, sql
FROM sqlite_master
WHERE type IN ('table', 'index', 'view', 'trigger')
  AND name NOT LIKE 'sqlite_%'
ORDER BY type, name;
""";

        List<DatabaseSchemaObject> schemaObjects = [];

        using SqliteDataReader reader = command.ExecuteReader();
        while (reader.Read())
        {
            string name = reader.GetString(0);
            string type = reader.GetString(1);
            string sql = reader.IsDBNull(2) ? string.Empty : reader.GetString(2);

            schemaObjects.Add(new DatabaseSchemaObject(name, type, sql));
        }

        return schemaObjects;
    }

    private sealed record DatabaseSchemaObject(string Name, string Type, string Sql);
}
