
using System.Net;
using System.Net.Http.Json;
using AuctionService.Data;
using AuctionService.DTOs;
using Microsoft.Extensions.DependencyInjection;

namespace AuctionService.IntegrationTests;

[Collection("Shared collection")]
public class AuctionControllerTest : IAsyncLifetime
{
  private readonly CustomWebAppFactory _factory;
  private readonly HttpClient _httpClient;

  private const string GT_ID = "bbab4d5a-8565-48b1-9450-5ac2a5c4a654";

  public AuctionControllerTest(CustomWebAppFactory factory)
  {
    _factory = factory;
    _httpClient = factory.CreateClient();
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
  public async Task GetAuctions_ShoudReturn3Auctions()
  {
    // act
    var response = await _httpClient.GetFromJsonAsync<List<AuctionDto>>("api/auctions");

    //assert
    Assert.Equal(3, response.Count);
  }

  [Fact]
  public async Task GetAuctionBydId_WithValidIdShoudReturnAuction()
  {
    // act
    var response = await _httpClient.GetFromJsonAsync<AuctionDto>($"api/auctions/{GT_ID}");

    //assert
    Assert.Equal("Mustang", response.Model);
  }

  [Fact]
  public async Task GetAuctionBydId_WithInvalidIdShoudReturn404()
  {
    // act
    var response = await _httpClient.GetAsync($"api/auctions/{Guid.NewGuid()}");

    //assert
    Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
  }

  [Fact]
  public async Task GetAuctionBydId_WithInvalidIdShoudReturn400()
  {
    // act
    var response = await _httpClient.GetAsync($"api/auctions/notguid");

    //assert
    Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
  }

  [Fact]
  public async Task CreateAuction_WithNoAuthShouldReturn401()
  {
    //Arragnge
    var auction = new CreateAuctionDto { Make = "teste" };

    // act
    var response = await _httpClient.PostAsJsonAsync("api/auctions", auction);

    //assert
    Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
  }

  [Fact]
  public async Task CreateAuction_WithAuthShouldReturn201()
  {
    //Arragnge
    var auction = GetAuctionForCreate();

    _httpClient.SetFakeJwtBearerToken(AuthHelper.GetBearerForUser("paulo"));

    // act
    var response = await _httpClient.PostAsJsonAsync("api/auctions", auction);

    //assert
    response.EnsureSuccessStatusCode();
    Assert.Equal(HttpStatusCode.Created, response.StatusCode);
    var createdAuction = await response.Content.ReadFromJsonAsync<AuctionDto>();
    Assert.Equal("paulo", createdAuction.Seller);
  }

  [Fact]
  public async Task CreateAuction_WithInvalidCreateAuctionDto_ShouldReturn400()
  {
    //Arragnge
    var auction = GetAuctionForCreate();
    auction.Color = null;

    _httpClient.SetFakeJwtBearerToken(AuthHelper.GetBearerForUser("paulo"));

    // act
    var response = await _httpClient.PostAsJsonAsync("api/auctions", auction);

    //assert
    Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
  }

  [Fact]
  public async Task UpdateAuction_WithValidUpdateDtoAndUser_ShouldReturn200()
  {
    // arrange
    _httpClient.SetFakeJwtBearerToken(AuthHelper.GetBearerForUser("bob"));
    var auction = GetAuctionForUpdateDto();

    // act
    var responseUpd = await _httpClient.PutAsJsonAsync($"api/auctions/{GT_ID}", auction);

    // assert
    responseUpd.EnsureSuccessStatusCode();
    Assert.Equal(HttpStatusCode.OK, responseUpd.StatusCode);
  }
  
    [Fact]
    public async Task UpdateAuction_WithValidUpdateDtoAndInvalidUser_ShouldReturn403()
    {
    // arrange
    _httpClient.SetFakeJwtBearerToken(AuthHelper.GetBearerForUser("pedro"));
    var auction = GetAuctionForUpdateDto();

    // act
    var responseUpd = await _httpClient.PutAsJsonAsync($"api/auctions/{GT_ID}", auction);

    // assert
    Assert.Equal(HttpStatusCode.Forbidden, responseUpd.StatusCode);
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

  private UpdateAuctionDto GetAuctionForUpdateDto()
  {
    return new UpdateAuctionDto
    {
      Color = "testColor",
      Make = "testMake",
      Model = "testModel",
      Mileage = 10,
      Year = 10
    };
  }



}
