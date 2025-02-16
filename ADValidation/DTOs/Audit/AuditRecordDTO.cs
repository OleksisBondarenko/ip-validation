namespace ADValidation.DTOs.Audit;

public class AuditRecordDTO
{
        public Guid Id { get; set; } 
        public string Name { get; set; }
        public AuditDataDTO AuditData { get; set; }
        public DateTime Timestamp { get; set; }
}