using AutoMapper;
using DuyProject.API.Configurations;
using DuyProject.API.Models;
using DuyProject.API.ViewModels;
using DuyProject.API.ViewModels.Pharmacy;
using MongoDB.Driver;

namespace DuyProject.API.Services;

public class PharmacyService
{
    private readonly IMongoCollection<Pharmacy> _pharmacyCollection;
    private readonly IMapper _mapper;

    public PharmacyService(IMongoClient client, IMapper mapper)
    {
        IMongoDatabase? database = client.GetDatabase(AppSettings.DbName);
        _pharmacyCollection = database.GetCollection<Pharmacy>(nameof(Pharmacy));
        _mapper = mapper;
    }

    public async Task<ServiceResult<PaginationResponse<PharmacyViewModel>>> List(int page, int pageSize, string? filterValue)
    {
        page = page < 1 ? 1 : page;
        pageSize = pageSize < 0 ? AppSettings.DefaultPageSize : pageSize;
        IQueryable<Pharmacy> query = _pharmacyCollection.AsQueryable().Where(pharmacy => !pharmacy.IsDeleted);
        if (!string.IsNullOrEmpty(filterValue))
        {
            string lowerValue = filterValue.ToLower();
            query = query.Where(p => p.Name.ToLower().Contains(lowerValue));
        }
        List<Pharmacy> items = query
            .OrderBy(pharmacy => pharmacy.Name)
            .Skip((page - 1) * pageSize).Take(pageSize).ToList();

        List<PharmacyViewModel> result = items.Select(pharmacy => _mapper.Map<PharmacyViewModel>(pharmacy)).ToList();

        int count = query.Count();
        var paginated = new PaginationResponse<PharmacyViewModel>
        {
            Items = result,
            Page = page,
            PageSize = pageSize,
            TotalItems = count
        };
        return new ServiceResult<PaginationResponse<PharmacyViewModel>>(paginated);
    }

    public async Task<ServiceResult<PharmacyViewModel>> Get(string id)
    {
        Pharmacy? entity = await _pharmacyCollection.Find(c => c.Id == id).FirstOrDefaultAsync();
        if (entity == null) return new ServiceResult<PharmacyViewModel>("Pharmacy was not found.");
        var data = _mapper.Map<PharmacyViewModel>(entity);
        return new ServiceResult<PharmacyViewModel>(data);
    }

    public async Task<ServiceResult<PharmacyViewModel>> Create(PharmacyCreateCommand command)
    {
        Pharmacy? entity = _mapper.Map<PharmacyCreateCommand, Pharmacy>(command);
        await _pharmacyCollection.InsertOneAsync(entity);
        return await Get(entity.Id);
    }


    public async Task<ServiceResult<PharmacyViewModel>> Update(string id, PharmacyUpdateCommand command)
    {
        Pharmacy? entity = await _pharmacyCollection.Find(c => c.Id == id && !c.IsDeleted).FirstOrDefaultAsync();
        if (entity == null) return new ServiceResult<PharmacyViewModel>("Pharmacy was not found.");

        entity.Name = command.Name;
        entity.Address = command.Address;
        entity.Phone = command.Phone;
        entity.Drugs = command.Drugs;
        entity.DoctorId = command.DoctorId;
        entity.LogoId = command.LogoId;
        entity.Column = command.Column;
        entity.ReferenceImage = command.ReferenceImage;
        entity.Type = command.Type;
        await _pharmacyCollection.ReplaceOneAsync(p => p.Id == id, entity);
        return await Get(id);
    }

    public async Task<ServiceResult<object>> ToggleActive(string id)
    {
        Pharmacy? entity = await _pharmacyCollection.Find(c => c.Id == id && !c.IsDeleted).FirstOrDefaultAsync();
        if (entity == null) return new ServiceResult<object>("Pharmacy was not found.");
        entity.IsActive = !entity.IsActive;
        await _pharmacyCollection.ReplaceOneAsync(c => c.Id == id, entity);
        return new ServiceResult<object>(new { isActive = entity.IsActive });
    }

    public async Task<ServiceResult<object>> Remove(string id)
    {
        Pharmacy? entity = await _pharmacyCollection.Find(c => c.Id == id && !c.IsDeleted).FirstOrDefaultAsync();
        if (entity == null) return new ServiceResult<object>("Pharmacy was not found.");
        entity.IsDeleted = true;
        await _pharmacyCollection.ReplaceOneAsync(c => c.Id == id, entity);
        return new ServiceResult<object>(new { isDeleted = true });
    }
}