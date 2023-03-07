using AutoMapper;
using DuyProject.API.Configurations;
using DuyProject.API.Models;
using DuyProject.API.ViewModels;
using DuyProject.API.ViewModels.User;
using Google.Apis.Auth;
using MongoDB.Driver;
using Newtonsoft.Json;
using System.Linq.Expressions;
using System.Security.Claims;

namespace DuyProject.API.Services;

public class UserService
{
    private readonly IMongoCollection<User> _users;
    private readonly TokenService _tokenService;
    private readonly IMapper _mapper;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly IConfiguration _config;
    private static readonly HttpClient Client = new HttpClient();


    public UserService(IMongoClient client, TokenService tokenService, IMapper mapper,
        IHttpContextAccessor httpContextAccessor, IConfiguration config)
    {
        _tokenService = tokenService;
        IMongoDatabase database = client.GetDatabase(AppSettings.DbName);
        _users = database.GetCollection<User>(nameof(User));
        _mapper = mapper;
        _httpContextAccessor = httpContextAccessor;
        _config = config;
    }

    public async Task<ServiceResult<PaginationResponse<UserViewModel>>> List(int page, int pageSize,
        string? filterValue)
    {
        page = page < 1 ? 1 : page;
        pageSize = pageSize < 0 ? AppSettings.DefaultPageSize : pageSize;
        Expression<Func<User, bool>>? filter = x => !x.IsDeleted;

        IQueryable<User>? query = _users.AsQueryable().Where(t => !t.IsDeleted);

        if (!string.IsNullOrEmpty(filterValue))
        {
            string lowerValue = filterValue.ToLower();
            query = query.Where(x => x.Name.ToLower().Contains(lowerValue) || x.Email.ToLower().Contains(lowerValue));
        }

        IEnumerable<UserViewModel> users = query
            .OrderByDescending(x => x.IsActive)
            .Skip((page - 1) * pageSize).Take(pageSize)
            .Select(c => _mapper.Map<UserViewModel>(c));

        int count = query.Count();
        var paginated = new PaginationResponse<UserViewModel>
        {
            Items = users,
            Page = page,
            PageSize = pageSize,
            TotalItems = count
        };
        return new ServiceResult<PaginationResponse<UserViewModel>>(paginated);
    }


    public async Task<ServiceResult<UserViewModel>> GetById(string id)
    {
        User user = await _users.Find(user => user.Id == id && !user.IsDeleted).FirstOrDefaultAsync();

        return await GetUserInformation(user);
    }

    public Task<ServiceResult<UserViewModel>> GetUserInformation(User? user)
    {
        if (user is null)
        {
            return Task.FromResult(new ServiceResult<UserViewModel>("User not found"));
        }

        var result = _mapper.Map<UserViewModel>(user);

        return Task.FromResult(new ServiceResult<UserViewModel>(result));
    }

    public async Task<ServiceResult<UserViewModel>> GetByUserNameAsync(string name)
        => await GetUserInformation(await _users.Find(user => user.Name == name && !user.IsDeleted)
            .FirstOrDefaultAsync());

    public async Task<ServiceResult<UserViewModel>> Create(UserCreateCommand command)
    {
        if (string.IsNullOrEmpty(command.Password) || string.IsNullOrEmpty(command.Name))
        {
            return new ServiceResult<UserViewModel>("Password and Name are required");
        }

        ServiceResult<UserViewModel> existedUser = await GetByUserNameAsync(command.Name);
        if (existedUser.Success)
        {
            return new ServiceResult<UserViewModel>("UserName is existed");
        }

        var user = _mapper.Map<User>(command);

        user.IsActive = true;
        user.CreatedAt = DateTime.UtcNow;
        await _users.InsertOneAsync(user);

        return await GetById(user.Id);
    }

    public async Task<ServiceResult<UserViewModel>> Update(string id, UserUpdateCommand userIn)
    {
        User user = await _users.Find(t => t.Id == id).FirstOrDefaultAsync();
        if (user == null)
        {
            return new ServiceResult<UserViewModel>("User not found");
        }
        user.Name = userIn.Name;
        await _users.ReplaceOneAsync(t => t.Id == id, user);
        return await GetById(id);
    }


    public async Task<ServiceResult<object>> Remove(string id)
    {
        string userId = _httpContextAccessor.HttpContext?.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        if (id == userId)
            return new ServiceResult<object>("Unable to delete yourself");

        User user = await _users.Find(user => user.Id == id).FirstOrDefaultAsync();
        if (user == null) return new ServiceResult<object>("User not found");
        user.IsDeleted = true;
        await _users.ReplaceOneAsync(t => t.Id == id, user);
        return new ServiceResult<object>(new { isDeleted = user.IsDeleted });
    }

    public async Task<ServiceResult<LoginViewModel>> Login(LoginCommand command)
    {
        User user = await _users.Find(user =>
                user.Name == command.UserName && user.Password == command.Password && !user.IsDeleted)
            .FirstOrDefaultAsync();
        if (user == null) return new ServiceResult<LoginViewModel>("User not found");
        if (!user.IsActive) return new ServiceResult<LoginViewModel>("User is inactive");

        TokenViewModel tokenData = _tokenService.GetToken(user);

        return new ServiceResult<LoginViewModel>(new LoginViewModel
        {
            Token = tokenData.Token,
            Expires = tokenData.Expires,
            User = _mapper.Map<LoginUserViewModel>(user)
        });
    }

