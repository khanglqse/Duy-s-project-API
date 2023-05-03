using AutoMapper;
using DuyProject.API.Configurations;
using DuyProject.API.Helpers;
using DuyProject.API.Models;
using DuyProject.API.Repositories;
using DuyProject.API.ViewModels;
using DuyProject.API.ViewModels.User;
using Google.Apis.Auth;
using MongoDB.Driver;
using Newtonsoft.Json;
using System.Security.Claims;

namespace DuyProject.API.Services;

public class UserService
{
    private readonly IMongoCollection<User> _users;
    private readonly TokenService _tokenService;
    private readonly MailService _mailService;
    private readonly IFileService _fileService;
    private readonly IMapper _mapper;
    private readonly IConfiguration _config;
    private static readonly HttpClient Client = new HttpClient();


    public UserService(IMongoClient client, TokenService tokenService, MailService mailService, IMapper mapper, IConfiguration config, IFileService fileService)
    {
        _tokenService = tokenService;
        _mailService = mailService;
        IMongoDatabase database = client.GetDatabase(AppSettings.DbName);
        _users = database.GetCollection<User>(nameof(User));
        _mapper = mapper;
        _config = config;
        _fileService = fileService;
    }

    public async Task<ServiceResult<PaginationResponse<UserViewModel>>> List(int page, int pageSize,
        string? filterValue, string? Role)
    {
        page = page < 1 ? 1 : page;
        pageSize = pageSize < 0 ? AppSettings.DefaultPageSize : pageSize;

        IQueryable<User> query = _users.AsQueryable().Where(t => !t.IsDeleted);

        if (!string.IsNullOrEmpty(filterValue))
        {
            string lowerValue = filterValue.ToLower();
            query = query.Where(x => x.UserName.ToLower().Contains(lowerValue) || x.Email.ToLower().Contains(lowerValue));
        }

        if (!string.IsNullOrEmpty(Role))
        {
            query = query.Where(x => x.Roles.Contains(Role));
        }

        IEnumerable<User> users = query
            .OrderByDescending(x => x.IsActive)
            .Skip((page - 1) * pageSize).Take(pageSize)
            .ToList();

        List<UserViewModel> result = users.Select(c => _mapper.Map<UserViewModel>(c)).ToList();

        int count = query.Count();
        var paginated = new PaginationResponse<UserViewModel>
        {
            Items = result,
            Page = page,
            PageSize = pageSize,
            TotalItems = count
        };
        return new ServiceResult<PaginationResponse<UserViewModel>>(paginated);
    }

    public async Task<ServiceResult<MailModel>> ResetPassword(List<string> receiver)
    {
        User? user = _users.AsQueryable().SingleOrDefault(user => user.Email == receiver.First());
        if (user is null)
        {
            return new ServiceResult<MailModel>("Invalid email address");
        }

        string newPassword = UserHelper.RandomPassword();
        var changePassword = new ChangePasswordCommand
        {
            OldPassword = user.Password,
            NewPassword = newPassword,
            ConfirmPassword = newPassword
        };

        await ChangePassword(user.Id, changePassword);

        var mailData = new MailData(receiver, "Password Reset", $"Your new password is {newPassword}. Please login using your new password");
        bool result = await _mailService.SendAsync(mailData);

        if (result)
        {
            return new ServiceResult<MailModel>(new MailModel
            {
                Receivers = receiver.First(),
                Message = "Your password have been reset"
            });
        }

        return new ServiceResult<MailModel>("Error while reset your password. Please try again later");
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
        try
        {
            result.Avatar = _fileService.ReadFileAsync(user.Id).Result.Data;
        }
        catch(Exception ex)
        {
            Console.WriteLine(ex.Message);
        }

        return Task.FromResult(new ServiceResult<UserViewModel>(result));
    }

    public async Task<ServiceResult<UserViewModel>> GetByUserNameAsync(string name)
        => await GetUserInformation(await _users.Find(user => user.UserName == name && !user.IsDeleted)
            .FirstOrDefaultAsync());

    public async Task<ServiceResult<UserViewModel>> Create(UserCreateCommand command)
    {
        if (string.IsNullOrEmpty(command.Password) || string.IsNullOrEmpty(command.UserName))
        {
            return new ServiceResult<UserViewModel>("Password and Name are required");
        }

        ServiceResult<UserViewModel> existedUser = await GetByUserNameAsync(command.UserName);
        if (existedUser.Success)
        {
            return new ServiceResult<UserViewModel>("UserName is existed");
        }

        if (UserHelper.RegexEmailCheck(command.Email))
        {
            var user = _mapper.Map<User>(command);

            user.IsActive = true;
            user.CreatedAt = DateTime.Now;
            await _users.InsertOneAsync(user);

            if (user.Roles == AppSettings.Doctor)
            {
                var receiver = new List<string> { user.Email };
                var mailData = new MailData(receiver, "Welcome to E-Heal System", $"Your user name is {user.UserName} your password is {user.Password}.");
                await _mailService.SendAsync(mailData);
            }

            return await GetById(user.Id);
        }

        return new ServiceResult<UserViewModel>("Invalid email address");
    }

