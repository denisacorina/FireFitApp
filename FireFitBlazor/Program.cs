using FireFitBlazor.Components;
using FireFitBlazor.Domain.Models;
using FireFitBlazor.Domain.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication;
using System.Security.Claims;
using Radzen;
using Radzen.Blazor;

using FireFitBlazor.Domain.ContextInterfaces;
using Application.Services;
using FireFitBlazor.Infrastructure.GatewayInterfaces;
using FireFitBlazor.Domain.Contexts;
using FireFitBlazor.Domain.Interfaces;
using FireFitBlazor.Infrastructure.Contexts;
using static FireFitBlazor.Domain.Enums.FoodTrackingEnums;
using FireFitBlazor.Infrastructure.Gateways;
using FireFitBlazor.Application.Services;
using FireFitBlazor.Infrastructure.Contexts.ProgressContexts;
using FireFitBlazor.Domain.ContextInterfaces.ProgressContexts;
using FireFitBlazor.Infrastructure.Contexts.GoalContexts;
using FireFitBlazor.Domain.ContextInterfaces.GoalContexts;
using FireFitBlazor.Infrastructure.Data;
using Microsoft.ML.Data;
using FoodObjectDetection;
using FireFitBlazor;
using Microsoft.AspNetCore.Components.Authorization;
using static FireFitBlazor.Application.FoodLogEntryChoice;
using IntentClassification;
using RecipeRecommendation;
using FireFitBlazor.Domain.Contexts.ProgressContexts;
using BlazorBootstrap;
using static BioTaggedSentence;
using NETCore.MailKit.Core;
using FireFitBlazor.Domain.ContextInterfaces.UserContexts;
using NETCore.MailKit;
using NETCore.MailKit.Extensions;
using Radzen.Blazor.Markdown;
using NETCore.MailKit.Infrastructure.Internal;
using FireFit.UI.Shared.Authentication;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

builder.Services.AddRazorPages();
builder.Services.AddServerSideBlazor();
// Add Radzen services
builder.Services.AddScoped<DialogService>();
builder.Services.AddScoped<NotificationService>();
builder.Services.AddScoped<TooltipService>();
builder.Services.AddScoped<ContextMenuService>();

// Add DbContext with provider switching (SqlServer/Sqlite/PostgreSQL)
builder.Services.AddDbContext<ApplicationDbContext>(options =>
{
    var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
    var provider = builder.Configuration["DatabaseProvider"]; // Optional override: SqlServer|Sqlite|PostgreSQL

    // Infer provider if not explicitly specified
    string inferred = provider?.Trim();
    if (string.IsNullOrWhiteSpace(inferred))
    {
        if (!string.IsNullOrWhiteSpace(connectionString))
        {
            var cs = connectionString;
            if (cs.Contains("Host=", StringComparison.OrdinalIgnoreCase) ||
                cs.Contains("Username=", StringComparison.OrdinalIgnoreCase) ||
                cs.Contains("User ID=", StringComparison.OrdinalIgnoreCase))
            {
                inferred = "PostgreSQL";
            }
            else if (cs.Contains("Data Source=", StringComparison.OrdinalIgnoreCase) ||
                     cs.EndsWith(".db", StringComparison.OrdinalIgnoreCase) ||
                     cs.Contains("Filename=", StringComparison.OrdinalIgnoreCase))
            {
                inferred = "Sqlite";
            }
            else
            {
                inferred = "SqlServer";
            }
        }
        else
        {
            inferred = "SqlServer";
        }
    }

    switch (inferred?.ToLowerInvariant())
    {
        case "sqlite":
            options.UseSqlite(connectionString);
            break;
        case "postgresql":
        case "npgsql":
            options.UseNpgsql(connectionString);
            break;
        default:
            options.UseSqlServer(connectionString);
            break;
    }
});

// Add Identity services

// Add authentication services
//builder.Services.AddAuthentication(options =>
//{
//    options.DefaultScheme = IdentityConstants.ApplicationScheme;
//    options.DefaultSignInScheme = IdentityConstants.ExternalScheme;
//})
//.AddIdentityCookies();
//.AddGoogle(options =>
//{
//    options.ClientId = builder.Configuration["Authentication:Google:ClientId"];
//    options.ClientSecret = builder.Configuration["Authentication:Google:ClientSecret"];
//});

