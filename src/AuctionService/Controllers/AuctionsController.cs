using AuctionService.Data;
using AuctionService.DTOs;
using AuctionService.Entities;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AuctionService.Controllers;

[ApiController]
[Route("api/auctions")]
public class AuctionsController : ControllerBase
{
    private readonly AuctionDbContext context;
    private readonly IMapper mapper;

    public AuctionsController(AuctionDbContext context, IMapper mapper)
    {
        this.context = context;
        this.mapper = mapper;
    }

    [HttpGet]
    public async Task<ActionResult<List<AuctionDto>>> GetAllAuctions()
    {
        List<Entities.Auction> auctions = await context.Auctions
            .Include(x => x.Item)
            .OrderBy(x => x.Item.Make)
            .ToListAsync();

        return mapper.Map<List<AuctionDto>>(auctions);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<AuctionDto>> GetAuctionById(Guid id)
    {
        Auction auction = await context.Auctions
            .Include(x => x.Item)
            .FirstOrDefaultAsync(x => x.Id == id);

        return auction != null ? (ActionResult<AuctionDto>)mapper.Map<AuctionDto>(auction) : NotFound();
    }

    [HttpPost]
    public async Task<ActionResult<AuctionDto>> CreateAuction(CreateAuctionDto auctionDto)
    {
        Auction auction = mapper.Map<Auction>(auctionDto);
        auction.Seller = "test";

        context.Auctions.Add(auction);

        bool result = await context.SaveChangesAsync() > 0;

        if (!result) return BadRequest("Could not save changes to DB");

        return CreatedAtAction(nameof(GetAuctionById), 
            new {auction.Id}, mapper.Map<AuctionDto>(auction));
    }

    [HttpPut("{id}")]
    public async Task<ActionResult> UpdateAuction(Guid id, UpdateAuctionDto updateAuctionDto)
    {
        
        
        var auction = await context.Auctions.Include(x => x.Item)
            .FirstOrDefaultAsync(x => x.Id == id);

        if (auction == null)
        {
            return NotFound();
        }

        // TODO: check seller == username

        auction.Item.Make = updateAuctionDto.Make ?? auction.Item.Make;
        auction.Item.Model = updateAuctionDto.Model ?? auction.Item.Model;
        auction.Item.Color = updateAuctionDto.Color ?? auction.Item.Color;
        auction.Item.Mileage = updateAuctionDto.Mileage ?? auction.Item.Mileage;
        auction.Item.Year = updateAuctionDto.Year ?? auction.Item.Year;

        bool result = await context.SaveChangesAsync() > 0;

        return result ? Ok() : BadRequest("Problem Saving Changes");
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult> DeleteAuction(Guid id)
    {
        Auction auction = await context.Auctions.FindAsync(id);

        if (auction != null)
        {
            //TODO: check seller == username
            context.Auctions.Remove(auction);
            bool result = await context.SaveChangesAsync() > 0;
            return result ? Ok() : BadRequest("Could not update DB");
        }
        else 
        {
            return NotFound();
        }
    }
}
