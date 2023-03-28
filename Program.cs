using DuyProject.API.Configurations;
using DuyProject.API.Endpoints;
using DuyProject.API.Hubs;
using DuyProject.API.Repositories;
using DuyProject.API.Services;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using MongoDB.Driver;
using System.Text;
using BackgroundService = DuyProject.API.Services.BackgroundService;

WebApplicationBuilder? builder = WebApplication.CreateBuilder(args);

//Db register
builder.Services.AddSingleton<IMongoClient>(o =>
    new MongoClient(builder.Configuration.GetConnectionString("MongoDb")));

//Service register
builder.Services.RegisterSwaggerServices();
builder.Services.RegisterMapperServices();
builder.Services.AddSingleton<UserService>();
builder.Services.AddSingleton<TokenService>();
builder.Services.AddSingleton<MailService>();
builder.Services.AddSingleton<DiseaseService>();
builder.Services.AddSingleton<DrugService>();
builder.Services.AddSingleton<CauseService>();
builder.Services.AddSingleton<PharmacyService>();
builder.Services.AddSingleton<LogoService>();
builder.Services.AddSingleton<BackgroundService>();
builder.Services.AddSingleton<IFileService, FileService>();
builder.Services.AddSingleton<IChatService, ChatService>();
builder.Services.AddSingleton<IConnectionManager, ConnectionManager>();

builder.Services.AddHttpContextAccessor();
builder.Services
    .Configure<ApplicationSettings>(builder.Configuration)
    .AddSingleton(sp => sp.GetRequiredService<IOptions<ApplicationSettings>>().Value);
builder.Services.Configure<MailSettings>(builder.Configuration.GetSection(nameof(MailSettings)));
builder.Services.AddCors();
builder.Services.AddAuthorization();
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateActor = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = false,
        ValidIssuer = builder.Configuration["JWT:Issuer"],
        ValidAudience = builder.Configuration["JWT:Audience"],
        IssuerSigningKey =
            new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["JWT:SigningKey"])),
        ClockSkew = TimeSpan.Zero
    };
});
builder.Services.AddFluentValidation(v => v.RegisterValidatorsFromAssemblyContaining<Program>());
builder.Services.AddSwaggerGen(s =>
{
    s.TagActionsBy(api => { return new[] { api.GroupName }; });
    s.DocInclusionPredicate((name, api) => true);
});
builder.Services.Configure<FormOptions>(o =>
{
    o.ValueLengthLimit = int.MaxValue;
    o.MultipartBodyLengthLimit = int.MaxValue;
    o.MemoryBufferThreshold = int.MaxValue;
});
builder.Services.AddSignalR();

WebApplication? app = builder.Build();

//Middleware
app.UseSwagger();
app.UseSwaggerUI();
app.UseHttpsRedirection();
app.UseCors(t => t.AllowAnyHeader().AllowAnyOrigin().AllowAnyMethod());
app.UseAuthorization();
app.UseAuthentication();
app.UseDeveloperExceptionPage();
app.MapHub<ChatHub>("/hub").RequireCors(t => t.AllowAnyHeader().AllowAnyMethod().AllowAnyOrigin());

// Endpoint register 
UserEndpoint.Map(app);
CauseEndpoint.Map(app);
DiseaseEndpoint.Map(app);
DrugEndpoint.Map(app);
PharmacyEndpoint.Map(app);
LogoEndpoint.Map(app);
FileEndpoint.Map(app);

//await DataInit.InitializeData(app);

using (IServiceScope? scope = app.Services.CreateScope())
{
    var backgroundService = scope.ServiceProvider.GetRequiredService<BackgroundService>();
    backgroundService?.RunBackgroundJob();
}

app.Run();