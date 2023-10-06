using System.Text;
using System.Text.Json;

using webapi.FunctionalTests.Fixtures;
using webapi.Models;

using Ardalis.HttpClientTestExtensions;

namespace webapi.FunctionalTests.Endpoints.Contacts;

public class CreateContact : IClassFixture<CustomWebApplicationFactory>
{
    private readonly HttpClient _client;
    private readonly JsonSerializerOptions _jsonOptions;

    public CreateContact(CustomWebApplicationFactory factory)
    {
        _client = factory.CreateClient();
        // Match the default serializer for Asp.Net
        _jsonOptions = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        };
    }

    [Fact]
    public async Task CreateContact_ReturnsDetails()
    {
        ContactModel contactModel = SeedData.NewContact;

        StringContent jsonContent = new(JsonSerializer.Serialize(contactModel),
                Encoding.UTF8, "application/json");

        // Return GUID
        var response = await _client.PostAsync($"contacts/", jsonContent);

        var guid = await response.Content.ReadAsStringAsync();

        response.EnsureSuccessStatusCode();

        Guid validGuid = JsonSerializer.Deserialize<Guid>(guid, _jsonOptions);

        response = await _client.GetAsync($"contacts/{validGuid}");

        string stringResponse = await response.Content.ReadAsStringAsync();
      
        stringResponse.Should().NotBeNullOrEmpty();

        var result = JsonSerializer.Deserialize<ContactModel>(stringResponse, _jsonOptions);

        result.Should().NotBeNull();
 
        result!.FirstName.Should().Be(SeedData.NewContact.FirstName);
        result.LastName.Should().Be(SeedData.NewContact.LastName);
        // result.BirthDate.Should().Be(SeedData.NewContact.BirthDate);
        result.EmailAddress.Should().Be(SeedData.NewContact.EmailAddress);
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