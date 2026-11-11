using AutoMapper;
using BipiEntregasWeb.Models;

namespace BipiEntregasWeb.Mappers
{
    public class EntregadorMapper : Profile
    {
        public EntregadorMapper()
        {
            CreateMap<EntregadorViewModel, Core.Pessoa>()
                .ForMember(
                    dest => dest.DocumentoVeiculo,
                    opt => opt.Ignore());

            CreateMap<Core.Pessoa, EntregadorViewModel>()
                .ForMember(
                    dest => dest.DocumentoVeiculo,
                    opt => opt.Ignore());
        }
    }
}