using System.Collections.Generic;

namespace CodexExpensa.Core.Domain.Payees;

public interface IPayeeRepository
{
    IReadOnlyList<Payee> GetAll();

    Payee? GetById(string payeeId);

    Payee? GetByName(string payeeName);

    void Add(Payee payee);

    void Update(Payee payee);

    void Delete(string payeeId);
}