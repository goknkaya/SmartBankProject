using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartBank.Application.DTOs.Transaction
{
    public class SelectTransactionDto
    {
        public int Id { get; set; }
        public int CardId { get; set; }
        public string Currency {  get; set; }
        public decimal Amount { get; set; }
        public DateTime TransactionDate { get; set; }
        public string Status { get; set; }
        public string Description { get; set; }
        public string Card { get; set; } = "";     // sadece gösterim için (gridde “Kart” kolonu buna bakacak)

    }
}
