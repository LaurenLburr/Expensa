namespace CodexExpensa.ExtensionDevHost.CommandEngineIntegration.Tests.Budgets;

// The former source-text tests in this file were removed as stale.
//
// They asserted that BudgetsDatabasePanelForm directly referenced
// HostBudgetRuntimeDatabaseSelectionService. Database selection and safe
// SQLite access are now delegated through shared services, so requiring that
// class name to appear in the form would enforce an obsolete implementation
// detail rather than behavior.