    public async Task<ServiceResult<LoginViewModel>> LoginWithGoogle(GoogleLoginCommand socialLoginCommand)
    {
        var settings = new GoogleJsonWebSignature.ValidationSettings();

        settings.Audience = new List<string> { _config.GetSection("GoogleKey").Value };

        GoogleJsonWebSignature.Payload payload = GoogleJsonWebSignature.ValidateAsync(socialLoginCommand.IdToken, settings).Result;

        User user = await _users.Find(user => user.Name == payload.Email).FirstOrDefaultAsync();

        if (user is null)
        {
            var newUser = new UserCreateCommand
            {
                Name = payload.Email,
                FirstName = payload.GivenName,
                LastName = payload.FamilyName,
                Email = payload.Email
            };

            await Create(newUser);

            user = _mapper.Map<User>(newUser);

            TokenViewModel tokenData = _tokenService.GetToken(user);

            return new ServiceResult<LoginViewModel>(new LoginViewModel
            {
                Token = tokenData.Token,
                Expires = tokenData.Expires,
                User = _mapper.Map<LoginUserViewModel>(user)
            });
        }
        else
        {
            TokenViewModel tokenData = _tokenService.GetToken(_mapper.Map<User>(user));

            return new ServiceResult<LoginViewModel>(new LoginViewModel
            {
                Token = tokenData.Token,
                Expires = tokenData.Expires,
                User = _mapper.Map<LoginUserViewModel>(user)
            });
        }
    }

    public async Task<ServiceResult<LoginViewModel>> LoginWithFacebook(FacebookLoginCommand facebookLoginCommand)
    {
        string appAccessTokenResponse = await Client.GetStringAsync(_config.GetValue<string>("ClientUrl"));
        var appAccessToken = JsonConvert.DeserializeObject<FacebookAppAccessToken>(appAccessTokenResponse);

        string userAccessTokenValidationResponse = await Client.GetStringAsync($"https://graph.facebook.com/debug_token?input_token={facebookLoginCommand.AccessToken}&access_token={appAccessToken.AccessToken}");
        var userAccessTokenValidation = JsonConvert.DeserializeObject<FacebookUserAccessTokenValidation>(userAccessTokenValidationResponse);

        if (!userAccessTokenValidation.Data.IsValid)
        {
            return new ServiceResult<LoginViewModel>("User not found");
        }

        string userInfoResponse = await Client.GetStringAsync($"https://graph.facebook.com/v2.8/me?fields=id,email,first_name,last_name,name,gender,locale,birthday&access_token={facebookLoginCommand.AccessToken}");
        var userInfo = JsonConvert.DeserializeObject<FacebookUserData>(userInfoResponse);

        User user = await _users.Find(user => user.Name == userInfo.Email).FirstOrDefaultAsync();

        if (user is null)
        {
            var newUser = new UserCreateCommand
            {
                Name = userInfo.Email,
                FirstName = userInfo.FirstName,
                LastName = userInfo.LastName
            };

            await Create(newUser);

            TokenViewModel tokenData = _tokenService.GetToken(_mapper.Map<User>(newUser));

            return new ServiceResult<LoginViewModel>(new LoginViewModel
            {
                Token = tokenData.Token,
                Expires = tokenData.Expires,
                User = _mapper.Map<LoginUserViewModel>(user)
            });
        }
        else
        {
            TokenViewModel tokenData = _tokenService.GetToken(_mapper.Map<User>(user));

            return new ServiceResult<LoginViewModel>(new LoginViewModel
            {
                Token = tokenData.Token,
                Expires = tokenData.Expires,
                User = _mapper.Map<LoginUserViewModel>(user)
            });
        }
    }

    public async Task<ServiceResult<LoginViewModel>> RefreshToken(RefreshTokenCommand command)
    {
        ClaimsPrincipal? principals = _tokenService.GetPrincipalFromExpiredToken(command.Token);
        if (principals == null)
        {
            return new ServiceResult<LoginViewModel>("Please login first to continue");
        }

        Claim userClaim = principals.FindFirst(ClaimTypes.Name);
        if (userClaim == null) return new ServiceResult<LoginViewModel>("Token is invalid");

        string userName = userClaim.Value;
        User user = await _users.Find(user => user.Name == userName).FirstOrDefaultAsync();
        if (user == null) return new ServiceResult<LoginViewModel>("User not found");
        if (!user.IsActive) return new ServiceResult<LoginViewModel>("User is inactive");

        TokenViewModel? tokenData = _tokenService.GetToken(user, true);

        return new ServiceResult<LoginViewModel>(new LoginViewModel
        {
            Token = tokenData.Token,
            Expires = tokenData.Expires,
            User = _mapper.Map<LoginUserViewModel>(user)
        });
    }

    public async Task<ServiceResult<object>> ToggleActive(string id)
    {
        User? user = await _users.Find(user => user.Id == id).FirstOrDefaultAsync();
        if (user == null) return new ServiceResult<object>("User was not found");
        user.IsActive = !user.IsActive;
        await _users.ReplaceOneAsync(t => t.Id == id, user);
        return new ServiceResult<object>(new { isActive = user.IsActive });
    }

}