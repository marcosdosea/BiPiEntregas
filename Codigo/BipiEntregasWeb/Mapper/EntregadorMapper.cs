using AutoMapper;
using Core;
using BipiEntregasWeb.Models;

namespace BipiEntregasWeb.Mapper
{
    public class EntregadorMapper : Profile
    {
        public EntregadorMapper()
        {
            CreateMap<Pessoa, EntregadorViewModel>()
                .ForMember(
                    dest => dest.DocumentoVeiculo,
                    opt => opt.Ignore()
                );
        }
    }
}
