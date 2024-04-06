
using System.Net;
using System.Net.Http.Json;
using AuctionService.Data;
using AuctionService.DTOs;
using Contracts;
using MassTransit.Testing;
using Microsoft.Extensions.DependencyInjection;

namespace AuctionService.IntegrationTests;

[Collection("Shared collection")]
public class AuctionBusTests : IAsyncLifetime
{
  private readonly CustomWebAppFactory _factory;
  private readonly HttpClient _httpClient;
  private ITestHarness _testHarness;

  public AuctionBusTests(CustomWebAppFactory factory)
  {
    _factory = factory;
    _httpClient = factory.CreateClient();
    _testHarness = factory.Services.GetTestHarness();
  }

  public Task InitializeAsync() => Task.CompletedTask;

  public Task DisposeAsync()
  {
    using var scope = _factory.Services.CreateScope();
    var db = scope.ServiceProvider.GetRequiredService<AuctionDbContext>();
    DbHelper.ReinitDbForTests(db);
    return Task.CompletedTask;
  }

  [Fact]
  public async Task CreateAuction_WithValidObject_ShoudPublishAuctionCreated()
  {
    var auction = GetAuctionForCreate();
    _httpClient.SetFakeJwtBearerToken(AuthHelper.GetBearerForUser("paulo"));

    
    var response = await _httpClient.PostAsJsonAsync("api/auctions", auction);
    
    
    response.EnsureSuccessStatusCode();
    Assert.True(await _testHarness.Published.Any<AuctionCreated>());
  } 

  private CreateAuctionDto GetAuctionForCreate()
  {
    return new CreateAuctionDto
    {
      Color = "testColor",
      Make = "testMake",
      Model = "testModel",
      ImageUrl = "testImage",
      Mileage = 10,
      Year = 10,
      ReservePrice = 10
    };
  }



}
