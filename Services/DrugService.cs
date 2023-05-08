using AutoMapper;
using DuyProject.API.Configurations;
using DuyProject.API.Models;
using DuyProject.API.ViewModels;
using DuyProject.API.ViewModels.Drug;
using MongoDB.Bson;
using MongoDB.Driver;

namespace DuyProject.API.Services;

public class DrugService
{
    private readonly IMongoCollection<Drug> _drugCollection;
    private readonly IMapper _mapper;

    public DrugService(IMongoClient client, IMapper mapper)
    {
        IMongoDatabase? database = client.GetDatabase(AppSettings.DbName);
        _drugCollection = database.GetCollection<Drug>(nameof(Drug));
        _mapper = mapper;
    }

    public async Task<ServiceResult<PaginationResponse<DrugViewModel>>> List(int page, int pageSize, string? filterValue)
    {
        page = page < 1 ? 1 : page;
        pageSize = pageSize < 0 ? AppSettings.DefaultPageSize : pageSize;
        IQueryable<Drug> query = _drugCollection.AsQueryable().Where(drug => !drug.IsDeleted);

        if (!string.IsNullOrEmpty(filterValue))
        {
            string lowerValue = filterValue.ToLower();
            query = query.Where(p => p.Name.ToLower().Contains(lowerValue));
        }

        List<Drug> items = query
            .OrderBy(x => x.IsActive).ThenBy(x => x.CreatedAt)
            .Skip((page - 1) * pageSize).Take(pageSize)
            .ToList();

        List<DrugViewModel> result = items.Select(c => _mapper.Map<DrugViewModel>(c)).ToList();

        int count = query.Count();
        var paginated = new PaginationResponse<DrugViewModel>
        {
            Items = result,
            Page = page,
            PageSize = pageSize,
            TotalItems = count
        };
        return new ServiceResult<PaginationResponse<DrugViewModel>>(paginated);
    }

    public async Task<ServiceResult<DrugViewModel>> Get(string id)
    {
        Drug? entity = await _drugCollection.Find(c => c.Id == id).FirstOrDefaultAsync();
        if (entity == null) return new ServiceResult<DrugViewModel>("Drug was not found.");
        var data = _mapper.Map<DrugViewModel>(entity);
        return new ServiceResult<DrugViewModel>(data);
    }

    public async Task<ServiceResult<DrugViewModel>> Create(DrugCreateCommand command)
    {
        Drug? entity = _mapper.Map<DrugCreateCommand, Drug>(command);
        await _drugCollection.InsertOneAsync(entity);
        return await Get(entity.Id);
    }

    public async Task<ServiceResult<DrugViewModel>> Update(string id, DrugUpdateCommand command)
    {
        Drug? entity = await _drugCollection.Find(c => c.Id == id && !c.IsDeleted).FirstOrDefaultAsync();
        if (entity == null) return new ServiceResult<DrugViewModel>("Drug was not found.");

        entity.Name = command.Name;
        entity.Effect = command.Effect;
        entity.Description = command.Description;
        entity.Price = command.Price;
        entity.Type = command.Type;
        entity.Quatity = command.Quatity;
        await _drugCollection.ReplaceOneAsync(p => p.Id == id, entity);
        return await Get(id);
    }

    public async Task<ServiceResult<object>> ToggleActive(string id)
    {
        Drug? entity = await _drugCollection.Find(c => c.Id == id && !c.IsDeleted).FirstOrDefaultAsync();
        if (entity == null) return new ServiceResult<object>("Drug was not found.");
        entity.IsActive = !entity.IsActive;
        await _drugCollection.ReplaceOneAsync(c => c.Id == id, entity);
        return new ServiceResult<object>(new { isActive = entity.IsActive });
    }

    public async Task<ServiceResult<object>> Remove(string id)
    {
        Drug? entity = await _drugCollection.Find(c => c.Id == id && !c.IsDeleted).FirstOrDefaultAsync();
        if (entity == null) return new ServiceResult<object>("Drug was not found.");
        entity.IsDeleted = true;
        await _drugCollection.ReplaceOneAsync(c => c.Id == id, entity);
        return new ServiceResult<object>(new { isDeleted = true });
    }
}