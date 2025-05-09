﻿using MediatR;
using VBanking.Application.Commands;
using VBanking.Domain.Entities;
using VBanking.Domain.Interfaces;

namespace VBanking.Application.Handlers
{
    public class TransferFundsHandler : IRequestHandler<TransferFundsCommand, bool>
    {
        private readonly IAccountRepository _repository;
        private readonly ITransferAuditLogRepository _transactionLogRepository;

        public TransferFundsHandler(IAccountRepository repository,
            ITransferAuditLogRepository transferAuditLogRepository)
        {
            _repository = repository;
            _transactionLogRepository = transferAuditLogRepository;
        }

        public async Task<bool> Handle(TransferFundsCommand request, CancellationToken cancellationToken)
        {
            if (request.FromDocument == request.ToDocument)
                throw new InvalidOperationException("Não é possível transferir para a mesma conta.");

            var fromAccount = await _repository.GetByDocumentAsync(request.FromDocument);
            var toAccount = await _repository.GetByDocumentAsync(request.ToDocument);

            if (fromAccount == null || toAccount == null)
                throw new Exception("Uma ou ambas as contas não foram encontradas.");

            if (!fromAccount.IsActive || !toAccount.IsActive)
                throw new Exception("Ambas as contas devem estar ativas para realizar a transferência.");

            if (fromAccount.Balance < request.Amount)
                throw new Exception("Saldo insuficiente na conta de origem.");

            fromAccount.Withdraw(request.Amount);
            toAccount.Deposit(request.Amount);

            await _repository.UpdateAsync(fromAccount);
            await _repository.UpdateAsync(toAccount);

            var auditLog = new TransferAuditLog(request.FromDocument, request.ToDocument, request.Amount);
            await _transactionLogRepository.AddLogAsync(auditLog);

            return true;
        }
    }
}