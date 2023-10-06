using webapi.Domain.Entities;
using webapi.Infrastructure.Data;
using webapi.Models;

using Microsoft.EntityFrameworkCore;

namespace webapi.FunctionalTests.Fixtures;

internal static class SeedData
{
    // Put seed data instances here

    public static readonly Contact Contact1 = new(Guid.NewGuid())
        {
        FirstName = "Joe",
        LastName = "Cool",
        EmailAddress = "jcool@example.com"
    };

	public static readonly Contact Contact2 = new(Guid.NewGuid())
	{
		FirstName = "Dave",
		LastName = "Cooler",
		EmailAddress = "jcooler@example.com"
	};

	public static readonly Contact Contact3 = new(Guid.NewGuid())
	{
		FirstName = "Allend",
		LastName = "Coolest",
		EmailAddress = "acoolest@example.com"
	};

	public static readonly ContactModel NewContact = new ContactModel()
    {
        FirstName = "Dave",
        LastName = "Tester",
        EmailAddress = "dtester@testing.com"
    };

    public static void PopulateTestData(ApplicationDbContext dbContext)
    {
        // Remove existing entities
        foreach (var item in dbContext.Contacts)
        {
            dbContext.Remove(item);
        }

        dbContext.SaveChanges();

        dbContext.Contacts.Add(Contact1);
		dbContext.Contacts.Add(Contact2);
		dbContext.Contacts.Add(Contact3);

		dbContext.SaveChanges();
    }
}