// Add authorization with policies
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("RequireAdminRole", policy =>
        policy.RequireRole("Admin"));

    options.AddPolicy("RequireUserRole", policy =>
        policy.RequireRole("User"));

});

builder.Services.AddScoped<ICurrentUserService, CurrentUserService>();
builder.Services.AddScoped<SharedAuthStateProvider>(sp =>
    new SharedAuthStateProvider(sp.GetRequiredService<FireFit.Shared.Contracts.IAuthService>()));
builder.Services.AddScoped<AuthenticationStateProvider>(sp =>
    sp.GetRequiredService<SharedAuthStateProvider>());
// Add application services
builder.Services.AddScoped<IRecipeGateway, RecipeGateway>();
builder.Services.AddScoped<IGoalContext, GoalContext>();
builder.Services.AddScoped<IUserProgressContext, UserProgressContext>();
builder.Services.AddScoped<IUpdateUserContext, UpdateUserContext>();
builder.Services.AddScoped<IBodyMeasurementGateway, BodyMeasurementGateway>();
builder.Services.AddScoped<IBodyMeasurementContext, BodyMeasurementContext>();
builder.Services.AddScoped<IUpdateUserProgressContext, UpdateUserProgressContext>();
builder.Services.AddScoped<IAddBodyMeasurementContext, AddBodyMeasurementContext>();
builder.Services.AddScoped<IDeleteBodyMeasurementContext, DeleteBodyMeasurementContext>();
builder.Services.AddScoped<IGetBodyMeasurementsContext, GetBodyMeasurementsContext>();
builder.Services.AddScoped<IGetLatestBodyMeasurementContext, GetLatestBodyMeasurementContext>();
builder.Services.AddScoped<IGetUserProgressContext, GetUserProgressContext>();
builder.Services.AddScoped<IGetUserContext, GetUserContext>();
builder.Services.AddScoped<IGetUserGateway, GetUserGateway>();
builder.Services.AddScoped<IUpdateUserProgressContext, UpdateUserProgressContext>();
builder.Services.AddScoped<IUserPreferencesContext, UserPreferencesContext>();
builder.Services.AddScoped<IFoodLogService, FoodLogService>();
builder.Services.AddScoped<IGoalService, GoalService>();
builder.Services.AddScoped<IGoalContext, GoalContext>();
//builder.Services.AddScoped<IGetUserGoalsContext, GetUserGoalsContext>();
//builder.Services.AddScoped<IMarkGoalAsCompletedContext, MarkGoalAsCompletedContext>();
//builder.Services.AddScoped<IReactivateGoalContext, ReactivateGoalContext>();
//builder.Services.AddScoped<IUpdateGoalContext, UpdateGoalContext>();
builder.Services.AddScoped<WeightPredictionService>();

// Email service configuration (MailKit)
builder.Services.AddMailKit(optionBuilder =>
{
    optionBuilder.UseMailKit(new MailKitOptions()
    {
        Server = builder.Configuration["EmailSettings:SmtpServer"],
        Port = Convert.ToInt32(builder.Configuration["EmailSettings:SmtpPort"] ?? "587"),
        SenderName = builder.Configuration["EmailSettings:FromName"],
        SenderEmail = builder.Configuration["EmailSettings:FromEmail"],
        Account = builder.Configuration["EmailSettings:SmtpUsername"] ?? builder.Configuration["EmailSettings:FromEmail"],
        Password = builder.Configuration["EmailSettings:SmtpPassword"],
        Security = true
    });
});
builder.Services.AddHttpContextAccessor();
builder.Services.AddControllers();
builder.Services.AddHttpClient<IBarcodeProductContext, BarcodeProductContext>();

builder.Services.AddScoped<IProgressGateway, ProgressGateway>();
builder.Services.AddScoped<IGoalGateway, GoalGateway>();
builder.Services.AddScoped<IPhotoUploadService, PhotoUploadService>();

