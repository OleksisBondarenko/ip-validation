using ADValidation.DTOs.Audit;
using ADValidation.Models.Audit;
using AutoMapper;

namespace ADValidation.Mappers.Audit;

public class AuditMapper
{
    private Mapper _mapper;

    public AuditMapper()
    {
        var configuration = new MapperConfiguration(cfg => {
            cfg.CreateMap<AuditData, AuditDataDTO>().ConvertUsing(new AuditDataConverter());
            cfg.CreateMap<AuditRecord, AuditRecordDTO>();
        });
        configuration.AssertConfigurationIsValid();
        
        _mapper = new Mapper(configuration);
    }

    public class AuditDataConverter : ITypeConverter<AuditData, AuditDataDTO>
    {
        public AuditDataDTO Convert(AuditData source, AuditDataDTO destination, ResolutionContext context)
        {
            var config = new MapperConfiguration(cfg => cfg.CreateMap<AuditData, AuditDataDTO>());
            var mapper = new Mapper(config);
            
            return mapper.Map<AuditDataDTO>(source);
        }
    }
    
    public AuditRecordDTO MapToAuditDataDTO(AuditRecord auditRecord)
    {
        return _mapper.Map<AuditRecordDTO>(auditRecord);
    }
}