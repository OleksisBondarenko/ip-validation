using ADValidation.Enums;
using ADValidation.Helpers.Audit;
using ADValidation.Helpers.TimeZone;

namespace ADValidation.DTOs.Audit;

public class AuditRecordDTO
{
        public Guid Id { get; set; } 
        public AuditType AuditType { get; set; }
        public string AuditTypeString { get => AuditTypeHelper.GetAuditTypeString(this.AuditType);  }
        public AuditDataDTO AuditData { get; set; }
        public string ResourceName { get; set; }
        private DateTime timeStamp;
        public DateTime Timestamp
        {
                get
                {
                        var utcTime = TimeZoneHelper.ConvertLocalToUtc(timeStamp, TimeSpan.Zero);
                        return TimeZoneHelper.ConvertUtcToUkrainianTime(utcTime.UtcTime);
                }
                set => timeStamp = value;
        }
}