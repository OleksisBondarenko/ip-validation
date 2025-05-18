using AutoMapper;
using System.Collections.Generic;
using ADValidation.DTOs.AccessPolicy;
using ADValidation.Models;

namespace ADValidation.Mappers.AccessRule
{
    public class AccessPolicyMapper
    {
        private readonly Mapper _mapper;

        public AccessPolicyMapper()
        {
            var configuration = new MapperConfiguration(cfg =>
            {
                // Mapping entity => DTO
                cfg.CreateMap<AccessPolicy, AccessPolicyDto>();

                // Mapping DTO => entity for create and update
                cfg.CreateMap<AccessPolicyDto, AccessPolicy>();
            });

            configuration.AssertConfigurationIsValid();

            _mapper = new Mapper(configuration);
        }

        // Entity => DTO
        public AccessPolicyDto MapToDto(AccessPolicy entity)
        {
            return _mapper.Map<AccessPolicyDto>(entity);
        }

        // DTO => Entity for Create
        public AccessPolicy MapFromCreateDto(AccessPolicyDto createDto)
        {
            return _mapper.Map<AccessPolicy>(createDto);
        }

        // DTO => Entity for Update
        public void MapFromUpdateDto(AccessPolicyDto updateDto, AccessPolicy entity)
        {
            _mapper.Map(updateDto, entity);
        }

        // Map list of entities to list of DTOs
        public List<AccessPolicyDto> MapToDtoList(IEnumerable<AccessPolicy> entities)
        {
            return _mapper.Map<List<AccessPolicyDto>>(entities);
        }
    }
}
