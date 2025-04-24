using MediatR;

namespace VBanking.Application.Commands
{
    public record TransferFundsCommand(string FromDocument, string ToDocument, decimal Amount) : IRequest<bool>;
}