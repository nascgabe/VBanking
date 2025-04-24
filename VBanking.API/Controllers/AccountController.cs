using MediatR;
using Microsoft.AspNetCore.Mvc;
using VBanking.Application.Commands;
using VBanking.Domain.Entities;
using VBanking.Domain.Interfaces;

namespace VBanking.API.Controllers
{
    [ApiController]
    [Route("api/accounts")]
    public class AccountController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly IAccountRepository _repository;
        private readonly IAccountDeactivationLogRepository _deactivationLogRepository; 

        public AccountController(IMediator mediator, IAccountRepository repository, 
            IAccountDeactivationLogRepository deactivationLogRepository)
        {
            _mediator = mediator;
            _repository = repository;
            _deactivationLogRepository = deactivationLogRepository;
        }

        [HttpPost]
        public async Task<IActionResult> CreateAccount([FromBody] CreateAccountCommand command)
        {
            var accountId = await _mediator.Send(command);
            return CreatedAtAction(nameof(GetAccountByDocument), new { document = command.Document }, accountId);
        }

        [HttpGet("{document}")]
        public async Task<IActionResult> GetAccountByDocument(string document)
        {
            var account = await _repository.GetByDocumentAsync(document);
            return account != null ? Ok(account) : NotFound();
        }

        [HttpPut("{document}/deactivate")]
        public async Task<IActionResult> DeactivateAccount(string document)
        {
            var account = await _repository.GetByDocumentAsync(document);
            if (account == null || !account.IsActive)
                return BadRequest("Conta não encontrada ou já está inativa.");

            account.Deactivate();
            await _repository.UpdateAsync(account);

            var log = new AccountDeactivationLog(document, "Admin");
            await _deactivationLogRepository.AddLogAsync(log);

            return NoContent();
        }

        [HttpPost("transfer")]
        public async Task<IActionResult> TransferFunds([FromBody] TransferFundsCommand command)
        {
            try
            {
                var success = await _mediator.Send(command);
                return success ? Ok("Transferência realizada com sucesso.") : BadRequest("Falha na transferência.");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}