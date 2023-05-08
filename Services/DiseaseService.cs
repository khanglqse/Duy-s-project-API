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
    private readonly IMongoCollection<Drug> _drugCollection;
    private readonly IMapper _mapper;
    private readonly IMongoCollection<Cause> _causeCollection;

    public DiseaseService(IMongoClient client, IMapper mapper)
    {
        IMongoDatabase? database = client.GetDatabase(AppSettings.DbName);
        _diseaseCollection = database.GetCollection<Disease>(nameof(Disease));
        _causeCollection = database.GetCollection<Cause>(nameof(Cause));
        _drugCollection = database.GetCollection<Drug>(nameof(Drug));
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
        List<Disease> items = query
            .OrderBy(x => x.IsActive).ThenBy(x => x.CreatedAt)
            .Skip((page - 1) * pageSize).Take(pageSize)
            .ToList();

        List<DiseaseViewModel> result = items.Select(c => _mapper.Map<DiseaseViewModel>(c)).ToList();
        foreach (DiseaseViewModel diseaseView in result)
        {
            diseaseView.Causes = _causeCollection.AsQueryable().Where(u => diseaseView.CauseIds.Contains(u.Id)).ToList();
        }

        int count = query.Count();
        var paginated = new PaginationResponse<DiseaseViewModel>
        {
            Items = result,
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
        data.Causes = _causeCollection.AsQueryable().Where(u => data.CauseIds.Contains(u.Id)).ToList();
        return new ServiceResult<DiseaseViewModel>(data);
    }

    public async Task<ServiceResult<DiseaseViewModel>> Create(DiseaseCreateCommand command)
    {
        Disease? entity = _mapper.Map<DiseaseCreateCommand, Disease>(command);

        bool isCauseExisted = CauseVerify(entity);

        if (!isCauseExisted)
        {
            return new ServiceResult<DiseaseViewModel>("Invalid cause.");
        }

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
        entity.CauseIds = entity.CauseIds.Union(command.CauseIds).ToList();
        entity.DrugIds = entity.DrugIds.Union(command.DrugIds).ToList();
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

    private bool CauseVerify(Disease entity)
    {
        return entity.CauseIds.Select(causeId => _causeCollection.AsQueryable().Any(x => x.Id == causeId)).Any(isCauseIdValid => isCauseIdValid);
    }
}