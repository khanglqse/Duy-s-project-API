using DuyProject.API.Configurations;
using DuyProject.API.Endpoints;
using DuyProject.API.Services;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using MongoDB.Driver;
using System.Text;
using BackgroundService = DuyProject.API.Services.BackgroundService;

{
    WebApplicationBuilder? builder = WebApplication.CreateBuilder(args);

    //Db register
    builder.Services.AddSingleton<IMongoClient>(o =>
        new MongoClient(builder.Configuration.GetConnectionString("MongoDb")));

    //Service register
    builder.Services.RegisterSwaggerServices();
    builder.Services.RegisterMapperServices();
    builder.Services.AddSingleton<UserService>();
    builder.Services.AddSingleton<TokenService>();
    builder.Services.AddSingleton<DiseaseService>();
    builder.Services.AddSingleton<DrugService>();
    builder.Services.AddSingleton<CauseService>();
    builder.Services.AddSingleton<PharmacyService>();
    builder.Services.AddSingleton<BackgroundService>();


    builder.Services.AddHttpContextAccessor();
    builder.Services
        .Configure<ApplicationSettings>(builder.Configuration)
        .AddSingleton(sp => sp.GetRequiredService<IOptions<ApplicationSettings>>().Value);
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
        s.TagActionsBy(api =>
        {
            return new[] { api.GroupName };
        });
        s.DocInclusionPredicate((name, api) => true);
    });

    WebApplication? app = builder.Build();

    //Middleware
    app.UseSwagger();
    app.UseSwaggerUI();
    app.UseHttpsRedirection();
    app.UseCors(t => t.AllowAnyHeader().AllowAnyOrigin().AllowAnyMethod());
    app.UseAuthorization();
    app.UseAuthentication();
    app.UseDeveloperExceptionPage();

    // Endpoint register 
    UserEndpoint.Map(app);
    CauseEndpoint.Map(app);
    DiseaseEndpoint.Map(app);
    DrugEndpoint.Map(app);
    PharmacyEndpoint.Map(app);

    await DataInit.InitializeData(app);

    using (IServiceScope? scope = app.Services.CreateScope())
    {
        var backgroundService = scope.ServiceProvider.GetRequiredService<BackgroundService>();
        backgroundService?.RunBackgroundJob();
    }

    app.Run();
}