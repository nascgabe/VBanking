using VBanking.Domain.Entities;
using VBanking.Domain.Interfaces;
using VBanking.Infrastructure.Data;

namespace VBanking.Infrastructure.Repositories
{
    public class TransferAuditLogRepository : ITransferAuditLogRepository
    {
        private readonly BankingDbContext _context;

        public TransferAuditLogRepository(BankingDbContext context) => _context = context;

        public async Task AddLogAsync(TransferAuditLog log)
        {
            _context.TransferAuditLogs.Add(log);
            await _context.SaveChangesAsync();
        }
    }
}