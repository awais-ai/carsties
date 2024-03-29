using AutoMapper;
using Contracts;
using MassTransit;
using MongoDB.Entities;
using SearchService.Models;

namespace SearchService;

public class AuctionDeletedConsumer  : IConsumer<AuctionDeleted>
{
    private readonly IMapper mapper;

    public AuctionDeletedConsumer(IMapper mapper)
    {
        this.mapper = mapper;
    }
    public async Task Consume(ConsumeContext<AuctionDeleted> context)
    {
        Console.WriteLine("--> Consuming auction deleted: " + context.Message.Id);

        var item = mapper.Map<Item>(context.Message);
        
        await item.DeleteAsync();
    }
}
