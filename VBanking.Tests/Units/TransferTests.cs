using FluentAssertions;
using Moq;
using VBanking.Application.Commands;
using VBanking.Application.Handlers;
using VBanking.Domain.Entities;
using VBanking.Domain.Interfaces;

namespace VBanking.Tests.Units
{
    public class TransferTests
    {
        [Fact]
        public void Should_Transfer_Funds_When_Balance_Is_Sufficient()
        {
            // Arrange
            var fromAccount = new Account("Gabriel", "11122233344");
            var toAccount = new Account("Fulano", "55566677788");

            // Act
            fromAccount.Withdraw(200);
            toAccount.Deposit(200);

            // Assert
            fromAccount.Balance.Should().Be(800m);
            toAccount.Balance.Should().Be(1200m);
        }

        [Fact]
        public void Should_Throw_Exception_When_Insufficient_Balance()
        {
            // Arrange
            var fromAccount = new Account("Gabriel", "11122233344");

            // Act & Assert
            Action act = () => fromAccount.Withdraw(2000);

            act.Should().Throw<InvalidOperationException>()
                .WithMessage("Saldo insuficiente.");
        }

        [Fact]
        public void Should_Throw_Exception_When_Transferring_To_Inactive_Account()
        {
            // Arrange
            var fromAccount = new Account("Gabriel", "11122233344");
            var toAccount = new Account("Fulano", "55566677788");
            toAccount.Deactivate();

            // Act & Assert
            Action act = () =>
            {
                fromAccount.Withdraw(200);
                toAccount.Deposit(200);
            };

            act.Should().Throw<InvalidOperationException>()
                .WithMessage("A conta de destino está inativa.");
        }

        [Fact]
        public void Should_Throw_Exception_When_Transferring_From_Inactive_Account()
        {
            // Arrange
            var fromAccount = new Account("Gabriel", "11122233344");
            var toAccount = new Account("Fulano", "55566677788");
            fromAccount.Deactivate();

            // Act & Assert
            Action act = () => fromAccount.Withdraw(200);

            act.Should().Throw<InvalidOperationException>()
                .WithMessage("A conta de origem está inativa.");
        }

        [Fact]
        public void Should_Transfer_Exact_Balance()
        {
            // Arrange
            var fromAccount = new Account("Gabriel", "11122233344");
            var toAccount = new Account("Fulano", "55566677788");

            // Act
            fromAccount.Withdraw(1000);
            toAccount.Deposit(1000);

            // Assert
            fromAccount.Balance.Should().Be(0);
            toAccount.Balance.Should().Be(2000);
        }

        [Fact]
        public void Should_Not_Affect_Other_Accounts_When_Transferring()
        {
            // Arrange
            var fromAccount = new Account("Gabriel", "11122233344");
            var toAccount = new Account("Fulano", "55566677788");
            var thirdAccount = new Account("Ciclano", "99988877766");

            // Act
            fromAccount.Withdraw(500);
            toAccount.Deposit(500);

            // Assert
            thirdAccount.Balance.Should().Be(1000);
        }

        [Fact]
        public void Should_Throw_Exception_When_Transferring_Negative_Value()
        {
            // Arrange
            var fromAccount = new Account("Gabriel", "11122233344");
            var toAccount = new Account("Fulano", "55566677788");

            // Act & Assert
            Action act = () => fromAccount.Withdraw(-500);

            act.Should().Throw<ArgumentException>()
                .WithMessage("Valor da transferência deve ser positivo.");
        }

        [Fact]
        public async Task Should_Throw_Exception_When_Transferring_To_Same_Account()
        {
            // Arrange
            var repository = new Mock<IAccountRepository>();
            var auditLogRepository = new Mock<ITransferAuditLogRepository>();

            repository.Setup(r => r.GetByDocumentAsync("11122233344"))
                .ReturnsAsync(new Account("Gabriel", "11122233344"));

            var handler = new TransferFundsHandler(repository.Object, auditLogRepository.Object);
            var command = new TransferFundsCommand("11122233344", "11122233344", 200);

            // Act & Assert
            await FluentActions.Invoking(() => handler.Handle(command, CancellationToken.None))
                .Should().ThrowAsync<InvalidOperationException>()
                .WithMessage("Não é possível transferir para a mesma conta.");
        }

        [Fact]
        public async Task Should_Throw_Exception_When_Banking_System_Is_Down()
        {
            // Arrange
            var repository = new Mock<IAccountRepository>();
            var auditLogRepository = new Mock<ITransferAuditLogRepository>();

            // Simular que as contas existem
            repository.Setup(r => r.GetByDocumentAsync("11122233344"))
                .ReturnsAsync(new Account("Gabriel", "11122233344"));

            repository.Setup(r => r.GetByDocumentAsync("55566677788"))
                .ReturnsAsync(new Account("Fulano", "55566677788"));

            // Simular erro ao tentar atualizar o banco
            repository.Setup(r => r.UpdateAsync(It.IsAny<Account>()))
                .ThrowsAsync(new Exception("Banco de dados indisponível"));

            var handler = new TransferFundsHandler(repository.Object, auditLogRepository.Object);
            var command = new TransferFundsCommand("11122233344", "55566677788", 200);

            // Act & Assert
            await FluentActions.Invoking(() => handler.Handle(command, CancellationToken.None))
                .Should().ThrowAsync<Exception>()
                .WithMessage("Banco de dados indisponível");
        }
    }
}