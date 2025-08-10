using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartBank.Domain.Entities
{
    public class Reversal
    {
        public int Id { get; set; }
        //Geri alınan işlem
        public int TransactionId { get; set; }
        public Transaction Transaction { get; set; }
        public string Reason { get; set; } //Geri alım sebebi
        public decimal ReversedAmount { get; set; }  //Geri alınan tutar
        public string Status { get; set; } //Geri alma durumu (Success, Failed, Pending vs)
        public string PerformedBy { get; set; } // Kim yaptı
        public DateTime ReversalDate { get; set; } // Ne zaman yapıldı
        public string ReversalSource { get; set; } // Loglama için (API, Manuel, BatchJob)
        public bool IsCardLimitRestored { get; set; } // Kart limiti iade edildi mi
        public bool IsDeleted { get; set; } // Soft delete' e açık yapı mı

        // Opsiyonel audit
        public string? VoidedBy { get; set; }
        public DateTime? VoidedAt { get; set; }
        public string? VoidReason { get; set; }
    }
}
