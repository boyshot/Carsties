using AuctionService.Entities;
using Microsoft.EntityFrameworkCore.Query;

namespace AuctionService.UnitTests;

public class AuctionEntityTest
{
    [Fact]
    //MethodName_Scenario_Result
    public void HasReservePrice_ReservePriceGtZero_True()
    {
        var auction = new Auction { Id = Guid.NewGuid(), ReservePrice = 10 };

        var result = auction.HasReservePrice();

        Assert.True(result);
    }

    [Fact]
    public void HasReservePrice_ReservePriceGtZero_False()
    {
        var auction = new Auction { Id = Guid.NewGuid(), ReservePrice = 0 };

        var result = auction.HasReservePrice();

        Assert.False(result);
    }
}