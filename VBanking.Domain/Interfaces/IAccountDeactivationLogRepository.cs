using VBanking.Domain.Entities;

namespace VBanking.Domain.Interfaces
{
    public interface IAccountDeactivationLogRepository
    {
        Task AddLogAsync(AccountDeactivationLog log);
    }
}