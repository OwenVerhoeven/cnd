using cnd;
using cnd.DTOs;
using cnd.Models.Auth;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using Xunit;

namespace JournalApi.Tests.Controllers
{
    public class JournalEntriesControllerTests : IClassFixture<WebApplicationFactory<Program>>
    {
        private readonly HttpClient _client;

        public JournalEntriesControllerTests(WebApplicationFactory<Program> factory)
        {
            _client = factory.WithWebHostBuilder(builder =>
            {
                builder.ConfigureAppConfiguration((context, config) =>
                {
                    config.AddInMemoryCollection(new Dictionary<string, string?>
                    {
                        { "Jwt:Key", "DezeSleutelMoetMinimaal32TekensLangZijn!" },
                        { "Jwt:Issuer", "JournalApi" },
                        { "Jwt:Audience", "JournalApiUser" }
                    });
                });
            }).CreateClient();
        }

        private async Task<string> GetJwtTokenAsync()
        {
            var user = new RegisterRequest
            {
                Username = $"integration_{Guid.NewGuid():N}",
                Password = "Test123!"
            };

            await _client.PostAsJsonAsync("/auth/register", user);
            var response = await _client.PostAsJsonAsync("/auth/login", user);
            response.EnsureSuccessStatusCode();

            var json = await response.Content.ReadFromJsonAsync<Dictionary<string, string>>();
            return json!["token"];
        }

        private async Task SetAuthHeaderAsync()
        {
            var token = await GetJwtTokenAsync();
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        }

        [Fact]
        public async Task PostEntry_ReturnsSuccess()
        {
            await SetAuthHeaderAsync();

            var dto = new JournalEntryCreateDto
            {
                Title = "Integration Test",
                Content = "Works end-to-end",
                Song = new SongDto { Artist = "AI", Title = "Symphony" }
            };

            var response = await _client.PostAsJsonAsync("/entries", dto);
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);

            var result = await response.Content.ReadFromJsonAsync<JournalEntryDto>();
            Assert.NotNull(result);
            Assert.Equal("Integration Test", result!.Title);
            Assert.Equal("AI", result.Song!.Artist);
        }

        [Fact]
        public async Task GetEntries_ReturnsCreatedEntry()
        {
            await SetAuthHeaderAsync();

            var dto = new JournalEntryCreateDto
            {
                Title = "MyEntry",
                Content = "Details",
                Song = new SongDto { Artist = "Artist1", Title = "Track1" }
            };

            await _client.PostAsJsonAsync("/entries", dto);
            var response = await _client.GetAsync("/entries");

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);

            var entries = await response.Content.ReadFromJsonAsync<List<JournalEntryDto>>();
            Assert.NotNull(entries);
            Assert.Contains(entries!, e => e.Title == "MyEntry");
        }

        [Fact]
        public async Task GetEntryById_ReturnsCorrectEntry()
        {
            await SetAuthHeaderAsync();

            var createDto = new JournalEntryCreateDto
            {
                Title = "SpecificEntry",
                Content = "SpecificContent"
            };

            var postResponse = await _client.PostAsJsonAsync("/entries", createDto);
            var created = await postResponse.Content.ReadFromJsonAsync<JournalEntryDto>();

            var getResponse = await _client.GetAsync($"/entries/{created!.JournalEntryId}");
            Assert.Equal(HttpStatusCode.OK, getResponse.StatusCode);

            var result = await getResponse.Content.ReadFromJsonAsync<JournalEntryDto>();
            Assert.Equal("SpecificEntry", result!.Title);
        }

        [Fact]
        public async Task UpdateEntry_ChangesEntry()
        {
            await SetAuthHeaderAsync();

            var postDto = new JournalEntryCreateDto { Title = "ToUpdate", Content = "Old" };
            var postResponse = await _client.PostAsJsonAsync("/entries", postDto);
            var created = await postResponse.Content.ReadFromJsonAsync<JournalEntryDto>();

            var updateDto = new JournalEntryUpdateDto { Title = "Updated", Content = "New" };
            var updateResponse = await _client.PutAsJsonAsync($"/entries/{created!.JournalEntryId}", updateDto);

            Assert.Equal(HttpStatusCode.OK, updateResponse.StatusCode);
        }

        [Fact]
        public async Task DeleteEntry_RemovesEntry()
        {
            await SetAuthHeaderAsync();

            var dto = new JournalEntryCreateDto { Title = "ToDelete", Content = "Gone" };
            var postResponse = await _client.PostAsJsonAsync("/entries", dto);
            var created = await postResponse.Content.ReadFromJsonAsync<JournalEntryDto>();

            var deleteResponse = await _client.DeleteAsync($"/entries/{created!.JournalEntryId}");
            Assert.Equal(HttpStatusCode.OK, deleteResponse.StatusCode);

            var getResponse = await _client.GetAsync($"/entries/{created!.JournalEntryId}");
            Assert.Equal(HttpStatusCode.NotFound, getResponse.StatusCode);
        }

        [Fact]
        public async Task DeleteSongOnly_RemovesSong()
        {
            await SetAuthHeaderAsync();

            var dto = new JournalEntryCreateDto
            {
                Title = "SongHolder",
                Content = "Has Song",
                Song = new SongDto { Artist = "DeleteMe", Title = "SongOnly" }
            };

            var postResponse = await _client.PostAsJsonAsync("/entries", dto);
            var created = await postResponse.Content.ReadFromJsonAsync<JournalEntryDto>();

            var deleteResponse = await _client.DeleteAsync($"/entries/{created!.JournalEntryId}/song");
            Assert.Equal(HttpStatusCode.OK, deleteResponse.StatusCode);

            var getResponse = await _client.GetAsync($"/entries/{created!.JournalEntryId}");
            var result = await getResponse.Content.ReadFromJsonAsync<JournalEntryDto>();
            Assert.Null(result!.Song);
        }
    }
}
