using AuctionService.Data;
using Contracts;
using MassTransit;

namespace AuctionService;

public class BidPlacedConsumer : IConsumer<BidPlaced>
{
  private readonly AuctionDbContext _dbcontext;

  public BidPlacedConsumer(AuctionDbContext context)
  {
    _dbcontext = context;
  }

  public async Task Consume(ConsumeContext<BidPlaced> context)
  {
    Console.WriteLine("--> Consuming bid placed");

    var auction = await _dbcontext.Auctions.FindAsync(Guid.Parse(context.Message.AuctionId));

    if (!auction.CurrentHighBid.HasValue
       || context.Message.BidStatus.Contains("Accepted")
       && context.Message.Amount > auction.CurrentHighBid)
    {
      auction.CurrentHighBid = context.Message.Amount;
      await _dbcontext.SaveChangesAsync();
    }
  }
}
