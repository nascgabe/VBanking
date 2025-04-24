using MediatR;

namespace VBanking.Application.Commands
{
    public record CreateAccountCommand(string Name, string Document) : IRequest<Guid>;
}