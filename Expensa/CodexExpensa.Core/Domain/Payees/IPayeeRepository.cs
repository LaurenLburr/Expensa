using System.Collections.Generic;

namespace CodexExpensa.Core.Domain.Payees;

public interface IPayeeRepository
{
    IReadOnlyList<Payee> GetAll();

    IReadOnlyList<Payee> GetBudgetTemplatePayees();

    Payee? GetById(string payeeId);

    Payee? GetByName(string payeeName);

    void Add(Payee payee);

    void Update(Payee payee);

    void UpdateIncludeInBudgetTemplate(string payeeId, bool includeInBudgetTemplate);

    void Delete(string payeeId);
}