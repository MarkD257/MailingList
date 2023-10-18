
using System.Reflection;
using Azure.Core;
using Azure.Identity;

using Microsoft.EntityFrameworkCore;
using webapi.Domain.Entities;

namespace webapi.Infrastructure.Data;

public class ApplicationDbContext : DbContext
{
    public DbSet<Contact> Contacts => Set<Contact>();

    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
    {
		var conn = (Microsoft.Data.SqlClient.SqlConnection)Database.GetDbConnection(); // NO VALUE TEST
		//var credential = new DefaultAzureCredential();
		
		/*
		TestCredentials();

		var credential = new DefaultAzureCredential(new DefaultAzureCredentialOptions { ManagedIdentityClientId = userAssignedClientId });

		//Exception: 'Cannot set the AccessToken property if 'Authentication' has been specified in the connection string.'
		var token = credential
				.GetToken(new Azure.Core.TokenRequestContext(
					new[] { "https://mwd-it-key.vault.azure.net/" })); // "https://mwdit.database.windows.net/.default"

		conn.AccessToken = token.Token;
		*/

		// remove from conection string: ;Authentication=Active Directory Default
	}

	protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
        base.OnModelCreating(modelBuilder);
    }


	private void TestCredentials()
	{
		var objDefaultAzureCredentialOptions = new DefaultAzureCredentialOptions
		{
			ExcludeEnvironmentCredential = false,
			ExcludeManagedIdentityCredential = true,
			ExcludeSharedTokenCacheCredential = true,
			ExcludeVisualStudioCredential = false,
			ExcludeVisualStudioCodeCredential = false,
			ExcludeAzureCliCredential = true,
			ExcludeInteractiveBrowserCredential = true
		};

		var tokenCredential = new DefaultAzureCredential(objDefaultAzureCredentialOptions);

		ValueTask<AccessToken> accessToken = tokenCredential.GetTokenAsync(
								new TokenRequestContext(scopes: new[] { "https://mwdit.database.windows.net/.default" }));

		Console.WriteLine(accessToken.Result);
	}
	
}