    public async Task<ServiceResult<UserViewModel>> CreateSocialUser(UserCreateCommand command)
    {
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

        user.Location.Update(userIn.Location);
        user.Email = user.Email.GetValue(userIn.Email);
        user.FirstName = user.FirstName.GetValue(userIn.FirstName);
        user.LastName = user.LastName.GetValue(userIn.LastName);
        user.UserName = user.UserName.GetValue(userIn.UserName);
        user.Phone = user.Phone.GetValue(userIn.Phone);
        user.ModifiedAt = DateTime.Now;

        await _users.ReplaceOneAsync(t => t.Id == id, user);
        return await GetById(id);
    }

    public async Task<ServiceResult<UserViewModel>> ChangePassword(string id, ChangePasswordCommand passWordForm)
    {
        User user = await _users.Find(t => t.Id == id).FirstOrDefaultAsync();
        if (user == null)
        {
            return new ServiceResult<UserViewModel>("User not found");
        }

        if (!passWordForm.OldPassword.Equals(user.Password))
        {
            return new ServiceResult<UserViewModel>("You have been entered the wrong password, please try again");
        }

        if (!string.IsNullOrWhiteSpace(passWordForm.NewPassword) && passWordForm.NewPassword.Equals(passWordForm.ConfirmPassword))
        {
            user.Password = passWordForm.NewPassword;
            await _users.ReplaceOneAsync(t => t.Id == id, user);
            var receiver = new List<string> { user.Password };
            var mailData = new MailData(receiver, "Password Reset", $"Your new password is {passWordForm.NewPassword}. Please login using your new password");
            await _mailService.SendAsync(mailData);

            return await GetById(id);
        }

        return new ServiceResult<UserViewModel>("Please chose valid password form");
    }

    public async Task<ServiceResult<LoginViewModel>> Login(LoginCommand command)
    {
        User user = await _users.Find(user =>
                user.UserName == command.UserName && user.Password == command.Password && !user.IsDeleted)
            .FirstOrDefaultAsync();
        if (user == null) return new ServiceResult<LoginViewModel>("User not found");
        if (!user.IsActive) return new ServiceResult<LoginViewModel>("User is inactive");

        TokenViewModel tokenData = _tokenService.GetToken(user);

        var loginUser = _mapper.Map<LoginUserViewModel>(user);
        loginUser.Avatar = _fileService.ReadFileAsync(user.Id).Result.Data;

        return new ServiceResult<LoginViewModel>(new LoginViewModel
        {
            Token = tokenData.Token,
            Expires = tokenData.Expires,
            User = loginUser
        });
    }

    public async Task<ServiceResult<LoginViewModel>> LoginWithGoogle(GoogleLoginCommand socialLoginCommand)
    {
        var settings = new GoogleJsonWebSignature.ValidationSettings();

        settings.Audience = new List<string> { _config.GetSection("GoogleKey").Value };

        GoogleJsonWebSignature.Payload payload = GoogleJsonWebSignature.ValidateAsync(socialLoginCommand.IdToken, settings).Result;

        User user = await _users.Find(user => user.UserName == payload.Email).FirstOrDefaultAsync();

        if (user is null)
        {
            var newUser = new UserCreateCommand
            {
                UserName = payload.Email,
                Locale = payload.Locale,
                Email = payload.Email,
                IsCreateBySocialAccount = true
            };

            await CreateSocialUser(newUser);

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

        User user = await _users.Find(user => user.UserName == userInfo.Email).FirstOrDefaultAsync();

        if (user is null)
        {
            var newUser = new UserCreateCommand
            {
                UserName = userInfo.Email,
                Email = userInfo.Email
            };

            await CreateSocialUser(newUser);

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

    public async Task<ServiceResult<LoginViewModel>> RefreshToken(RefreshTokenCommand command)
    {
        ClaimsPrincipal? principals = _tokenService.GetPrincipalFromExpiredToken(command.Token);
        if (principals is null)
        {
            return new ServiceResult<LoginViewModel>("Please login first to continue");
        }

        Claim userClaim = principals.FindFirst(ClaimTypes.Name);
        if (userClaim == null) return new ServiceResult<LoginViewModel>("Token is invalid");

        string userName = userClaim.Value;
        User user = await _users.Find(user => user.UserName == userName).FirstOrDefaultAsync();
        if (user == null) return new ServiceResult<LoginViewModel>("User not found");
        if (!user.IsActive) return new ServiceResult<LoginViewModel>("User is inactive");

        TokenViewModel tokenData = _tokenService.GetToken(user, true);

        return new ServiceResult<LoginViewModel>(new LoginViewModel
        {
            Token = tokenData.Token,
            Expires = tokenData.Expires,
            User = _mapper.Map<LoginUserViewModel>(user)
        });
    }

    public async Task<ServiceResult<UserViewModel>> ToggleActive(string id)
    {
        User? user = await _users.Find(user => user.Id == id).FirstOrDefaultAsync();
        if (user == null) return new ServiceResult<UserViewModel>("User was not found");
        user.IsActive = !user.IsActive;
        await _users.ReplaceOneAsync(t => t.Id == id, user);
        return new ServiceResult<UserViewModel>(_mapper.Map<UserViewModel>(user));
    }
}