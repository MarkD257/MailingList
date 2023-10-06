
using Microsoft.OpenApi.Models;
using webapi.Domain.Interfaces;
using webapi.Domain.Repositories;
using webapi.Services.Interfaces;
using webapi.Services;
using Microsoft.EntityFrameworkCore;
using webapi.Infrastructure.Data;

//string MyAllowSpecificOrigins = "_AllowSpecificOrigins";

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
//builder.Services.AddCors(options =>
//{
//	options.AddPolicy("angularapp", policy =>
//		policy.WithOrigins("https://localhost:4200")
//		.AllowAnyMethod()
//		.AllowAnyHeader()
//	);

//	options.AddPolicy("webapi", policy =>
//		policy.WithOrigins("https://localhost:7164")
//		.AllowAnyMethod()
//		.AllowAnyHeader()
//	);

//});

//builder.Services.AddCors();
//builder.Services.AddCors(options =>
//{
//	options.AddPolicy("MyPolicy", p => p.AllowAnyMethod().AllowAnyHeader().AllowCredentials()
//		.WithOrigins( "https://localhost", "https://localhost:4200", "https://localhost:7164"));

//	options.AddDefaultPolicy(
//			builder =>
//			{
//				builder.WithOrigins("https://localhost:44341", "https://localhost:4200", "https://localhost:7164")
//									.AllowAnyHeader()
//									.AllowAnyMethod();
//			});
//});

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddScoped<IContactsService, ContactsService>();
builder.Services.AddScoped<IContactRepository, ContactRepository>();

var folder = Environment.SpecialFolder.LocalApplicationData;
var path = Environment.GetFolderPath(folder);
var dbPath = Path.Join(path, "contacts.db");
builder.Services.AddDbContext<ApplicationDbContext>(options =>
{
	options.UseSqlite($"Data Source={dbPath}");
});

/// ****  CORS MOVED ABOVE


// Add CORS  [09-11-23 MWD] added
//builder.Services.AddCors();

// https://stackoverflow.com/questions/31942037/how-to-enable-cors-in-asp-net-core
//builder.Services.AddCors(options =>
//{
//	// Added to controller methods: [EnableCors("MyPolicy")]
//	options.AddPolicy("MyPolicy", options => options
//			.AllowAnyOrigin()
//			.AllowAnyMethod()
//		   .AllowAnyHeader());
//	options.AddPolicy(name: MyAllowSpecificOrigins,
//					  builder =>
//					  {
//						  builder.WithOrigins("https://localhost:4200",
//											  "https://weatherwidget.io");
//					  });
//});

//builder.Services.AddCors(options =>
//{
//	options.AddPolicy(name: MyAllowSpecificOrigins,
//		policy =>
//		{
//			policy.WithOrigins("https://localhost",
//							   "https://localhost:4200",
//							   "https://localhost:7164")
//					.AllowAnyHeader()
//					.AllowAnyMethod();
//		});
//});

//builder.Services.AddCors(options =>
//{
//	options.AddPolicy("MyPolicy",
//		builder => builder.WithOrigins("localhost", "localhost:7164")
//		.AllowAnyMethod()
//		.AllowAnyHeader()
//		.AllowCredentials());
//});

/// ****  CORS MOVED ABOVE


// Add cors
builder.Services.AddCors();

builder.Services.AddControllersWithViews();


// config.MapHttpAttributeRoutes();
var app = builder.Build();

// Configure the HTTP request pipeline.
app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseCors(builder => builder
	.AllowAnyOrigin()
	.AllowAnyHeader()
	.AllowAnyMethod());

app.UseAuthorization();

if (app.Environment.IsDevelopment())
{
	app.UseSwagger();
	app.UseSwaggerUI();
}
app.MapFallbackToFile("index.html");

app.Run();