builder.Services.AddHttpClient("ServerAPI", client =>
{
    var baseAddress = builder.Configuration["ServerAPI:BaseAddress"];
    if (!string.IsNullOrWhiteSpace(baseAddress))
    {
        client.BaseAddress = new Uri(baseAddress);
    }
});

builder.Services.AddHttpClient("MLAPI", client =>
{
    var baseAddress = builder.Configuration["MLAPI:BaseAddress"];
    if (!string.IsNullOrWhiteSpace(baseAddress))
    {
        client.BaseAddress = new Uri(baseAddress);
    }
});

builder.Services.AddScoped(sp => sp.GetRequiredService<IHttpClientFactory>().CreateClient("ServerAPI"));
builder.Services.AddBlazorBootstrap();
//ObjectDetection.Train();

// Dev-only sample ML code guarded to avoid startup failures in prod
if (builder.Environment.IsDevelopment())
{
    try
    {
        // Create single instance of sample data from first line of dataset for model input.
        var image = MLImage.CreateFromFile(@"C:\Users\DENI\Downloads\cucumber.jpg");
        ObjectDetection.ModelInput sampleData = new ObjectDetection.ModelInput()
        {
            Image = image,
        };
        // var predictionResult = ObjectDetection.Predict(sampleData);
    }
    catch { /* ignore in dev if file missing */ }
}

//MLModel1.Train();

//MLModel1.ModelInput sample = new MLModel1.ModelInput();
//sample.Text = "add tomatoes";
//var predict = MLModel1.Predict(sample);
//var predictedLabel = predict.PredictedLabel;
// Make a single prediction on the sample data and print results.
//var predictionResult = ObjectDetection.Predict(sampleData);

//var predictedLabel = predictionResult.PredictedLabel;

//Console.WriteLine("\n\nPredicted Boxes:\n");
//if (predictionResult.PredictedBoundingBoxes == null)
//{
//    Console.WriteLine("No Predicted Bounding Boxes");
//    return;
//}
//var boxes =
//    predictionResult.PredictedBoundingBoxes.Chunk(4)
//        .Select(x => new { XTop = x[0], YTop = x[1], XBottom = x[2], YBottom = x[3] })
//        .Zip(predictionResult.Score, (a, b) => new { Box = a, Score = b });

//foreach (var item in boxes)
//{
//    Console.WriteLine($"XTop: {item.Box.XTop},YTop: {item.Box.YTop},XBottom: {item.Box.XBottom},YBottom: {item.Box.YBottom}, Score: {item.Score}");
//}

builder.Services.AddScoped<IGoalGateway, GoalGateway>();
builder.Services.AddScoped<IUpdateUserGateway, UpdateUserGateway>();

// Register logging service
builder.Services.AddScoped<ILoggingService, LoggingService>();
builder.Services.AddAuthorizationCore();
builder.Services.AddSingleton<ImageTransferService>();
// Register contexts
builder.Services.AddScoped<IFoodLogContext, FoodLogContext>();
builder.Services.AddScoped<IIngredientRecognitionContext>(sp =>
{
    var httpClient = new HttpClient();
    var apiEndpoint = builder.Configuration["ImageRecognition:ApiEndpoint"];
    var loggingService = sp.GetRequiredService<ILoggingService>();
    return new IngredientRecognitionContext(httpClient, apiEndpoint, loggingService);
});

builder.Services.ConfigureApplicationCookie(options =>
{
    options.LoginPath = "/login";
    options.LogoutPath = "/logout";
    options.AccessDeniedPath = "/forbidden";
    options.Cookie.SameSite = SameSiteMode.Lax;
    options.Cookie.SecurePolicy = CookieSecurePolicy.None;
    options.Cookie.HttpOnly = true;
    options.ExpireTimeSpan = TimeSpan.FromDays(30);
});


// Configure loggingdotnet dev-certs https --trust
builder.Logging.AddConsole();
builder.Logging.AddDebug();
builder.Logging.SetMinimumLevel(LogLevel.Information);



