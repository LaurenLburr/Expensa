using CoreWorkflowDefinitionDocument = Codex.CommandEngine.Core.WorkflowDefinitionDocument;

namespace Codex.CommandEngine.App;

// Host-local alias wrapper for source compatibility with MainForm.
// The authoritative workflow definition model lives in Codex.CommandEngine.Core.
using WorkflowDefinitionDocument = CoreWorkflowDefinitionDocument;
