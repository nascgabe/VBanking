using MediatR;
using VBanking.Application.Commands;
using VBanking.Domain.Entities;
using VBanking.Domain.Interfaces;

namespace VBanking.Application.Handlers
{
    public class CreateAccountHandler : IRequestHandler<CreateAccountCommand, Guid>
    {
        private readonly IAccountRepository _repository;

        public CreateAccountHandler(IAccountRepository repository) => _repository = repository;

        public async Task<Guid> Handle(CreateAccountCommand request, CancellationToken cancellationToken)
        {
            if (await _repository.GetByDocumentAsync(request.Document) != null)
                throw new Exception("Já existe uma conta para este documento.");

            var account = new Account(request.Name, request.Document);
            await _repository.CreateAsync(account);
            return account.Id;
        }
    }
}