//if (!File.Exists("\\vocab.json") || !File.Exists("\\ner_model.index")) 
//{
//    Console.WriteLine("Modelul NER sau vocabularul nu există. Se va antrena modelul...");
//    RecipeRecommendationGen.TrainModel();
//}
builder.Services.AddScoped<RecipeRecommendation.NERPredictor>(sp =>
    new RecipeRecommendation.NERPredictor("./ner_model", "./vocab.json")
);

builder.Services.AddScoped<RecipePredictor>(provider =>
    new RecipePredictor("bio_ner_model3.zip"));

builder.Services.AddScoped<IntentClassification.MLModel1>();

builder.Services.AddScoped<RecipeChatService>();
builder.Services.AddScoped<RecipeGeneratorService>();
builder.Services.AddScoped<IWorkoutService, WorkoutService>();
builder.Services.AddScoped<IAnalyticsService, AnalyticsService>();

builder.Services.AddHttpClient<OpenFoodFactsService>();

// Register shared abstractions
builder.Services.AddScoped<FireFit.Shared.Email.IEmailSender, FireFitBlazor.Infrastructure.Email.EmailSenderAdapter>();

// Shared contract adapters for reuse with MAUI client
builder.Services.AddScoped(typeof(FireFit.Shared.Contracts.IAuthService), typeof(FireFitBlazor.Infrastructure.Adapters.AuthServiceAdapter));
builder.Services.AddScoped(typeof(FireFit.Shared.Contracts.IProfileService), typeof(FireFitBlazor.Infrastructure.Adapters.ProfileServiceAdapter));
builder.Services.AddScoped(typeof(FireFit.Shared.Contracts.IFoodLogService), typeof(FireFitBlazor.Infrastructure.Adapters.FoodLogServiceAdapter));
builder.Services.AddScoped(typeof(FireFit.Shared.Contracts.IGoalService), typeof(FireFitBlazor.Infrastructure.Adapters.GoalServiceAdapter));
builder.Services.AddScoped(typeof(FireFit.Shared.Contracts.IUserProgressService), typeof(FireFitBlazor.Infrastructure.Adapters.UserProgressServiceAdapter));
builder.Services.AddScoped(typeof(FireFit.Shared.Contracts.IBodyMeasurementService), typeof(FireFitBlazor.Infrastructure.Adapters.BodyMeasurementServiceAdapter));
builder.Services.AddScoped(typeof(FireFit.Shared.Contracts.IUserPreferencesService), typeof(FireFitBlazor.Infrastructure.Adapters.UserPreferencesServiceAdapter));
builder.Services.AddScoped(typeof(FireFit.Shared.Contracts.IImageRecognitionService), typeof(FireFitBlazor.Infrastructure.Adapters.ImageRecognitionServiceAdapter));
builder.Services.AddScoped(typeof(FireFit.Shared.Contracts.IWorkoutSessionService), typeof(FireFitBlazor.Infrastructure.Adapters.WorkoutSessionServiceAdapter));

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage(); // <--- Add this

    // Dev-only endpoint to inspect email config (sanitized)
    app.MapGet("/api/debug/email-config", (IConfiguration cfg) => Results.Json(new
    {
        Server = cfg["EmailSettings:SmtpServer"],
        Port = cfg["EmailSettings:SmtpPort"],
        FromEmail = cfg["EmailSettings:FromEmail"],
        FromName = cfg["EmailSettings:FromName"],
        Username = cfg["EmailSettings:SmtpUsername"],
        HasPassword = !string.IsNullOrWhiteSpace(cfg["EmailSettings:SmtpPassword"]) // do not return the password
    }));
}

//app.UseHttpsRedirection();
app.UseStaticFiles();

// Add authentication and authorization middleware
app.UseAuthentication();
app.UseAuthorization();

app.UseAntiforgery();

app.MapControllers();
app.MapStaticAssets();

app.MapRazorComponents<App>().AddInteractiveServerRenderMode();


//// Ensure database is created and migrations are applied
// Apply EF Core migrations automatically at startup (safe for dev/small setups)
try
{
    using var scope = app.Services.CreateScope();
    var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    db.Database.Migrate();
}
catch (Exception ex)
{
    app.Logger.LogError(ex, "Error applying database migrations at startup");
}
app.Run();

