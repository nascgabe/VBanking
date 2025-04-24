namespace VBanking.Domain.Entities
{
    public class AccountDeactivationLog
    {
        public Guid Id { get; private set; }
        public string Document { get; private set; }
        public DateTime DeactivatedAt { get; private set; }
        public string PerformedBy { get; private set; }

        public AccountDeactivationLog(string document, string performedBy)
        {
            Id = Guid.NewGuid();
            Document = document;
            DeactivatedAt = DateTime.UtcNow;
            PerformedBy = performedBy;
        }
    }
}