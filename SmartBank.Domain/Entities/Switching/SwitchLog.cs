using System.ComponentModel.DataAnnotations;

namespace SmartBank.Domain.Entities.Switching
{
    public class SwitchLog
    {
        public int Id { get; set; }
        public int MessageId { get; set; }
        public SwitchMessage Message { get; set; } = null!;

        [MaxLength(20)]
        public string Stage { get; set; } = "";     // Received/Routed/Persisted/Responded

        [MaxLength(10)]
        public string Level { get; set; } = "INFO"; // INFO/WARN/ERROR

        [MaxLength(200)]
        public string? Note { get; set; }

        public string? PayloadIn { get; set; }   // JSON request
        public string? PayloadOut { get; set; }  // JSON response/decision
        public DateTime CreatedAt { get; set; } = DateTime.Now;
    }
}
