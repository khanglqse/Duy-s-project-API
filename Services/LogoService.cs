using AutoMapper;
using DuyProject.API.Configurations;
using DuyProject.API.Models;
using DuyProject.API.ViewModels;
using DuyProject.API.ViewModels.Logo;
using MongoDB.Driver;

namespace DuyProject.API.Services;

public class LogoService
{
    private readonly IMongoCollection<Logo> _logoCollection;
    private readonly IMapper _mapper;

    public LogoService(IMongoClient client, IMapper mapper)
    {
        IMongoDatabase? database = client.GetDatabase(AppSettings.DbName);
        _logoCollection = database.GetCollection<Logo>(nameof(Logo));
        _mapper = mapper;
    }

    public async Task<ServiceResult<PaginationResponse<LogoViewModel>>> List(int page, int pageSize, string? filterValue)
    {
        page = page < 1 ? 1 : page;
        pageSize = pageSize < 0 ? AppSettings.DefaultPageSize : pageSize;
        IQueryable<Logo> query = _logoCollection.AsQueryable().Where(logo => !logo.IsDeleted);

        List<LogoViewModel> items = query
            .OrderBy(x => x.IsActive).ThenBy(x => x.CreatedAt)
            .Skip((page - 1) * pageSize).Take(pageSize).ToList()
            .Select(c => _mapper.Map<LogoViewModel>(c)).ToList();

        int count = items.Count;
        var paginated = new PaginationResponse<LogoViewModel>()
        {
            Items = items,
            Page = page,
            PageSize = pageSize,
            TotalItems = count
        };
        return new ServiceResult<PaginationResponse<LogoViewModel>>(paginated);
    }

    public async Task<ServiceResult<LogoViewModel>> Get(string id)
    {
        Logo? entity = await _logoCollection.Find(c => c.Id == id).FirstOrDefaultAsync();
        if (entity == null) return new ServiceResult<LogoViewModel>("Logo was not found.");
        var data = _mapper.Map<LogoViewModel>(entity);
        return new ServiceResult<LogoViewModel>(data);
    }

    public async Task<ServiceResult<LogoViewModel>> Create(LogoCreateCommand command)
    {
        Logo? entity = _mapper.Map<LogoCreateCommand, Logo>(command);
        await _logoCollection.InsertOneAsync(entity);
        return await Get(entity.Id);
    }


    public async Task<ServiceResult<LogoViewModel>> Update(string id, LogoUpdateCommand command)
    {
        Logo? entity = await _logoCollection.Find(c => c.Id == id && !c.IsDeleted).FirstOrDefaultAsync();
        if (entity == null) return new ServiceResult<LogoViewModel>("Logo was not found.");

        entity.ImagePath = command.ImagePath;
        entity.PharmacyId = command.PharmacyId;
        await _logoCollection.ReplaceOneAsync(p => p.Id == id, entity);
        return await Get(id);
    }

    public async Task<ServiceResult<object>> ToggleActive(string id)
    {
        Logo? entity = await _logoCollection.Find(c => c.Id == id && !c.IsDeleted).FirstOrDefaultAsync();
        if (entity == null) return new ServiceResult<object>("Logo was not found.");
        entity.IsActive = !entity.IsActive;
        await _logoCollection.ReplaceOneAsync(c => c.Id == id, entity);
        return new ServiceResult<object>(new { isActive = entity.IsActive });
    }

    public async Task<ServiceResult<object>> Remove(string id)
    {
        Logo? entity = await _logoCollection.Find(c => c.Id == id && !c.IsDeleted).FirstOrDefaultAsync();
        if (entity == null) return new ServiceResult<object>("Logo was not found.");
        entity.IsDeleted = true;
        await _logoCollection.ReplaceOneAsync(c => c.Id == id, entity);
        return new ServiceResult<object>(new { isDeleted = true });
    }
}