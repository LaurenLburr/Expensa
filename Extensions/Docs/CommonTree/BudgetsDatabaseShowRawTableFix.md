# Budgets Database Show Raw Table Fix

The Budgets database panel was incorrectly assuming the selected Budget table had a friendly display-name column.

The page now loads the raw selected table:

```sql
SELECT *
FROM [BudgetTable]
LIMIT 500;
```

Database inspection pages should show raw data instead of failing because a table lacks a conventional name column.
