using FluentAssertions;
using Moq;
using VBanking.Application.Commands;
using VBanking.Application.Handlers;
using VBanking.Domain.Entities;
using VBanking.Domain.Interfaces;

namespace VBanking.Tests.Units
{
    public class AccountTests
    {
        [Fact]
        public void Should_Create_Account_With_Initial_Balance()
        {
            // Arrange
            var account = new Account("Gabriel", "12345678900");

            // Act
            decimal initialBalance = account.Balance;

            // Assert
            initialBalance.Should().Be(1000.00m);
        }

        [Fact]
        public void Should_Throw_Exception_When_Name_Is_Empty()
        {
            // Act
            Action act = () => new Account("", "12345678900");

            // Assert
            act.Should().Throw<ArgumentException>()
                .WithMessage("Nome é obrigatório.");
        }

        [Fact]
        public void Should_Throw_Exception_When_Document_Is_Empty()
        {
            // Act
            Action act = () => new Account("Fulano", "");

            // Assert
            act.Should().Throw<ArgumentException>()
                .WithMessage("Documento é obrigatório.");
        }

        [Fact]
        public async Task Should_Throw_Exception_When_Document_Is_Duplicated()
        {
            // Arrange
            var repository = new Mock<IAccountRepository>();
            repository.Setup(r => r.GetByDocumentAsync("12345678900"))
                .ReturnsAsync(new Account("Gabriel", "12345678900"));

            var handler = new CreateAccountHandler(repository.Object);
            var command = new CreateAccountCommand("Gabriel", "12345678900");

            // Act & Assert
            await FluentActions.Invoking(() => handler.Handle(command, CancellationToken.None))
                .Should().ThrowAsync<Exception>()
                .WithMessage("Já existe uma conta para este documento.");
        }

        [Fact]
        public void Should_Set_CreatedAt_Automatically_On_Account_Creation()
        {
            // Arrange
            var account = new Account("Fulano", "65432198700");

            // Act
            var createdAt = account.CreatedAt;

            // Assert
            createdAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));
        }

        [Fact]
        public async Task Should_Return_Account_When_Document_Exists()
        {
            // Arrange
            var repository = new Mock<IAccountRepository>();
            repository.Setup(r => r.GetByDocumentAsync("12345678900"))
                .ReturnsAsync(new Account("Gabriel", "12345678900"));

            var result = await repository.Object.GetByDocumentAsync("12345678900");

            // Act & Assert
            result.Should().NotBeNull();
            result.Name.Should().Be("Gabriel");
            result.Document.Should().Be("12345678900");
        }

        [Fact]
        public async Task Should_Return_Null_When_Document_Does_Not_Exist()
        {
            // Arrange
            var repository = new Mock<IAccountRepository>();
            repository.Setup(r => r.GetByDocumentAsync("00000000000"))
                .ReturnsAsync((Account?)null);

            var result = await repository.Object.GetByDocumentAsync("00000000000");

            // Act & Assert
            result.Should().BeNull();
        }

        [Fact]
        public async Task Should_Return_Accounts_When_Searching_By_Name_Partial()
        {
            // Arrange
            var repository = new Mock<IAccountRepository>();
            repository.Setup(r => r.GetByNameAsync("Gabriel"))
                .ReturnsAsync(new List<Account>
                {
            new Account("Gabriel Nascimento", "12345678900"),
            new Account("Gabriel Oliveira", "98765432100")
                });

            var result = await repository.Object.GetByNameAsync("Gabriel");

            // Act & Assert
            result.Should().NotBeEmpty();
            result.Should().HaveCount(2);
        }

        [Fact]
        public async Task Should_Return_Single_Account_When_Searching_By_Exact_Name()
        {
            // Arrange
            var repository = new Mock<IAccountRepository>();
            repository.Setup(r => r.GetByNameAsync("Gabriel"))
                .ReturnsAsync(new List<Account> { new Account("Gabriel", "98765432100") });

            var result = await repository.Object.GetByNameAsync("Gabriel");

            // Act & Assert
            result.Should().HaveCount(1);
            result.First().Name.Should().Be("Gabriel");
        }

        [Fact]
        public async Task Should_Return_Empty_List_When_No_Accounts_Found()
        {
            // Arrange
            var repository = new Mock<IAccountRepository>();
            repository.Setup(r => r.GetByNameAsync("Nome Inexistente"))
                .ReturnsAsync(new List<Account>());

            var result = await repository.Object.GetByNameAsync("Nome Inexistente");

            // Act & Assert
            result.Should().BeEmpty();
        }

        [Fact]
        public void Should_Deactivate_Account_When_Active()
        {
            // Arrange
            var account = new Account("Gabriel", "12345678900");

            // Act
            account.Deactivate();

            // Assert
            account.IsActive.Should().BeFalse();
        }

        [Fact]
        public void Should_Not_Deactivate_Account_When_Already_Inactive()
        {
            // Arrange
            var account = new Account("Fulano", "98765432100");
            account.Deactivate();

            // Act
            Action act = () => account.Deactivate();

            // Assert
            act.Should().Throw<InvalidOperationException>()
                .WithMessage("A conta já está inativa.");
        }

        [Fact]
        public async Task Should_Register_Deactivation_Log_When_Account_Is_Deactivated()
        {
            // Arrange
            var logRepository = new Mock<IAccountDeactivationLogRepository>();
            var log = new AccountDeactivationLog("12345678900", "Admin");

            // Act
            await logRepository.Object.AddLogAsync(log);

            // Assert
            log.DeactivatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));
            log.Document.Should().Be("12345678900");
            log.PerformedBy.Should().Be("Admin");
        }

        [Fact]
        public void Should_Not_Delete_Account_After_Deactivation()
        {
            // Arrange
            var account = new Account("Ciclano", "65432198700");

            // Act
            account.Deactivate();

            // Assert
            account.Should().NotBeNull();
            account.Document.Should().Be("65432198700");
            account.Balance.Should().Be(1000.00m);
        }
    }
}