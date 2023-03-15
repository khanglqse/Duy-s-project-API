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
            .ReverseMap();
        CreateMap<User, LoginUserViewModel>()
            .ForMember(x => x.Roles, opt => opt.MapFrom(src => SplitString(src.Roles)))
            .ReverseMap();
        CreateMap<User, UserCreateCommand>()
            .ReverseMap();
        CreateMap<User, UserUpdateCommand>()
            .ReverseMap();

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
            .ReverseMap();
        CreateMap<Pharmacy, PharmacyCreateCommand>()
            .ReverseMap();
        CreateMap<Pharmacy, PharmacyUpdateCommand>()
            .ReverseMap();

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