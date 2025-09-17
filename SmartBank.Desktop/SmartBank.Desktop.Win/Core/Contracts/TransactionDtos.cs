using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartBank.Desktop.Win.Core.Contracts
{
    // LIST/GET
    public class SelectTransactionDto
    {
        public int Id { get; set; }
        public int CardId { get; set; }
        public string Currency { get; set; } = "TRY";
        public decimal Amount { get; set; }
        public DateTime TransactionDate { get; set; }
        public string Status { get; set; } = "S";
        public string? Description { get; set; }
        public string Card { get; set; } = ""; // sadece gösterim için (gridde “Kart” kolonu buna bakacak)
    }

    // CREATE
    public class CreateTransactionDto
    {
        public int CardId { get; set; }
        public decimal Amount { get; set; }
        public string Currency { get; set; } = "TRY";
        public string? Description { get; set; }
        public DateTime TransactionDate { get; set; } = DateTime.Now;
    }
}
