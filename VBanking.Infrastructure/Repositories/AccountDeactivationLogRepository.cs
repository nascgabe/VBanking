using VBanking.Domain.Entities;
using VBanking.Domain.Interfaces;
using VBanking.Infrastructure.Data;

namespace VBanking.Infrastructure.Repositories
{
    public class AccountDeactivationLogRepository : IAccountDeactivationLogRepository
    {
        private readonly BankingDbContext _context;

        public AccountDeactivationLogRepository(BankingDbContext context) => _context = context;

        public async Task AddLogAsync(AccountDeactivationLog log)
        {
            _context.AccountDeactivationLogs.Add(log);
            await _context.SaveChangesAsync();
        }
    }
}