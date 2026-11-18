using AutoMapper;
using Core;
using BipiEntregasWeb.Models;

namespace BipiEntregasWeb.Mapper
{
    public class PagamentoMapper : Profile
    {
        public PagamentoMapper()
        {
            CreateMap<Pagamento, PagamentoViewModel>();
        }
    }
}