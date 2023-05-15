using AutoMapper;
using DuyProject.API.Configurations;
using DuyProject.API.Models;
using DuyProject.API.ViewModels;
using DuyProject.API.ViewModels.Disease;
using DuyProject.API.ViewModels.Pharmacy;
using MongoDB.Driver;

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
    private readonly GoogleMapService _googleMapService;

    public DiagnoseService(IMongoClient client, IMapper mapper, UserService userService, GoogleMapService googleMapService)
    {
        IMongoDatabase? database = client.GetDatabase(AppSettings.DbName);
        _userCollection = database.GetCollection<User>(nameof(User));
        _diseaseCollection = database.GetCollection<Disease>(nameof(Disease));
        _causeCollection = database.GetCollection<Cause>(nameof(Cause));
        _drugCollection = database.GetCollection<Drug>(nameof(Drug));
        _pharmacyCollection = database.GetCollection<Pharmacy>(nameof(Pharmacy));
        _userService = userService;
        _mapper = mapper;
        _googleMapService = googleMapService;
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

        foreach (var diseaseView in items)
        {
            var pharmaciesView = new List<PharmacyViewModel>();
            var pharmacyFilter = Builders<Pharmacy>.Filter
                .Where(pharmacy => !pharmacy.IsDeleted
                    && pharmacy.Address.Address != null
                    && pharmacy.DrugIds
                    .Any(drugId => diseaseView.DrugIds.Contains(drugId)));

            var pharmacies = _pharmacyCollection.Find(pharmacyFilter).ToList();

            foreach (var pharmacy in pharmacies)
            {
                var pharmacyView = _mapper.Map<PharmacyViewModel>(pharmacy);
                pharmacyView.Distance = (_googleMapService.DistanceMatrixUsingAddress(userInfo?.Data?.Address, pharmacyView.Address))?.Data?.Rows.FirstOrDefault()?.Elements.FirstOrDefault()?.Distance;
                pharmaciesView.Add(pharmacyView);
            }

            var model = new DiagnoseModel()
            {
                Disease = diseaseView,
                Pharmacies = pharmaciesView.OrderBy(x => x.Distance?.Value).ToList()
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