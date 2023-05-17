using AutoMapper;
using DuyProject.API.Models;
using DuyProject.API.ViewModels;
using DuyProject.API.ViewModels.Symptoms;
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
            .ForMember(x => x.Address, opt => opt.MapFrom(x => x.Address.Address))
            .ForMember(x => x.Street, opt => opt.MapFrom(x => x.Address.Street))
            .ForMember(x => x.City, opt => opt.MapFrom(x => x.Address.City))
            .ForMember(x => x.State, opt => opt.MapFrom(x => x.Address.State))
            .ForMember(x => x.ZipCode, opt => opt.MapFrom(x => x.Address.ZipCode))
            .ReverseMap()
            .ForMember(x => x.Address, opt => opt.MapFrom(x => new AddressModel()
            {
                Address = x.Address,
                Street = x.Street,
                City = x.City,
                State = x.State,
                ZipCode = x.ZipCode,
            }));
        CreateMap<User, LoginUserViewModel>()
            .ForMember(x => x.Roles, opt => opt.MapFrom(src => SplitString(src.Roles)))
            .ForMember(x => x.Address, opt => opt.MapFrom(x => x.Address.Address))
            .ForMember(x => x.Street, opt => opt.MapFrom(x => x.Address.Street))
            .ForMember(x => x.City, opt => opt.MapFrom(x => x.Address.City))
            .ForMember(x => x.State, opt => opt.MapFrom(x => x.Address.State))
            .ForMember(x => x.ZipCode, opt => opt.MapFrom(x => x.Address.ZipCode))
            .ReverseMap()
            .ForMember(x => x.Address, opt => opt.MapFrom(x => new AddressModel()
            {
                Address = x.Address,
                Street = x.Street,
                City = x.City,
                State = x.State,
                ZipCode = x.ZipCode,
            }));
        CreateMap<User, UserCreateCommand>()
            .ForMember(x => x.Address, opt => opt.MapFrom(x => x.Address.Address))
            .ForMember(x => x.Street, opt => opt.MapFrom(x => x.Address.Street))
            .ForMember(x => x.City, opt => opt.MapFrom(x => x.Address.City))
            .ForMember(x => x.State, opt => opt.MapFrom(x => x.Address.State))
            .ForMember(x => x.ZipCode, opt => opt.MapFrom(x => x.Address.ZipCode))
            .ForMember(u => u.Roles, opt => opt.MapFrom(x => String.IsNullOrWhiteSpace(x.Roles) ? AppSettings.Patient : x.Roles))
            .ReverseMap()
            .ForMember(x => x.Address, opt => opt.MapFrom(x => new AddressModel()
            {
                Address = x.Address,
                Street = x.Street,
                City = x.City,
                State = x.State,
                ZipCode = x.ZipCode,
            }));
        CreateMap<User, UserUpdateCommand>()
            .ForMember(x => x.Address, opt => opt.MapFrom(x => x.Address.Address))
            .ForMember(x => x.Street, opt => opt.MapFrom(x => x.Address.Street))
            .ForMember(x => x.City, opt => opt.MapFrom(x => x.Address.City))
            .ForMember(x => x.State, opt => opt.MapFrom(x => x.Address.State))
            .ForMember(x => x.ZipCode, opt => opt.MapFrom(x => x.Address.ZipCode))
            .ReverseMap()
            .ForMember(x => x.Address, opt => opt.MapFrom(x => new AddressModel()
            {
                Address = x.Address,
                Street = x.Street,
                City = x.City,
                State = x.State,
                ZipCode = x.ZipCode,
            }));

        CreateMap<Drug, DrugViewModel>()
            .ReverseMap();
        CreateMap<Drug, DrugCreateCommand>()
            .ReverseMap();
        CreateMap<Drug, DrugUpdateCommand>()
            .ReverseMap();

        CreateMap<Disease, DiseaseViewModel>()
            .ReverseMap();

        CreateMap<Disease, DiseaseModel>()
            .ReverseMap();
        CreateMap<Disease, DiseaseCreateCommand>()
            .ReverseMap();
        CreateMap<Disease, DiseaseUpdateCommand>()
            .ReverseMap();

        CreateMap<Symptom, SymptomViewModel>()
            .ReverseMap();
        CreateMap<Symptom, SymptomCreateCommand>()
            .ReverseMap();
        CreateMap<Symptom, SymptomUpdateCommand>()
            .ReverseMap();

        CreateMap<Pharmacy, PharmacyViewModel>()
            .ForMember(x => x.Address, opt => opt.MapFrom(x => x.Address.Address))
            .ReverseMap()
            .ForMember(x => x.Address, opt => opt.MapFrom(x => new AddressModel()
            {
                Address = x.Address,
            }));
        CreateMap<Pharmacy, PharmacyCreateCommand>()
            .ForMember(x => x.Address, opt => opt.MapFrom(x => x.Address.Address))
            .ReverseMap()
            .ForMember(x => x.Address, opt => opt.MapFrom(x => new AddressModel()
            {
                Address = x.Address,
            }));
        CreateMap<Pharmacy, PharmacyUpdateCommand>()
            .ForMember(x => x.Address, opt => opt.MapFrom(x => x.Address.Address))
            .ReverseMap()
            .ForMember(x => x.Address, opt => opt.MapFrom(x => new AddressModel()
            {
                Address = x.Address,
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