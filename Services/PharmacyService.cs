using AutoMapper;
using DuyProject.API.Configurations;
using DuyProject.API.Models;
using DuyProject.API.ViewModels;
using DuyProject.API.ViewModels.Pharmacy;
using MongoDB.Driver;
using System.Linq;

namespace DuyProject.API.Services;

public class PharmacyService
{
    private readonly IMongoCollection<Pharmacy> _pharmacyCollection;
    private readonly IMongoCollection<Drug> _drugCollection;
    private readonly IMongoCollection<User> _userCollection;
    private readonly IMapper _mapper;

    public PharmacyService(IMongoClient client, IMapper mapper)
    {
        IMongoDatabase? database = client.GetDatabase(AppSettings.DbName);
        _pharmacyCollection = database.GetCollection<Pharmacy>(nameof(Pharmacy));
        _drugCollection = database.GetCollection<Drug>(nameof(Drug));
        _userCollection = database.GetCollection<User>(nameof(User));
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
        foreach (PharmacyViewModel pharmacyView in result)
        {
            pharmacyView.Drugs = _drugCollection.AsQueryable().Where(d => pharmacyView.DrugIds.Contains(d.Id)).ToList();
            pharmacyView.Doctor = _userCollection.AsQueryable().Where(u => pharmacyView.DoctorIds.Contains(u.Id)).ToList();
        }

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
        data.Drugs = _drugCollection.AsQueryable().Where(d => data.DrugIds.Contains(d.Id)).ToList();
        data.Doctor = _userCollection.AsQueryable().Where(u => data.DoctorIds.Contains(u.Id)).ToList();

        return new ServiceResult<PharmacyViewModel>(data);
    }

    public async Task<ServiceResult<PharmacyViewModel>> Create(PharmacyCreateCommand command)
    {
        Pharmacy? entity = _mapper.Map<PharmacyCreateCommand, Pharmacy>(command);

        bool isDrugExisted = DrugVerify(entity);

        if (!isDrugExisted)
        {
            return new ServiceResult<PharmacyViewModel>("Pharmacy contain invalid drug.");
        }
        if (!_userCollection.AsQueryable().Any(x => command.DoctorIds.Contains(x.Id)))
        {
            return new ServiceResult<PharmacyViewModel>("No register doctor found.");
        }

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
        entity.DrugIds = entity.DrugIds.Union(command.DrugIds).ToList();
        entity.DoctorIds = entity.DoctorIds.Union(command.DoctorIds).ToList();
        entity.LogoId = command.LogoId;
        entity.Column = command.Column;
        entity.Type = command.Type;
        entity.FollowUser = command.FollowUser;
        await _pharmacyCollection.ReplaceOneAsync(p => p.Id == id, entity);
        return await Get(id);
    }

    public async Task<ServiceResult<PharmacyViewModel>> Follow (string userName, string pharmacyId)
    {
        Pharmacy? entity = await _pharmacyCollection.Find(c => c.Id == pharmacyId && !c.IsDeleted).FirstOrDefaultAsync();
        if (entity == null) return new ServiceResult<PharmacyViewModel>("Pharmacy was not found.");
        PharmacyUpdateCommand command = _mapper.Map<Pharmacy, PharmacyUpdateCommand>(entity);
        command.FollowUser.Add(userName);
        return await Update(pharmacyId,command);
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

    public bool DrugVerify(Pharmacy entity)
    {
        return entity.DrugIds.Select(drugId => _drugCollection.AsQueryable().Any(x => x.Id == drugId)).Any(isDrugIdValid => isDrugIdValid);
    }
}