using System.Collections.Generic;
using System.Text;
using System.Text.Json;

using webapi.FunctionalTests.Fixtures;
using webapi.Models;

using Ardalis.HttpClientTestExtensions;

using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace webapi.FunctionalTests.Endpoints.Contacts;

public class UpdateContact : IClassFixture<CustomWebApplicationFactory>
{
    private readonly HttpClient _client;
    private readonly JsonSerializerOptions _jsonOptions;

    public UpdateContact(CustomWebApplicationFactory factory)
    {
        _client = factory.CreateClient();
        // Match the default serializer for Asp.Net
        _jsonOptions = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        };
    }

    [Fact]
    public async Task UpdateContact_ReturnsDetails()
    {
        ContactModel contactModel = SeedData.NewContact;

        StringContent jsonContent = new(JsonSerializer.Serialize(contactModel),
                Encoding.UTF8, "application/json");

        contactModel.FirstName = "First updated";
        contactModel.LastName = "Last updated";
        contactModel.EmailAddress = "Updated@Outlook.com";

        var response = await _client.PostAsync($"contacts/{contactModel.Id}", jsonContent);

        response.EnsureSuccessStatusCode();

        string stringResponse = await response.Content.ReadAsStringAsync();

        response = await _client.GetAsync($"contacts/{contactModel.Id}");

        stringResponse = await response.Content.ReadAsStringAsync();

        stringResponse.Should().NotBeNullOrEmpty();


        var result = JsonSerializer.Deserialize<ContactModel>(stringResponse, _jsonOptions);
        result.Should().NotBeNull();
 
        result!.FirstName.Should().Be("First updated");
        result.LastName.Should().Be("Last updated");
        //result.BirthDate.Should().Be(SeedData.NewContact.BirthDate);
        result.EmailAddress.Should().Be("Updated@Outlook.com");
    }
}


///// <summary>
///// Placeholder for API results.  You may end up moving this to the application code later, in
///// which case reference that in the tests above.
///// </summary>
//internal class ContactResult
//{
//    public Guid Id { get; set; }
//    public string FirstName { get; set; } = string.Empty;
//    public string LastName { get; set; } = string.Empty;
//    public DateTime? BirthDate { get; set; }
//    public string? EmailAddress { get; set; }
//}