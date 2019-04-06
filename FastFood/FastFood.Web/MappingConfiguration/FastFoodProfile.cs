namespace FastFood.Web.MappingConfiguration
{
    using AutoMapper;
    using FastFood.Web.ViewModels.Categories;
    using FastFood.Web.ViewModels.Employees;
    using FastFood.Web.ViewModels.Items;
    using FastFood.Web.ViewModels.Orders;
    using Models;

    using ViewModels.Positions;

    public class FastFoodProfile : Profile
    {
        public FastFoodProfile()
        {
            //Positions
            this.CreateMap<CreatePositionInputModel, Position>()
                .ForMember(x => x.Name, y => y.MapFrom(s => s.PositionName));

            this.CreateMap<Position, PositionsAllViewModel>()
                .ForMember(x => x.Name, y => y.MapFrom(s => s.Name));

            //Employees
            this.CreateMap<Position, RegisterEmployeeViewModel>()
                .ForMember(x => x.PositionId, y => y.MapFrom(p => p.Id));

            this.CreateMap<RegisterEmployeeInputModel, Employee>();

            this.CreateMap<Employee, EmployeesAllViewModel>()
                .ForMember(x => x.Position, e => e.MapFrom(s => s.Position.Name));

            //Categories

            this.CreateMap<CreateCategoryInputModel, Category>()
                .ForMember(d => d.Name, s => s.MapFrom(x => x.CategoryName));

            this.CreateMap<Category, CategoryAllViewModel>()
                .ForMember(d => d.Name, s => s.MapFrom(c => c.Name));

            //Items
            this.CreateMap<Category, CreateItemViewModel>()
              .ForMember(x => x.CategoryName, y => y.MapFrom(s => s.Name));

            this.CreateMap<CreateItemInputModel, Item>();

            this.CreateMap<Item, ItemsAllViewModels>()
                .ForMember(d => d.Category, s => s.MapFrom(i => i.Category.Name));

            //Orders

            this.CreateMap<CreateOrderInputModel, Order>();

            this.CreateMap<Order, OrderAllViewModel>()
                .ForMember(d => d.OrderId, s => s.MapFrom(o => o.Id))
                .ForMember(d => d.Employee, s => s.MapFrom(o => o.Employee.Name))
                .ForMember(d => d.DateTime, s => s.MapFrom(o => o.DateTime.ToShortDateString()));
                
        }
    }
}
