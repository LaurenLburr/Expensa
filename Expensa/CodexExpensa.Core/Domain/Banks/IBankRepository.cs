using System.Collections.Generic;

namespace CodexExpensa.Core.Domain.Banks;

public interface IBankRepository
{
    IReadOnlyList<Bank> GetAll();

    Bank? GetById(string bankId);

    void Add(Bank bank);

    void Update(Bank bank);

    void Delete(string bankId);
}