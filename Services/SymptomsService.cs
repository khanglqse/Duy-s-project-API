using AutoMapper;
using DuyProject.API.Configurations;
using DuyProject.API.Models;
using DuyProject.API.ViewModels;
using DuyProject.API.ViewModels.Symptoms;
using MongoDB.Driver;

namespace DuyProject.API.Services;

public class SymptomsService
{
    private readonly IMongoCollection<Symptoms> _causeCollection;
    private readonly IMapper _mapper;

    public SymptomsService(IMongoClient client, IMapper mapper)
    {
        IMongoDatabase? database = client.GetDatabase(AppSettings.DbName);
        _causeCollection = database.GetCollection<Symptoms>(nameof(Symptoms));
        _mapper = mapper;
    }

    public async Task<ServiceResult<PaginationResponse<SymptomsViewModel>>> List(int page, int pageSize, string? filterValue)
    {
        page = page < 1 ? AppSettings.DefaultPage : page;
        pageSize = pageSize < 0 ? AppSettings.DefaultPageSize : pageSize;
        IQueryable<Symptoms> query = _causeCollection.AsQueryable().Where(symptoms => !symptoms.IsDeleted);
        if (!string.IsNullOrEmpty(filterValue))
        {
            string lowerValue = filterValue.ToLower();
            query = query.Where(p => p.Name.ToLower().Contains(lowerValue));
        }
        List<Symptoms> items = query
            .OrderBy(x => x.IsActive).ThenBy(x => x.CreatedAt)
            .Skip((page - 1) * pageSize).Take(pageSize)
            .ToList();

        List<SymptomsViewModel> result = items.Select(c => _mapper.Map<SymptomsViewModel>(c)).ToList();

        int count = query.Count();
        var paginated = new PaginationResponse<SymptomsViewModel>
        {
            Items = result,
            Page = page,
            PageSize = pageSize,
            TotalItems = count
        };
        return new ServiceResult<PaginationResponse<SymptomsViewModel>>(paginated);
    }

    public async Task<ServiceResult<SymptomsViewModel>> Get(string id)
    {
        Symptoms? entity = await _causeCollection.Find(c => c.Id == id).FirstOrDefaultAsync();
        if (entity == null) return new ServiceResult<SymptomsViewModel>("Symptoms was not found.");
        var data = _mapper.Map<SymptomsViewModel>(entity);
        return new ServiceResult<SymptomsViewModel>(data);
    }

    public async Task<ServiceResult<SymptomsViewModel>> Create(SymptomsCreateCommand command)
    {
        Symptoms? entity = _mapper.Map<SymptomsCreateCommand, Symptoms>(command);
        await _causeCollection.InsertOneAsync(entity);
        return await Get(entity.Id);
    }


    public async Task<ServiceResult<SymptomsViewModel>> Update(string id, SymptomsUpdateCommand command)
    {
        Symptoms? entity = await _causeCollection.Find(c => c.Id == id && !c.IsDeleted).FirstOrDefaultAsync();
        if (entity == null) return new ServiceResult<SymptomsViewModel>("Symptoms was not found.");

        entity.Name = command.Name;
        entity.Description = command.Description;
        entity.ReferenceImage = command.ReferenceImage;
        await _causeCollection.ReplaceOneAsync(p => p.Id == id, entity);
        return await Get(id);
    }

    public async Task<ServiceResult<object>> ToggleActive(string id)
    {
        Symptoms? entity = await _causeCollection.Find(c => c.Id == id && !c.IsDeleted).FirstOrDefaultAsync();
        if (entity == null) return new ServiceResult<object>("Symptoms was not found.");
        entity.IsActive = !entity.IsActive;
        await _causeCollection.ReplaceOneAsync(c => c.Id == id, entity);
        return new ServiceResult<object>(new { isActive = entity.IsActive });
    }

    public async Task<ServiceResult<object>> Remove(string id)
    {
        Symptoms? entity = await _causeCollection.Find(c => c.Id == id && !c.IsDeleted).FirstOrDefaultAsync();
        if (entity == null) return new ServiceResult<object>("Symptoms was not found.");
        entity.IsDeleted = true;
        await _causeCollection.ReplaceOneAsync(c => c.Id == id, entity);
        return new ServiceResult<object>(new { isDeleted = true });
    }
}