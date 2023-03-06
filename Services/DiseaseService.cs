using AutoMapper;
using DuyProject.API.Configurations;
using DuyProject.API.Models;
using DuyProject.API.ViewModels;
using DuyProject.API.ViewModels.Disease;
using MongoDB.Driver;

namespace DuyProject.API.Services;

public class DiseaseService
{
    private readonly IMongoCollection<Disease> _diseaseCollection;
    private readonly IMapper _mapper;

    public DiseaseService(IMongoClient client, IMapper mapper)
    {
        IMongoDatabase? database = client.GetDatabase(AppSettings.DbName);
        _diseaseCollection = database.GetCollection<Disease>(nameof(Disease));
        _mapper = mapper;
    }

    public async Task<ServiceResult<PaginationResponse<DiseaseViewModel>>> List(int page, int pageSize, string? filterValue)
    {
        page = page < 1 ? 1 : page;
        pageSize = pageSize < 0 ? AppSettings.DefaultPageSize : pageSize;
        IQueryable<Disease> query = _diseaseCollection.AsQueryable().Where(disease => !disease.IsDeleted);
        if (!string.IsNullOrEmpty(filterValue))
        {
            string lowerValue = filterValue.ToLower();
            query = query.Where(p => p.Name.ToLower().Contains(lowerValue));
        }
        List<DiseaseViewModel> items = query
            .OrderBy(x => x.IsActive).ThenBy(x => x.CreatedAt)
            .Skip((page - 1) * pageSize).Take(pageSize)
            .Select(c => _mapper.Map<DiseaseViewModel>(c)).ToList();

        int count = items.Count;
        var paginated = new PaginationResponse<DiseaseViewModel>
        {
            Items = items,
            Page = page,
            PageSize = pageSize,
            TotalItems = count
        };
        return new ServiceResult<PaginationResponse<DiseaseViewModel>>(paginated);
    }

    public async Task<ServiceResult<DiseaseViewModel>> Get(string id)
    {
        Disease? entity = await _diseaseCollection.Find(c => c.Id == id).FirstOrDefaultAsync();
        if (entity == null) return new ServiceResult<DiseaseViewModel>("Disease was not found.");
        var data = _mapper.Map<DiseaseViewModel>(entity);
        return new ServiceResult<DiseaseViewModel>(data);
    }

    public async Task<ServiceResult<DiseaseViewModel>> Create(DiseaseCreateCommand command)
    {
        Disease? entity = _mapper.Map<DiseaseCreateCommand, Disease>(command);
        await _diseaseCollection.InsertOneAsync(entity);
        return await Get(entity.Id);
    }


    public async Task<ServiceResult<DiseaseViewModel>> Update(string id, DiseaseUpdateCommand command)
    {
        Disease? entity = await _diseaseCollection.Find(c => c.Id == id && !c.IsDeleted).FirstOrDefaultAsync();
        if (entity == null) return new ServiceResult<DiseaseViewModel>("Disease was not found.");

        entity.Name = command.Name;
        entity.Description = command.Description;
        entity.BasicExperiment = command.BasicExperiment;
        entity.Approach = command.Approach;
        entity.Treatment = command.Treatment;
        entity.Diet = command.Diet;
        entity.LivingActivity = command.LivingActivity;
        entity.ReferenceImage = command.ReferenceImage;
        entity.Type = command.Type;
        await _diseaseCollection.ReplaceOneAsync(p => p.Id == id, entity);
        return await Get(id);
    }

    public async Task<ServiceResult<object>> ToggleActive(string id)
    {
        Disease? entity = await _diseaseCollection.Find(c => c.Id == id && !c.IsDeleted).FirstOrDefaultAsync();
        if (entity == null) return new ServiceResult<object>("Disease was not found.");
        entity.IsActive = !entity.IsActive;
        await _diseaseCollection.ReplaceOneAsync(c => c.Id == id, entity);
        return new ServiceResult<object>(new { isActive = entity.IsActive });
    }

    public async Task<ServiceResult<object>> Remove(string id)
    {
        Disease? entity = await _diseaseCollection.Find(c => c.Id == id && !c.IsDeleted).FirstOrDefaultAsync();
        if (entity == null) return new ServiceResult<object>("Disease was not found.");
        entity.IsDeleted = true;
        await _diseaseCollection.ReplaceOneAsync(c => c.Id == id, entity);
        return new ServiceResult<object>(new { isDeleted = true });
    }
}