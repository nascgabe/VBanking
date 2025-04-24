using VBanking.Domain.Entities;

namespace VBanking.Domain.Interfaces
{
    public interface ITransferAuditLogRepository
    {
        Task AddLogAsync(TransferAuditLog log);
    }
}