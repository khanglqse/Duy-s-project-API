using AutoMapper;
using DuyProject.API.Models;
using DuyProject.API.ViewModels;
using DuyProject.API.ViewModels.Cause;
using DuyProject.API.ViewModels.Disease;
using DuyProject.API.ViewModels.Drug;
using DuyProject.API.ViewModels.Pharmacy;
using DuyProject.API.ViewModels.User;

namespace DuyProject.API.Configurations;

public class MassMapperProfile : Profile
{
    public MassMapperProfile()
    {
        CreateMap<User, UserViewModel>()
            .ForMember(x => x.Type, opt => opt.MapFrom(x => x.Address.Location.Type))
            .ForMember(x => x.Address, opt => opt.MapFrom(x => x.Address.Address))
            .ForMember(x => x.Street, opt => opt.MapFrom(x => x.Address.Street))
            .ForMember(x => x.City, opt => opt.MapFrom(x => x.Address.City))
            .ForMember(x => x.State, opt => opt.MapFrom(x => x.Address.State))
            .ForMember(x => x.ZipCode, opt => opt.MapFrom(x => x.Address.ZipCode))
            .ForMember(x => x.Coordinates, opt => opt.MapFrom(x => x.Address.Location.Coordinates))
            .ReverseMap()
            .ForMember(x => x.Address, opt => opt.MapFrom(x => new AddressModel()
            {
                Address = x.Address,
                Street = x.Street,
                City = x.City,
                State = x.State,
                ZipCode = x.ZipCode,
                Location = new Location(x.Coordinates)
            }));
        CreateMap<User, LoginUserViewModel>()
            .ForMember(x => x.Roles, opt => opt.MapFrom(src => SplitString(src.Roles)))
            .ReverseMap();
        CreateMap<User, UserCreateCommand>()
            .ForMember(x => x.Type, opt => opt.MapFrom(x => "Point"))
            .ForMember(x => x.Address, opt => opt.MapFrom(x => x.Address.Street))
            .ForMember(x => x.City, opt => opt.MapFrom(x => x.Address.City))
            .ForMember(x => x.State, opt => opt.MapFrom(x => x.Address.State))
            .ForMember(x => x.ZipCode, opt => opt.MapFrom(x => x.Address.ZipCode))
            .ForMember(x => x.Coordinates, opt => opt.MapFrom(x => x.Address.Location.Coordinates))
            .ForMember(u => u.Roles, opt => opt.MapFrom(x => String.IsNullOrWhiteSpace(x.Roles) ? AppSettings.Patient : x.Roles))
            .ReverseMap()
            .ForMember(x => x.Address, opt => opt.MapFrom(x => new AddressModel()
            {
                Address = x.Address,
                Street = x.Street,
                City = x.City,
                State = x.State,
                ZipCode = x.ZipCode,
                Location = new Location(x.Coordinates)
            }));
        CreateMap<User, UserUpdateCommand>()
            .ForMember(x => x.Type, opt => opt.MapFrom(x => "Point"))
            .ForMember(x => x.Address, opt => opt.MapFrom(x => x.Address.Street))
            .ForMember(x => x.City, opt => opt.MapFrom(x => x.Address.City))
            .ForMember(x => x.State, opt => opt.MapFrom(x => x.Address.State))
            .ForMember(x => x.ZipCode, opt => opt.MapFrom(x => x.Address.ZipCode))
            .ForMember(x => x.Coordinates, opt => opt.MapFrom(x => x.Address.Location.Coordinates))
            .ReverseMap()
            .ForMember(x => x.Address, opt => opt.MapFrom(x => new AddressModel()
            {
                Address = x.Address,
                Street = x.Street,
                City = x.City,
                State = x.State,
                ZipCode = x.ZipCode,
                Location = new Location(x.Coordinates)
            }));

        CreateMap<Drug, DrugViewModel>()
            .ReverseMap();
        CreateMap<Drug, DrugCreateCommand>()
            .ReverseMap();
        CreateMap<Drug, DrugUpdateCommand>()
            .ReverseMap();

        CreateMap<Disease, DiseaseViewModel>()
            .ReverseMap();
        CreateMap<Disease, DiseaseCreateCommand>()
            .ReverseMap();
        CreateMap<Disease, DiseaseUpdateCommand>()
            .ReverseMap();

        CreateMap<Cause, CauseViewModel>()
            .ReverseMap();
        CreateMap<Cause, CauseCreateCommand>()
            .ReverseMap();
        CreateMap<Cause, CauseUpdateCommand>()
            .ReverseMap();

        CreateMap<Pharmacy, PharmacyViewModel>()
            .ForMember(x => x.Type, opt => opt.MapFrom(x => x.Address.Location.Type))
            .ForMember(x => x.Address, opt => opt.MapFrom(x => x.Address.Address))
            .ForMember(x => x.Coordinates, opt => opt.MapFrom(x => x.Address.Location.Coordinates))
            .ReverseMap()
            .ForMember(x => x.Address, opt => opt.MapFrom(x => new AddressModel()
            {
                Address = x.Address,
                Location = new Location(x.Coordinates)
            }));
        CreateMap<Pharmacy, PharmacyCreateCommand>()
            .ForMember(x => x.Type, opt => opt.MapFrom(x => "Point"))
            .ForMember(x => x.Address, opt => opt.MapFrom(x => x.Address.Address))
            .ForMember(x => x.Coordinates, opt => opt.MapFrom(x => x.Address.Location.Coordinates))
            .ReverseMap()
            .ForMember(x => x.Address, opt => opt.MapFrom(x => new AddressModel()
            {
                Address = x.Address,
                Location = new Location(x.Coordinates)
            }));
        CreateMap<Pharmacy, PharmacyUpdateCommand>()
            .ForMember(x => x.Type, opt => opt.MapFrom(x => "Point"))
            .ForMember(x => x.Address, opt => opt.MapFrom(x => x.Address.Address))
            .ForMember(x => x.Coordinates, opt => opt.MapFrom(x => x.Address.Location.Coordinates))
            .ReverseMap()
            .ForMember(x => x.Address, opt => opt.MapFrom(x => new AddressModel()
            {
                Address = x.Address,
                Location = new Location(x.Coordinates)
            }));

        // Create your map here
    }

    private IEnumerable<string> SplitString(string srcRoles)
    {
        return srcRoles.Split(",").Select(r => r.Trim());
    }
}

public static class AutoMapperConfiguration
{
    public static IServiceCollection RegisterMapperServices(this IServiceCollection services)
    {
        var mappingConfig = new MapperConfiguration(mc =>
        {
            mc.AddProfile(new MassMapperProfile());
        });

        IMapper? mapper = mappingConfig.CreateMapper();
        services.AddSingleton(mapper);
        return services;
    }
}