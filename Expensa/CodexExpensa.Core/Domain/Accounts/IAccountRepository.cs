namespace CodexExpensa.Core.Domain.Accounts;

public interface IAccountRepository
{
    IReadOnlyList<Account> GetAll();

    Account? GetById(string accountId);

    void Add(Account account);

    void Update(Account account);

    void Delete(string accountId);
}