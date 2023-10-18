using Microsoft.EntityFrameworkCore;
using System.Reflection;
// https://learn.microsoft.com/en-us/dotnet/azure/sdk/authentication/?tabs=command-line
using Azure.Identity;
using Microsoft.Extensions.Azure;
using Microsoft.Extensions.Configuration.AzureAppConfiguration;
using Microsoft.AspNetCore.Diagnostics;
using System.Net;
using Microsoft.OpenApi.Models;

using webapi.Infrastructure.Data;
using webapi.Domain.Interfaces;
using webapi.Domain.Repositories;
using webapi.Services.Interfaces;
using webapi.Services;
using Microsoft.AspNetCore.Mvc;

namespace webapi
{
	class Startup
	{
		private IConfiguration _configuration { get; }


		IWebHostEnvironment _env;   // was IHostingEnvironment in Core 3.0

		readonly string MyAllowSpecificOrigins = "_myAllowSpecificOrigins";

		public Startup(IConfiguration configuration)
		{
			_configuration = configuration;
		}

		public void ConfigureServices(IServiceCollection services)
		{
			var useSQLServer = "False"; // builder.Configuration["UseSqlServer"].ToString();
			//var useSQLServer = _configuration["UseSqlServer"].ToString();

			//var useDiegelAzure = "True";    // builder.Configuration["UseDiegelAzure"].ToString();
			var useDiegelAzure = _configuration["UseDiegelAzure"].ToString();

			string connectOption = string.Empty;

			if (useDiegelAzure == "True")
				connectOption = "AzureDiegelConnection";
			else
				connectOption = "DefaultConnection";

			// services.Configuration.GetConnectionString(connectOption) 
			var connectionString = _configuration.GetConnectionString(connectOption) ??
									throw new InvalidOperationException($"Connection string {connectOption} not found.");

			services.AddControllers(options => options.EnableEndpointRouting = true);

			// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
			services.AddEndpointsApiExplorer();

			services.AddScoped<IContactsService, ContactsService>();
			services.AddScoped<IContactRepository, ContactRepository>();

			var folder = Environment.SpecialFolder.LocalApplicationData;
			var path = Environment.GetFolderPath(folder);

			var dbPath = Path.Join(path, "contacts.db");

			var migrationsAssembly = typeof(Program).GetTypeInfo().Assembly.GetName().Name;

			if (useSQLServer == "True" || useDiegelAzure == "True")
			{
				services.AddDbContext<ApplicationDbContext>(options =>
						 options.UseSqlServer(connectionString, b => b.MigrationsAssembly(migrationsAssembly)));
			}
			else
				services.AddDbContext<ApplicationDbContext>(options =>
				{
					options.UseSqlite($"Data Source={dbPath}");
				});

			// bool useDiegelAzure = AddServices(builder);

			// ApplicationDbContext  class can have Credential/Toek code for Azure
			if (useDiegelAzure == "True")
			{
				// WORKS WITH DEPLOYED APP ONLY
				string userAssignedClientId = "11951d92-dbbd-41ca-bee0-5364fa86fba9"; // dbuseraccess
																					  // *FAILED* var credential = new ManagedIdentityCredential(userAssignedClientId);
				var credential = new DefaultAzureCredential(new DefaultAzureCredentialOptions { ManagedIdentityClientId = userAssignedClientId });
				//	services.AddAzureClients(x => x.UseCredential(credential));

				// WORKS LOCALLY WITH: with Authentication=Active Directory Default;"

				services.AddAzureClients(x => x.UseCredential(new DefaultAzureCredential()));
			}

			services.AddMvc();

			// Register the Swagger generator, defining 1 or more Swagger documents
			services.AddSwaggerGen();
		}

		//											   IHostingEnvironment
		public void Configure(IApplicationBuilder app, IWebHostEnvironment env, ILoggerFactory loggerFactory)
		{

			//ConfigureRequestPipeline(app); // Configure the HTTP request pipeline.

			if (env.IsDevelopment())  //app.Environment
			{
				app.UseDeveloperExceptionPage();
				//IdentityModelEventSource.ShowPII = true;
			}
			else
			{
				app.UseExceptionHandler("/Error");
				// The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
				app.UseHsts();
			}

			app.UseHttpsRedirection();

			app.UseRouting();

			app.UseSwagger(c =>
			{
				c.SerializeAsV2 = true;
			});

			app.UseSwaggerUI(options =>
			{
				options.SwaggerEndpoint("/swagger/v1/swagger.json", "v1");
				// To serve the Swagger UI at the app's root (https://localhost:<port>/),
				// set the RoutePrefix property to an empty string:
				//options.RoutePrefix = string.Empty;  // DO NOT USE
			});

			//app.UseAuthorization();  // Moved here based on intelli-sense

			app.UseEndpoints(endpoints =>
			{
				endpoints.MapControllers();
			});

			app.UseStaticFiles();


			app.UseExceptionHandler(
				options =>
				{
					options.Run(async context =>
					{
						// This returns user error message to UI - Angular needs to pick up message
						context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
						context.Response.ContentType = "text/html";
						var exceptionObject = context.Features.Get<IExceptionHandlerFeature>();
						if (null != exceptionObject)
						{
							var errorMessage = $"{exceptionObject.Error.Message}";
							await context.Response.WriteAsync(errorMessage).ConfigureAwait(false);
						}
					});
				}
				);

			app.UseCors(builder => builder
				.AllowAnyOrigin()
				.AllowAnyHeader()
				.AllowAnyMethod());

			//app.UseMvc();  // moved to Sservices
			//app.UseMvcWithDefaultRoute();

			//SeedDatabase(app); //Seed initial database
		}
	}
}


		//builder.Configuration.AddAzureAppConfiguration(options =>
		//{
		//	if (options.KeyValueSelectors.Any()) { 
		//		options.Connect(builder.Configuration.GetConnectionString("ConnectionStrings"))
		//			// Load configuration values with no label
		//			.Select(KeyFilter.Any, LabelFilter.Null)
		//			// Override with any configuration values specific to current hosting env
		//			.Select(KeyFilter.Any, builder.Environment.EnvironmentName);
		//	}
		//});

		//var connect = builder.Configuration.AddAzureAppConfiguration((Environment.GetEnvironmentVariable("ConnectionString"));



		// Create a secret client using the DefaultAzureCredential
		//	var client = new SecretClient(new Uri("https://mwd-it-key.vault.azure.net/"), new DefaultAzureCredential());
		//});

	//	var app = builder.Build();


