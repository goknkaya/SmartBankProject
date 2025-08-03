using Microsoft.AspNetCore.Mvc;
using SmartBank.Application.Interfaces;
using SmartBank.Application.DTOs.Card;

namespace SmartBank.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CardController : ControllerBase
    {
        private readonly ICardService _cardService;

        public CardController(ICardService cardService)
        {
            _cardService = cardService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllCards()
        {
            var cards = await _cardService.GetAllCardsAsync();
            return Ok(cards);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetCardById(int id)
        {
            var card = await _cardService.GetCardByIdAsync(id);
            return Ok(card);
        }

        [HttpPost]
        public async Task<IActionResult> CreateCard([FromBody] CreateCardDto dto)
        {
            var result = await _cardService.CreateCardAsync(dto);
            return result ? Ok("Kart başarıyla oluşturuldu.") : BadRequest("Kart oluşturulamadı.");
        }

        [HttpPut]
        public async Task<IActionResult> UpdateCard([FromBody] UpdateCardDto dto)
        {
            var result = await _cardService.UpdateCardAsync(dto);
            return result ? Ok("Kart başarıyla güncellendi.") : BadRequest("Kart güncellenemedi.");
        }

        [HttpDelete]
        public async Task<IActionResult> DeleteCard([FromBody] DeleteCardDto dto)
        {
            var result = await _cardService.DeleteCardAsync(dto);
            return result ? Ok("Kart başarıyla silindi.") : BadRequest("Kart silinemedi.");
        }
    }
}
