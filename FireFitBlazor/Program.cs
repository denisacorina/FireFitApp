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

// Add DbContext
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

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
builder.Services.AddScoped<AuthenticationStateProvider, CustomAuthStateProvider>();
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

//builder.Services.Configure<EmailSettings>(builder.Configuration.GetSection("EmailSettings"));
//builder.Services.AddScoped<IEmailService, EmailService>();
builder.Services.AddHttpContextAccessor();
builder.Services.AddControllers();
builder.Services.AddHttpClient<IBarcodeProductContext, BarcodeProductContext>();

builder.Services.AddScoped<IProgressGateway, ProgressGateway>();
builder.Services.AddScoped<IGoalGateway, GoalGateway>();
builder.Services.AddScoped<IPhotoUploadService, PhotoUploadService>();

builder.Services.AddHttpClient("ServerAPI", client =>
{
   /* client.BaseAddress = new Uri("https://localhost:7128/");*/ // Use your actual dev URL
    client.BaseAddress = new Uri("http://192.168.100.87:5000/"); // Use your actual dev URL
});

builder.Services.AddHttpClient("MLAPI", client =>
{
    client.BaseAddress = new Uri("http://192.168.0.123:6000/");
});

builder.Services.AddScoped(sp => sp.GetRequiredService<IHttpClientFactory>().CreateClient("ServerAPI"));
builder.Services.AddBlazorBootstrap();
//ObjectDetection.Train();

// Create single instance of sample data from first line of dataset for model input.
var image = MLImage.CreateFromFile(@"C:\Users\DENI\Downloads\cucumber.jpg");
ObjectDetection.ModelInput sampleData = new ObjectDetection.ModelInput()
{
    Image = image,
};

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
    options.Cookie.SameSite = SameSiteMode.Lax; // ✅ required for mobile + cross-tab
    options.Cookie.SecurePolicy = CookieSecurePolicy.None; // ✅ required when SameSite=None
    options.Cookie.HttpOnly = true;
    options.ExpireTimeSpan = TimeSpan.FromDays(7);
});


// Configure loggingdotnet dev-certs https --trust
builder.Logging.AddConsole();
builder.Logging.AddDebug();
builder.Logging.SetMinimumLevel(LogLevel.Information);
builder.Services.AddScoped<RecipeRecommendation.NERPredictor>(sp =>
    new RecipeRecommendation.NERPredictor("./ner_model", "./vocab.json")
);

builder.Services.AddScoped<IntentClassification.MLModel1>();
builder.Services.AddScoped<RecipeGeneratorService>();
builder.Services.AddScoped<IWorkoutService, WorkoutService>();
builder.Services.AddScoped<IAnalyticsService, AnalyticsService>();

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


app.Run();
