using System.Text.RegularExpressions;
using AutoMapper;
using DuyProject.API.Configurations;
using DuyProject.API.Models;
using DuyProject.API.ViewModels;
using DuyProject.API.ViewModels.Disease;
using DuyProject.API.ViewModels.Pharmacy;
using MongoDB.Bson;
using MongoDB.Driver;

namespace DuyProject.API.Services;

public class DiseaseService
{
    private readonly IMongoCollection<Disease> _diseaseCollection;
    private readonly IMongoCollection<Drug> _drugCollection;
    private readonly IMongoCollection<Pharmacy> _pharmacyCollection;
    private readonly IMongoCollection<Symptom> _causeCollection;
    private readonly IMapper _mapper;
    private readonly GoogleMapService _googleMapService;
    private readonly UserService _userService;

    public DiseaseService(IMongoClient client, IMapper mapper, GoogleMapService googleMapService, UserService userService)
    {
        IMongoDatabase? database = client.GetDatabase(AppSettings.DbName);
        _diseaseCollection = database.GetCollection<Disease>(nameof(Disease));
        _causeCollection = database.GetCollection<Symptom>(nameof(Symptom));
        _drugCollection = database.GetCollection<Drug>(nameof(Drug));
        _pharmacyCollection = database.GetCollection<Pharmacy>(nameof(Pharmacy));
        _mapper = mapper;
        _googleMapService = googleMapService;
        _userService = userService;
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
            diseaseView.Symptoms = _causeCollection.AsQueryable().Where(u => diseaseView.SymptomsIds.Contains(u.Id)).ToList();
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
        data.Symptoms = _causeCollection.AsQueryable().Where(u => data.SymptomsIds.Contains(u.Id)).ToList();
        data.Drugs = _drugCollection.AsQueryable().Where(u => data.DrugIds.Contains(u.Id)).ToList();
        return new ServiceResult<DiseaseViewModel>(data);
    }

    public async Task<ServiceResult<DiagnoseModel>> GetDiagnoseInfo(string id, string? userId = null, string? address = null)
    {
        var disease = await Get(id);
        if (!disease.Success) 
        {
            return new ServiceResult<DiagnoseModel>("Disease was not found.");
        }

        var data = new DiagnoseModel(disease.Data ?? new DiseaseViewModel());
        var isAddress = false;
        var addressInfo = string.Empty;

        if (!string.IsNullOrEmpty(userId))
        {
            isAddress = true;
            addressInfo = (await _userService.GetById(userId))?.Data?.Address;
        }
        else if (!string.IsNullOrWhiteSpace(address))
        {
            isAddress = true;
            addressInfo = address;
        }

        var pharmaciesView = new List<PharmacyViewModel>();
            var pharmacyFilter = Builders<Pharmacy>.Filter
                .Where(pharmacy => !pharmacy.IsDeleted
                    && pharmacy.Address.Address != null
                    && pharmacy.DrugIds
                    .Any(drugId => data.Disease.DrugIds.Contains(drugId)));

        var pharmacies = _pharmacyCollection.Find(pharmacyFilter).ToList();

        foreach (var pharmacy in pharmacies)
        {
            var pharmacyView = _mapper.Map<PharmacyViewModel>(pharmacy);
            if (isAddress)
            {
                pharmacyView.Distance = (_googleMapService.DistanceMatrixUsingAddress(addressInfo, pharmacyView.Address))?.Data?.Rows.FirstOrDefault()?.Elements.FirstOrDefault()?.Distance;
            }
            pharmaciesView.Add(pharmacyView);
        }

        data.Pharmacies = pharmaciesView;
        return new ServiceResult<DiagnoseModel>(data);
    }

    public async Task<ServiceResult<DiseaseViewModel>> Create(DiseaseCreateCommand command)
    {
        Disease? entity = _mapper.Map<DiseaseCreateCommand, Disease>(command);

        bool isSymptomsExisted = SymptomsVerify(entity);

        if (!isSymptomsExisted)
        {
            return new ServiceResult<DiseaseViewModel>("Invalid symptoms.");
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
        entity.SymptomsIds = entity.SymptomsIds.Union(command.SymptomsIds).ToList();
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

    public async Task<ServiceResult<PaginationResponse<DiseaseModel>>> Diagnosis(DiagnoseRequestModel request) 
    {
        var causeFilter = Builders<Symptom>.Filter.Where(symptoms => !symptoms.IsDeleted);
        if (request.Symptoms != null)
        {
            var regexFilter = "(" + string.Join("|", request.Symptoms) + ")";
            var expReg = new BsonRegularExpression(regexFilter, "i");
            causeFilter &= Builders<Symptom>.Filter.Regex(x => x.Name, expReg);
        }

        var causeIds = _causeCollection.Find(causeFilter).ToList().Select(symptoms => symptoms.Id);
        IQueryable<Disease> diseaseQuery = _diseaseCollection.AsQueryable().Where(disease => !disease.IsDeleted
            && disease.SymptomsIds.Any(causeId => causeIds.Contains(causeId)));

        List<Disease> items = diseaseQuery
            .OrderBy(x => x.IsActive).ThenBy(x => x.CreatedAt)
            .Skip((request.PageNumber - 1) * request.PageSize).Take(request.PageSize)
            .ToList();

        int count = diseaseQuery.Count();
        var paginated = new PaginationResponse<DiseaseModel>
        {
            Items = items.Select(c => _mapper.Map<DiseaseModel>(c)).ToList(),
            Page = request.PageNumber,
            PageSize = request.PageSize,
            TotalItems = count
        };
        return new ServiceResult<PaginationResponse<DiseaseModel>>(paginated);
    }

    private bool SymptomsVerify(Disease entity)
    {
        return entity.SymptomsIds.Select(causeId => _causeCollection.AsQueryable().Any(x => x.Id == causeId)).Any(isSymptomsIdValid => isSymptomsIdValid);
    }
}