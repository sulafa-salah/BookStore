using Catalog.Api.IntegrationTests.Common;
using Catalog.Contracts.Categories;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;

using System.Net;
using System.Net.Http.Json;


namespace Catalog.Api.IntegrationTests.Controllers;


[Collection(CatalogApiFactoryCollection.CollectionName)]

public class CreateCategoryTests
{
    private readonly HttpClient _client;
    private const string BaseUrl = "/api/categories";
    public CreateCategoryTests(CatalogApiFactory apiFactory)
    {
        _client = apiFactory.HttpClient;
        apiFactory.ResetDatabase();
    }
    [Fact]
    public async Task CreateCategory_WhenValid_ShouldReturn200_AndPersist()
    {
        // Arrange
        var request = new CreateCategoryRequest(
            Name: "Technology",
            Description: "Programming & AI");

        // Act
        var response = await _client.PostAsJsonAsync($"{BaseUrl}", request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK); 
        var body = await response.Content.ReadFromJsonAsync<CategoryResponse>();
        body.Should().NotBeNull();
        body!.Id.Should().NotBe(Guid.Empty);
        body.Name.Should().Be("Technology");
        body.Description.Should().Be("Programming & AI");

        
    }

    [Fact]
    public async Task CreateCategory_WhenDuplicateName_ShouldReturnConflict409()
    {
        // Seed one
        var first = await _client.PostAsJsonAsync($"{BaseUrl}",
            new CreateCategoryRequest("Fiction", "All fiction"));
        first.IsSuccessStatusCode.Should().BeTrue();

        // Try same name again
        var second = await _client.PostAsJsonAsync($"{BaseUrl}",
            new CreateCategoryRequest("Fiction", "Anything"));

        // mapping uses ErrorType.Conflict 409:
        second.StatusCode.Should().Be(HttpStatusCode.Conflict);
       
    }

    [Theory]
    [InlineData("")]      // empty
    [InlineData("a")]     // too short if you enforce min length 2
    public async Task CreateCategory_WhenInvalidName_ShouldReturnBadRequest400(string invalidName)
    {
        var response = await _client.PostAsJsonAsync($"{BaseUrl}",
            new CreateCategoryRequest(invalidName, "desc"));

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        //  deserialize ProblemDetails to inspect the payload:
         var problem = await response.Content.ReadFromJsonAsync<ProblemDetails>();
         problem!.Status.Should().Be(400);
    }
}