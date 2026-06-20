using System.Reflection;
using System.Text.Json;
using CodexExpensa.App.WinForms.UI.Addins;
using Xunit;

namespace CodexExpensa.App.WinForms.Tests.UI.Addins;

public sealed class BudgetAddinScreenCapabilityTests
{
    [Fact]
    public void ParseScreen_WhenBudgetAddinAllowsTransactions_PreservesCapability()
    {
        string json =
            JsonSerializer.Serialize(
                new
                {
                    title = "June 2026",
                    columns = new[] { "RowType", "Item", "Amount" },
                    rows = new[]
                    {
                        new Dictionary<string, object?>
                        {
                            ["RowType"] = "Budget",
                            ["Item"] = "Power Company",
                            ["Amount"] = 125m
                        }
                    },
                    allowAddTransaction = true
                });

        AddinScreenModel screen =
            InvokeParseScreen(json);

        Assert.True(screen.AllowAddTransaction);
    }

    private static AddinScreenModel InvokeParseScreen(string json)
    {
        MethodInfo method =
            typeof(AddinScreenRuntimeInvoker).GetMethod(
                "ParseScreen",
                BindingFlags.Static | BindingFlags.NonPublic)
            ?? throw new InvalidOperationException(
                "ParseScreen was not found.");

        return (AddinScreenModel?)method.Invoke(null, [json])
            ?? throw new InvalidOperationException(
                "ParseScreen returned null.");
    }
}
