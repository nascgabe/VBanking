using Microsoft.EntityFrameworkCore;
using VBanking.Domain.Entities;
using VBanking.Domain.Interfaces;
using VBanking.Infrastructure.Data;

namespace VBanking.Infrastructure.Repositories
{
    public class AccountRepository : IAccountRepository
    {
        private readonly BankingDbContext _context;

        public AccountRepository(BankingDbContext context) => _context = context;

        public async Task<Account?> GetByDocumentAsync(string document) =>
            await _context.Accounts.FirstOrDefaultAsync(acc => acc.Document == document);

        public async Task<List<Account>> GetByNameAsync(string name) =>
            await _context.Accounts.Where(acc => acc.Name.Contains(name)).ToListAsync();

        public async Task CreateAsync(Account account)
        {
            _context.Accounts.Add(account);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(Account account)
        {
            _context.Accounts.Update(account);
            await _context.SaveChangesAsync();
        }
    }
}