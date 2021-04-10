using AutoMapper;
using SafeWarehouseApp.Models;

namespace SafeWarehouseApp.Mapping
{
    public class AutoMapperProfile : Profile
    {
        public AutoMapperProfile()
        {
            CreateMap<Customer, Customer>();
            CreateMap<Report, Report>();
            CreateMap<DamageType, DamageType>();
            CreateMap<Material, Material>();
            CreateMap<Supplier, Supplier>();
        }
    }
}