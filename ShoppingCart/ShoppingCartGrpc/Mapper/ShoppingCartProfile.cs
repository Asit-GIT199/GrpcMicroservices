using AutoMapper;
using ShoppingCartGrpc.Protos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ShoppingCartGrpc.Mapper
{
    public class ShoppingCartProfile : Profile
    {
        public ShoppingCartProfile()
        {
            CreateMap<Models.ShoppingCart, ShoppingCartModel>().ReverseMap();
            CreateMap<Models.ShoppingCartItem, ShoppingCartItemModel>().ReverseMap();
        }
    }
}
