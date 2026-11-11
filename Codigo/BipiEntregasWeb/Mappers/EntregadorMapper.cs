using AutoMapper;
using Core;
using BipiEntregasWeb.Models;

namespace BipiEntregasWeb.Mappers
{
    public class EntregadorMapper : Profile
    {
        public EntregadorMapper()
        {
            CreateMap<EntregadorViewModel, Pessoa>()
                .ReverseMap();
        }
    }
}