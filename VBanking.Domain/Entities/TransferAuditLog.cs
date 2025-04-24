namespace VBanking.Domain.Entities
{
    public class TransferAuditLog
    {
        public Guid Id { get; private set; }
        public string FromDocument { get; private set; }
        public string ToDocument { get; private set; }
        public decimal Amount { get; private set; }
        public DateTime Timestamp { get; private set; }

        public TransferAuditLog(string fromDocument, string toDocument, decimal amount)
        {
            Id = Guid.NewGuid();
            FromDocument = fromDocument;
            ToDocument = toDocument;
            Amount = amount;
            Timestamp = DateTime.UtcNow;
        }
    }
}