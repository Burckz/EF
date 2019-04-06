using AutoMapper;
using ProductShop.Dtos.Export;
using ProductShop.Models;

namespace ProductShop
{
    public class ProductShopProfile : Profile
    {
        public ProductShopProfile()
        {
            this.CreateMap<User, SoldProductsDto>()
                .ForMember(spd => spd.ProductsSold, src => src.MapFrom(s => s.ProductsSold));

            this.CreateMap<User, UsersWithProductsDto>()
                .ForMember(spd => spd.SoldProducts, src => src.MapFrom(s => s.ProductsSold));
        }
    }
}
