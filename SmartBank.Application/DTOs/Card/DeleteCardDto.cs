using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartBank.Application.DTOs.Card
{
    public class DeleteCardDto
    {
        public int Id { get; set; }
        public string? DeleteReason { get; set; }
    }
}
