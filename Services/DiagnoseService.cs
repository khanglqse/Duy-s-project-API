using AutoMapper;
using DuyProject.API.Configurations;
using DuyProject.API.Models;
using DuyProject.API.ViewModels;
using DuyProject.API.ViewModels.Disease;
using MongoDB.Driver;
using MongoDB.Driver.GeoJsonObjectModel;

namespace DuyProject.API.Services;

public class DiagnoseService
{
    private readonly IMongoCollection<User> _userCollection;
    private readonly IMongoCollection<Disease> _diseaseCollection;
    private readonly IMongoCollection<Cause> _causeCollection;
    private readonly IMongoCollection<Drug> _drugCollection;
    private readonly IMongoCollection<Pharmacy> _pharmacyCollection;
    private readonly IMapper _mapper;
    private readonly UserService _userService;

    public DiagnoseService(IMongoClient client, IMapper mapper, UserService userService)
    {
        IMongoDatabase? database = client.GetDatabase(AppSettings.DbName);
        _userCollection = database.GetCollection<User>(nameof(User));
        _diseaseCollection = database.GetCollection<Disease>(nameof(Disease));
        _causeCollection = database.GetCollection<Cause>(nameof(Cause));
        _drugCollection = database.GetCollection<Drug>(nameof(Drug));
        _pharmacyCollection = database.GetCollection<Pharmacy>(nameof(Pharmacy));
        _userService = userService;
        _mapper = mapper;
    }
    
    public async Task<ServiceResult<PaginationResponse<DiagnoseModel>>> Diagnosis(string userId, DiagnoseRequestModel request) 
    {
        var userInfo = await _userService.GetById(userId);
        var causeFilter = Builders<Cause>.Filter.Where(cause => !cause.IsDeleted);
        if (request.Causes != null)
        {
            causeFilter &= Builders<Cause>.Filter.Where(cause => request.Causes.Contains(cause.Name));
        }

        var causeIds = _causeCollection.Find(causeFilter).ToList().Select(cause => cause.Id);
        IQueryable<Disease> diseaseQuery = _diseaseCollection.AsQueryable().Where(disease => !disease.IsDeleted
            && disease.CauseIds.Any(causeId => causeIds.Contains(causeId)));

        List<Disease> items = diseaseQuery
            .OrderBy(x => x.IsActive).ThenBy(x => x.CreatedAt)
            .Skip((request.PageNumber - 1) * request.PageSize).Take(request.PageSize)
            .ToList();

        List<DiagnoseModel> result = new List<DiagnoseModel>();
        items.Select(c => _mapper.Map<DiseaseViewModel>(c)).ToList();

        var point = GeoJson.Point(GeoJson.Position(
            userInfo.Data.Coordinates[0], 
            userInfo.Data.Coordinates[1])
        );

        // Instantiate builder
        var pharmacyFilter = Builders<Pharmacy>.Filter
           .Near(x => x.Coordinates, point, maxDistance: 10000, minDistance: 0);

        foreach (var diseaseView in items)
        {
            pharmacyFilter &= Builders<Pharmacy>.Filter
                .Where(pharmacy => !pharmacy.IsDeleted &&
                    pharmacy.DrugIds
                    .Any(drugId => diseaseView.DrugIds.Contains(drugId)));
            var model = new DiagnoseModel()
            {
                Disease = diseaseView,
                Pharmacies = _pharmacyCollection.Find(pharmacyFilter).ToList()                    
            };

            result.Add(model);
        }

        int count = diseaseQuery.Count();
        var paginated = new PaginationResponse<DiagnoseModel>
        {
            Items = result,
            Page = request.PageNumber,
            PageSize = request.PageSize,
            TotalItems = count
        };
        return new ServiceResult<PaginationResponse<DiagnoseModel>>(paginated);
    }
}