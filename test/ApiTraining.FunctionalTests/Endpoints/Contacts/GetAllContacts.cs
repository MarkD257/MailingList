using System.Text.Json;
using webapi.FunctionalTests.Fixtures;
using Ardalis.HttpClientTestExtensions;

namespace webapi.FunctionalTests.Endpoints.Contacts;

public class GetAllContacts : IClassFixture<CustomWebApplicationFactory>
{
    private readonly HttpClient _client;
    private readonly JsonSerializerOptions _jsonOptions;

    public GetAllContacts(CustomWebApplicationFactory factory)
    {
        _client = factory.CreateClient();
        // Match the default serializer for Asp.Net
        _jsonOptions = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        };
    }

    [Fact]
    public async Task WhenFound_ReturnsDetails()
    {
        var response = await _client.GetAsync($"contacts");

        response.EnsureSuccessStatusCode();

        string stringResponse = await response.Content.ReadAsStringAsync();
      
        stringResponse.Should().NotBeNullOrEmpty();

        var result = JsonSerializer.Deserialize<List<ContactObj>>(stringResponse, _jsonOptions);

        result.Should().NotBeNull();
        result!.Count.Should().Be(1);
    }
}


/// <summary>
/// Placeholder for API results.  You may end up moving this to the application code later, in
/// which case reference that in the tests above.
/// </summary>
internal class ContactObj
{
    public Guid Id { get; set; }
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public DateTime? BirthDate { get; set; }
    public string? EmailAddress { get; set; }
}