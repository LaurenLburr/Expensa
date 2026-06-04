using System.Text.Json;
using System.Text.Json.Serialization;

namespace BudgetsAddin;

public static class BudgetJsonSerializerOptions
{
    public static readonly JsonSerializerOptions Default =
        new()
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            WriteIndented = true,
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
        };
}
