using Microsoft.EntityFrameworkCore;
using System.Reflection;
// https://learn.microsoft.com/en-us/dotnet/azure/sdk/authentication/?tabs=command-line
using Azure.Identity;
using Microsoft.Extensions.Azure;

using webapi.Infrastructure.Data;
using webapi.Domain.Interfaces;
using webapi.Domain.Repositories;
using webapi.Services.Interfaces;
using webapi.Services;



var builder = WebApplication.CreateBuilder(args);

bool useDiegelAzure = AddServices(builder);

// ApplicationDbContext  class can have Credential/Toek code for Azure
if (useDiegelAzure)
{
	// WORKS WITH DEPLOYED APP ONLY
	string userAssignedClientId = "11951d92-dbbd-41ca-bee0-5364fa86fba9"; // dbuseraccess
																		  // *FAILED* var credential = new ManagedIdentityCredential(userAssignedClientId);
	var credential = new DefaultAzureCredential(new DefaultAzureCredentialOptions { ManagedIdentityClientId = userAssignedClientId });
	//	builder.Services.AddAzureClients(x => x.UseCredential(credential));

	// WORKS LOCALLY WITH: with Authentication=Active Directory Default;"

	builder.Services.AddAzureClients(x => x.UseCredential(new DefaultAzureCredential()));
}

// Create a secret client using the DefaultAzureCredential
//	var client = new SecretClient(new Uri("https://mwd-it-key.vault.azure.net/"), new DefaultAzureCredential());
//});


var app = builder.Build();
ConfigureRequestPipeline(app); // Configure the HTTP request pipeline.

//SeedDatabase(app); //Seed initial database

//app.MapGet("/Contacts", (ApplicationDbContext context) =>
//{
//	return context.Contacts.ToList();
//})
//.WithName("GetAllContacts");
////.WithOpenApi();

//app.MapPost("/Contact", (Contact contact, ApplicationDbContext context) =>
//{
//	context.Add(contact);
//	context.SaveChanges();
//})
//.WithName("CreatePerson");
////.WithOpenApi();

app.Run();


static bool AddServices(WebApplicationBuilder builder)
{
	var useSQLServer = builder.Configuration["UseSqlServer"].ToString();

	var useDiegelAzure = builder.Configuration["UseDiegelAzure"].ToString();

	string connectString = string.Empty;

	if (useDiegelAzure == "True")
		connectString = "AzureDiegelConnection";
	else
		connectString = "DefaultConnection";

	var connectionString = builder.Configuration.GetConnectionString(connectString) ??
							throw new InvalidOperationException($"Connection string {connectString} not found.");
	
	builder.Services.AddSingleton(connectionString);

	builder.Services.AddControllers();

	// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
	builder.Services.AddEndpointsApiExplorer();
	builder.Services.AddSwaggerGen();


	builder.Services.AddScoped<IContactsService, ContactsService>();
	builder.Services.AddScoped<IContactRepository, ContactRepository>();

	var folder = Environment.SpecialFolder.LocalApplicationData;
	var path = Environment.GetFolderPath(folder);
	
	var dbPath = Path.Join(path, "contacts.db");

	var migrationsAssembly = typeof(Program).GetTypeInfo().Assembly.GetName().Name;

	if (useSQLServer == "True" || useDiegelAzure == "True")
	{
		builder.Services.AddDbContext<ApplicationDbContext>(options =>
				 options.UseSqlServer(connectionString, b => b.MigrationsAssembly(migrationsAssembly)));
	}
	else
		builder.Services.AddDbContext<ApplicationDbContext>(options =>
		{
			options.UseSqlite($"Data Source={dbPath}");
		});


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
	return useDiegelAzure == "True";
}

static void ConfigureRequestPipeline(WebApplication app)
{
	if (app.Environment.IsDevelopment())
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
	app.UseStaticFiles();
	app.UseRouting();
	app.UseCors(builder => builder
		.AllowAnyOrigin()
		.AllowAnyHeader()
		.AllowAnyMethod());

	//app.UseIdentityServer();
	app.UseAuthorization();

	if (app.Environment.IsDevelopment())
	{
		app.UseSwagger();
		app.UseSwaggerUI(options =>
		{
			options.SwaggerEndpoint("/swagger/v1/swagger.json", "v1");
			// To serve the Swagger UI at the app's root (https://localhost:<port>/),
			// set the RoutePrefix property to an empty string:
			// options.RoutePrefix = string.Empty;  // DO NOT USE
		});
	}


	//app.UseSwaggerUI(c =>
	//{
	//	c.DocumentTitle = "Swagger UI - Contact Mailing List";
	//	c.SwaggerEndpoint("/swagger/v1/swagger.json", $"Contact Mailing List API V1");

	//});

	app.MapControllerRoute(
		name: "default",
		pattern: "{controller}/{action=Index}/{id?}");

	app.Map("api/{**slug}", context =>
	{
		context.Response.StatusCode = StatusCodes.Status404NotFound;
		return Task.CompletedTask;
	});

	app.MapFallbackToFile("index.html");
}