namespace VBanking.Domain.Entities
{
    public class Account
    {
        public Guid Id { get; private set; }
        public string Name { get; private set; }
        public string Document { get; private set; }
        public decimal Balance { get; private set; }
        public DateTime CreatedAt { get; private set; }
        public bool IsActive { get; private set; }

        public Account(string name, string document)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("Nome é obrigatório.");

            if (string.IsNullOrWhiteSpace(document))
                throw new ArgumentException("Documento é obrigatório.");

            Id = Guid.NewGuid();
            Name = name;
            Document = document;
            Balance = 1000.00m;
            CreatedAt = DateTime.UtcNow;
            IsActive = true;
        }

        public void Deactivate()
        {
            if (!IsActive)
                throw new InvalidOperationException("A conta já está inativa.");

            IsActive = false;
        }

        public void Withdraw(decimal amount)
        {
            if (amount <= 0)
                throw new ArgumentException("Valor da transferência deve ser positivo.");

            if (!IsActive)
                throw new InvalidOperationException("A conta de origem está inativa.");

            if (Balance < amount)
                throw new InvalidOperationException("Saldo insuficiente.");

            Balance -= amount;
        }

        public void Deposit(decimal amount)
        {
            if (!IsActive)
                throw new InvalidOperationException("A conta de destino está inativa.");

            if (amount <= 0)
                throw new ArgumentException("Valor do depósito deve ser positivo.");

            Balance += amount;
        }
    }
}