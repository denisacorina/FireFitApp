using Microsoft.AspNetCore.Components.WebView.Maui;
using Microsoft.Maui.Devices;
using System;
using System.Net.Http;
using FireFit.Maui.Data;
using Radzen;
using FireFit.Shared.Contracts;
using FireFit.Client.Http;
using FireFit.Maui.Services;
using Microsoft.AspNetCore.Components.Authorization;
using FireFit.UI.Shared.Authentication;

namespace FireFit.Maui;

public static class MauiProgram
{
	public static MauiApp CreateMauiApp()
	{
		var builder = MauiApp.CreateBuilder();
		builder
			.UseMauiApp<App>()
			.ConfigureFonts(fonts =>
			{
				fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
			});

		builder.Services.AddMauiBlazorWebView();
#if DEBUG
		builder.Services.AddBlazorWebViewDeveloperTools();
#endif

		builder.Services.AddSingleton<WeatherForecastService>();

		// Radzen services for dialogs, notifications, tooltips, context menus
		builder.Services.AddScoped<DialogService>();
		builder.Services.AddScoped<NotificationService>();
		builder.Services.AddScoped<TooltipService>();
		builder.Services.AddScoped<ContextMenuService>();
		builder.Services.AddScoped(sp => new SharedAuthStateProvider(
			sp.GetRequiredService<IAuthService>(),
			fetchOnInit: false));
		builder.Services.AddScoped<AuthenticationStateProvider>(sp => sp.GetRequiredService<SharedAuthStateProvider>());
		builder.Services.AddAuthorizationCore();

		var apiBaseUrl = DeviceInfo.Platform == DevicePlatform.WinUI
			? "http://localhost:5074/"
			: builder.Configuration["ServerApiBaseUrl"] ?? "http://localhost:5074/";


		// API HttpClient and adapters
		builder.Services.AddHttpClient("ServerAPI", client =>
		{
			client.BaseAddress = new Uri(apiBaseUrl);
		})
		.ConfigurePrimaryHttpMessageHandler(CreatePlatformHttpMessageHandler);

		builder.Services.AddScoped<IGoalService>(sp =>
			new HttpGoalService(sp.GetRequiredService<IHttpClientFactory>().CreateClient("ServerAPI")));
		builder.Services.AddScoped<IImageRecognitionService>(sp =>
			new HttpImageRecognitionService(sp.GetRequiredService<IHttpClientFactory>().CreateClient("ServerAPI")));
		builder.Services.AddScoped<IAuthService>(sp =>
			new HttpAuthService(sp.GetRequiredService<IHttpClientFactory>().CreateClient("ServerAPI")));
		builder.Services.AddScoped<IProfileService>(sp =>
			new HttpProfileService(sp.GetRequiredService<IHttpClientFactory>().CreateClient("ServerAPI")));
		builder.Services.AddScoped<IFoodLogService>(sp =>
			new HttpFoodLogService(sp.GetRequiredService<IHttpClientFactory>().CreateClient("ServerAPI")));
		builder.Services.AddScoped<IUserProgressService>(sp =>
			new HttpUserProgressService(sp.GetRequiredService<IHttpClientFactory>().CreateClient("ServerAPI")));
		builder.Services.AddScoped<IUserPreferencesService>(sp =>
			new HttpUserPreferencesService(sp.GetRequiredService<IHttpClientFactory>().CreateClient("ServerAPI")));
		builder.Services.AddScoped<IBodyMeasurementService>(sp =>
			new HttpBodyMeasurementService(sp.GetRequiredService<IHttpClientFactory>().CreateClient("ServerAPI")));
		builder.Services.AddScoped<IWorkoutSessionService>(sp =>
			new HttpWorkoutSessionService(sp.GetRequiredService<IHttpClientFactory>().CreateClient("ServerAPI")));

		// Media service for camera/gallery capture
		builder.Services.AddScoped<IMediaService, MauiMediaService>();

		return builder.Build();
	}
	private static HttpMessageHandler CreatePlatformHttpMessageHandler()
	{
#if WINDOWS
		return new HttpClientHandler
		{
			ServerCertificateCustomValidationCallback = (message, cert, chain, errors) => true
		};
#else
		return new HttpClientHandler();
#endif
	}
}
