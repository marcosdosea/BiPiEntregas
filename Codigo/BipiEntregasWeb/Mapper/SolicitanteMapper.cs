using AutoMapper;
using Core;
using BipiEntregasWeb.Models;

namespace BipiEntregasWeb.Mapper
{
    public class SolicitanteMapper : Profile
    {
        public SolicitanteMapper()
        {
            CreateMap<Pessoa, SolicitanteViewModel>();
        }
    }
}
