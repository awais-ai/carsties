using AutoMapper;
using Contracts;
using MassTransit;
using MongoDB.Entities;
using SearchService.Models;

namespace SearchService;

public class AuctionUpdatedConsumer : IConsumer<AuctionUpdated>
{
    private readonly IMapper mapper;

    public AuctionUpdatedConsumer(IMapper mapper)
    {
        this.mapper = mapper;
    }
    public async Task Consume(ConsumeContext<AuctionUpdated> context)
    {
        Console.WriteLine("--> Consuming auction updated: " + context.Message.Id);

        var item = mapper.Map<Item>(context.Message);

        await item.SaveOnlyAsync(x => new 
        { 
            x.Make,
            x.Model,
            x.Color,
            x.Mileage,
            x.Year
        });
    }
}