// app.Run();


//static bool AddServices(WebApplicationBuilder builder)
//{

	//if (useDiegelAzure == "True")
	//{
	//	// WORKS WITH DEPLOYED APP ONLY
	//	string userAssignedClientId = "11951d92-dbbd-41ca-bee0-5364fa86fba9"; // dbuseraccess
	//	// *FAILED* var credential = new ManagedIdentityCredential(userAssignedClientId);
	//	var credential = new DefaultAzureCredential(new DefaultAzureCredentialOptions { ManagedIdentityClientId = userAssignedClientId });
	//	//	builder.Services.AddAzureClients(x => x.UseCredential(credential));

	//	// WORKS LOCALLY WITH: with Authentication=Active Directory Default;"

	//		builder.Services.AddAzureClients(x => x.UseCredential(new DefaultAzureCredential()) );
	//}

	//// Create a secret client using the DefaultAzureCredential
	////	var client = new SecretClient(new Uri("https://mwd-it-key.vault.azure.net/"), new DefaultAzureCredential());
	////});


	// [10-05-23 MWD] Disabled only works when depluyed
	//  It is important to understand that Managed Identity feature in Azure 
	//  is ONLY relevant when the App Service is deployed. 
	//if (useDiegelAzure == "True")
	//{
	//	// Create a secret client using the DefaultAzureCredential
	//	var client = new SecretClient(new Uri("https://dbuseraccess.vault.azure.net/"), new DefaultAzureCredential());

	//	// 11951d92-dbbd-41ca-bee0-5364fa86fba9
	//	string userAssignedClientId = "11951d92-dbbd-41ca-bee0-5364fa86fba9";
	//	var credential = new DefaultAzureCredential(new DefaultAzureCredentialOptions { ManagedIdentityClientId = userAssignedClientId });
	//}
	//else
	//{
	//	var secretClient = new SecretClient(new Uri("https://mwd-it-key.vault.azure.net"), new DefaultAzureCredential());
	//	var secret = secretClient.GetSecretAsync("MySecretKey");
	//}


	//var connectionString = Configuration.GetConnectionString("QuotesDatabase");
	//builder.Services.AddTransient(a =>
	//{
	//	var sqlConnection = new SqlConnection(connectionString);
	//	var credential = new DefaultAzureCredential();
	//	var token = credential
	//		.GetToken(new Azure.Core.TokenRequestContext(
	//			new[] { "https://database.windows.net/.default" }));
	//	sqlConnection.AccessToken = token.Token;
	//	return sqlConnection;
	//});


//	return true;	//	 useDiegelAzure == "True";
//}

//static void ConfigureRequestPipeline(WebApplication app)
//{
//	if (app.Environment.IsDevelopment())
//	{
//		app.UseDeveloperExceptionPage();
//		app.UseSwagger();
//		app.UseSwaggerUI(options =>
//		{
//			options.SwaggerEndpoint("/swagger/v1/swagger.json", "v1");
//			// To serve the Swagger UI at the app's root (https://localhost:<port>/),
//			// set the RoutePrefix property to an empty string:
//			//options.RoutePrefix = string.Empty;  // DO NOT USE
//		});
//		//IdentityModelEventSource.ShowPII = true;
//	}
//	else
//	{
//		app.UseExceptionHandler("/Error");
//		// The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
//		app.UseHsts();
//	}

//	app.UseHttpsRedirection();
//	//app.UseStaticFiles();

//	app.UseExceptionHandler(
//	options =>
//	{
//		options.Run(async context =>
//		{
//			// This returns user error message to UI - Angular needs to pick up message
//			context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
//			context.Response.ContentType = "text/html";
//			var exceptionObject = context.Features.Get<IExceptionHandlerFeature>();
//			if (null != exceptionObject)
//			{
//				var errorMessage = $"{exceptionObject.Error.Message}";
//				await context.Response.WriteAsync(errorMessage).ConfigureAwait(false);
//			}
//		});
//	}
//);
//	app.UseRouting();
//	app.UseCors(builder => builder
//		.AllowAnyOrigin()
//		.AllowAnyHeader()
//		.AllowAnyMethod());

//	app.MapControllers();

//	/*
//	app.UseAuthorization();

//	app.MapControllerRoute(
//		name: "default",
//		pattern: "{controller}/{action=Index}/{id?}");

//	app.Map("api/{**slug}", context =>
//	{
//		context.Response.StatusCode = StatusCodes.Status404NotFound;
//		return Task.CompletedTask;
//	});

//	*/
//	app.MapFallbackToFile("index.html");
//}