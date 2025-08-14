using Microsoft.AspNetCore.Mvc;
using SmartBank.Application.Interfaces;
using SmartBank.Application.DTOs.Transaction;
using System.ComponentModel.DataAnnotations;

namespace SmartBank.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TransactionController : ControllerBase
    {
        private readonly ITransactionService _transactionService;

        public TransactionController(ITransactionService transactionService)
        {
            _transactionService = transactionService;
        }

        /// <summary>
        /// Yeni bir işlem oluşturur.
        /// </summary>
        [HttpPost]
        public async Task<IActionResult> CreateTransaction([FromBody] CreateTransactionDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _transactionService.CreateTransactionAsync(dto);
            if (result)
                return Ok("İşlem başarıyla oluşturuldu.");
            return BadRequest("İşlem oluşturulamadı.");
        }

        /// <summary>
        /// Tüm işlemleri getirir.
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> GetAllTransactions()
        {
            var result = await _transactionService.GetAllTransactionsAsync();
            return Ok(result);
        }

        /// <summary>
        /// Belirli bir işlem ID' sine göre işlemi getirir.
        /// </summary>
        [HttpGet("{id}")]
        public async Task<IActionResult> GetTransactionById(int id)
        {
            var transaction = await _transactionService.GetTransactionByIdAsync(id);

            if (transaction == null)
                return NotFound("İşlem bulunamadı.");
            
            return Ok(transaction);
        }

        /// <summary>
        /// Belirli bir kart ID' sine ait işlemleri getirir.
        /// </summary>
        [HttpGet("card/{cardId:int}")]
        public async Task<IActionResult> GetByCardId([FromRoute, Range(1, int.MaxValue)] int cardId)
        {
            try
            {
                var result = await _transactionService.GetTransactionByCardIdAsync(cardId);
                return Ok(result);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new {message = ex.Message});
            }
        }
    }
}
