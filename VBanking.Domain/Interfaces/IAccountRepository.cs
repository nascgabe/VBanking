using VBanking.Domain.Entities;

namespace VBanking.Domain.Interfaces
{
    public interface IAccountRepository
    {
        Task<Account?> GetByDocumentAsync(string document);
        Task<List<Account>> GetByNameAsync(string name);
        Task CreateAsync(Account account);
        Task UpdateAsync(Account account);
    }
}