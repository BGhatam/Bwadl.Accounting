using Bwadl.Accounting.Application.Common.DTOs;
using Bwadl.Accounting.API.Models.Users;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using System.Net.Http.Json;
using Xunit;

namespace Bwadl.Accounting.Tests.Integration;

public class ApiVersioningTests : IClassFixture<TestWebApplicationFactory<Program>>
{
    private readonly TestWebApplicationFactory<Program> _factory;

    public ApiVersioningTests(TestWebApplicationFactory<Program> factory)
    {
        _factory = factory;
    }

    [Fact]
    public async Task GetUsers_V1_Should_Return_Paged_Response()
    {
        // Arrange
        var client = _factory.CreateClient();

        // Act
        var response = await client.GetFromJsonAsync<PagedResponse<UserDto>>("/api/v1/users");

        // Assert
        response.Should().NotBeNull();
        response.Should().BeOfType<PagedResponse<UserDto>>();
        response!.Data.Should().NotBeNull();
        response.Page.Should().Be(1);
        response.PageSize.Should().Be(10); // Default page size
    }

    [Fact]
    public async Task GetUsers_V2_Should_Return_NotFound_Since_V2_Controller_Removed()
    {
        // Arrange
        var client = _factory.CreateClient();

        // Act
        var response = await client.GetAsync("/api/v2/users?page=1&pageSize=5");

        // Assert
        response.StatusCode.Should().Be(System.Net.HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task GetUsers_Default_Should_Return_Paged_Response()
    {
        // Arrange
        var client = _factory.CreateClient();

        // Act - Using explicit v1 versioning
        var response = await client.GetFromJsonAsync<PagedResponse<UserDto>>("/api/v1/users");

        // Assert
        response.Should().NotBeNull();
        response.Should().BeOfType<PagedResponse<UserDto>>();
        response!.Data.Should().NotBeNull();
        response.Page.Should().Be(1);
        response.PageSize.Should().Be(10); // Default page size
    }

    [Fact]
    public async Task GetUsers_QueryString_Versioning_Should_Work()
    {
        // Arrange
        var client = _factory.CreateClient();

        // Act - Version via query string with pagination
        var response = await client.GetFromJsonAsync<PagedResponse<UserDto>>("/api/v1/users?page=1&pageSize=3");

        // Assert
        response.Should().NotBeNull();
        response.Should().BeOfType<PagedResponse<UserDto>>();
        response!.PageSize.Should().Be(3);
    }

    [Fact]
    public async Task GetUsers_Header_Versioning_Should_Work()
    {
        // Arrange
        var client = _factory.CreateClient();
        client.DefaultRequestHeaders.Add("X-Version", "1.0");

        // Act - Version via header with pagination
        var response = await client.GetFromJsonAsync<PagedResponse<UserDto>>("/api/v1/users?page=1&pageSize=2");

        // Assert
        response.Should().NotBeNull();
        response.Should().BeOfType<PagedResponse<UserDto>>();
        response!.PageSize.Should().Be(2);
    }
}
