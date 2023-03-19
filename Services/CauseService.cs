using AutoMapper;
using DuyProject.API.Configurations;
using DuyProject.API.Models;
using DuyProject.API.ViewModels;
using DuyProject.API.ViewModels.Cause;
using MongoDB.Driver;

namespace DuyProject.API.Services;

public class CauseService
{
    private readonly IMongoCollection<Cause> _causeCollection;
    private readonly IMapper _mapper;

    public CauseService(IMongoClient client, IMapper mapper)
    {
        IMongoDatabase? database = client.GetDatabase(AppSettings.DbName);
        _causeCollection = database.GetCollection<Cause>(nameof(Cause));
        _mapper = mapper;
    }

    public async Task<ServiceResult<PaginationResponse<CauseViewModel>>> List(int page, int pageSize, string? filterValue)
    {
        page = page < 1 ? 1 : page;
        pageSize = pageSize < 0 ? AppSettings.DefaultPageSize : pageSize;
        IQueryable<Cause> query = _causeCollection.AsQueryable().Where(cause => !cause.IsDeleted);
        if (!string.IsNullOrEmpty(filterValue))
        {
            string lowerValue = filterValue.ToLower();
            query = query.Where(p => p.Name.ToLower().Contains(lowerValue));
        }
        List<Cause> items = query
            .OrderBy(x => x.IsActive).ThenBy(x => x.CreatedAt)
            .Skip((page - 1) * pageSize).Take(pageSize)
            .ToList();

        List<CauseViewModel> result = items.Select(c => _mapper.Map<CauseViewModel>(c)).ToList();

        int count = query.Count();
        var paginated = new PaginationResponse<CauseViewModel>
        {
            Items = result,
            Page = page,
            PageSize = pageSize,
            TotalItems = count
        };
        return new ServiceResult<PaginationResponse<CauseViewModel>>(paginated);
    }

    public async Task<ServiceResult<CauseViewModel>> Get(string id)
    {
        Cause? entity = await _causeCollection.Find(c => c.Id == id).FirstOrDefaultAsync();
        if (entity == null) return new ServiceResult<CauseViewModel>("Cause was not found.");
        var data = _mapper.Map<CauseViewModel>(entity);
        return new ServiceResult<CauseViewModel>(data);
    }

    public async Task<ServiceResult<CauseViewModel>> Create(CauseCreateCommand command)
    {
        Cause? entity = _mapper.Map<CauseCreateCommand, Cause>(command);
        await _causeCollection.InsertOneAsync(entity);
        return await Get(entity.Id);
    }


    public async Task<ServiceResult<CauseViewModel>> Update(string id, CauseUpdateCommand command)
    {
        Cause? entity = await _causeCollection.Find(c => c.Id == id && !c.IsDeleted).FirstOrDefaultAsync();
        if (entity == null) return new ServiceResult<CauseViewModel>("Cause was not found.");

        entity.Name = command.Name;
        entity.Description = command.Description;
        entity.ReferenceImage = command.ReferenceImage;
        await _causeCollection.ReplaceOneAsync(p => p.Id == id, entity);
        return await Get(id);
    }

    public async Task<ServiceResult<object>> ToggleActive(string id)
    {
        Cause? entity = await _causeCollection.Find(c => c.Id == id && !c.IsDeleted).FirstOrDefaultAsync();
        if (entity == null) return new ServiceResult<object>("Cause was not found.");
        entity.IsActive = !entity.IsActive;
        await _causeCollection.ReplaceOneAsync(c => c.Id == id, entity);
        return new ServiceResult<object>(new { isActive = entity.IsActive });
    }

    public async Task<ServiceResult<object>> Remove(string id)
    {
        Cause? entity = await _causeCollection.Find(c => c.Id == id && !c.IsDeleted).FirstOrDefaultAsync();
        if (entity == null) return new ServiceResult<object>("Cause was not found.");
        entity.IsDeleted = true;
        await _causeCollection.ReplaceOneAsync(c => c.Id == id, entity);
        return new ServiceResult<object>(new { isDeleted = true });
    }
}