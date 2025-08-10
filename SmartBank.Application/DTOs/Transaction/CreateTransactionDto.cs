using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartBank.Application.DTOs.Transaction
{
    public class CreateTransactionDto
    {
        public int CardId { get; set; }
        public decimal Amount { get; set; }
        public string Currency {  get; set; }
        public string Description { get; set; }
        public DateTime TransactionDate { get; set; }
    }
}
