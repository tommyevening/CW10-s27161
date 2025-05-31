using CW10_s27161.Data;
using CW10_s27161.DTOs;
using CW10_s27161.Models;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CW10_s27161.Service
{
    // ...
}

public interface IDbService
{
    Task<TripGetDTO> GetTripsAsync(int page = 1, int pageSize = 10);
    public Task<IActionResult> DeleteClient(int idClient);
    public Task<IActionResult> GetClientAsync(int idClient);
    public Task<IActionResult> AddClientToTrip(CreateClientTripDTO dto);
    
}



public class DbService(Cw10Context data) : IDbService
{
    
    public async Task<IActionResult> AddClientToTrip(CreateClientTripDTO dto)
{
    using var transaction = await data.Database.BeginTransactionAsync();
    try
    {
        var trip = await data.Trips.FirstOrDefaultAsync(t => t.IdTrip == dto.IdTrip);
        if (trip == null)
            return new BadRequestObjectResult("Trip with that Id does not exist");
        if (trip.DateFrom < DateTime.Now)
            return new BadRequestObjectResult("Cannot sign up for a trip that already started");
        
        var client = await data.Clients.FirstOrDefaultAsync(c => c.Pesel == dto.Pesel);
        if (client == null)
        {
            client = new Client
            {
                FirstName = dto.FirstName,
                LastName = dto.LastName,
                Email = dto.Email,
                Telephone = dto.Telephone,
                Pesel = dto.Pesel
            };
            data.Clients.Add(client);
            await data.SaveChangesAsync(); 
        }
        
        var alreadySigned = await data.ClientTrips.AnyAsync(ct => ct.IdClient == client.IdClient && ct.IdTrip == dto.IdTrip);
        if (alreadySigned)
            return new BadRequestObjectResult("Client is already signed for that trip");
        
        var clientTrip = new ClientTrip
        {
            IdClient = client.IdClient,
            IdTrip = dto.IdTrip,
            RegisteredAt = DateTime.Now,
            PaymentDate = dto.PaymentDate 
        };
        data.ClientTrips.Add(clientTrip);
        await data.SaveChangesAsync();
        
        await transaction.CommitAsync();

        return new CreatedResult("", "Client added to trip");
    }
    catch (Exception)
    {
        await transaction.RollbackAsync();
        return new StatusCodeResult(500); 
    }
}
    
    public async Task<TripGetDTO> GetTripsAsync(int page = 1, int pageSize = 10)
    {
        var query = data.Trips
            .Include(t => t.ClientTrips)
            .ThenInclude(ct => ct.IdClientNavigation)
            .Include(t => t.IdCountries)
            .OrderByDescending(t => t.DateFrom);

        var totalTrips = await query.CountAsync();
        var allPages = (int)Math.Ceiling((double)totalTrips / pageSize);
        var trips = await query
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        var tripDtos = trips.Select(t => new CountryTripGetDTO{
            Name = t.Name,
            Description = t.Description,
            DateFrom = t.DateFrom,
            DateTo = t.DateTo,
            MaxPeople = t.MaxPeople,
            Countries = t.IdCountries.Select(c => new CountryGetDTO{
                Name = c.Name
            }).ToList(),
            Clients = t.ClientTrips.Select(c => new ClientGetDTO{
                FirstName = c.IdClientNavigation.FirstName,
                LastName = c.IdClientNavigation.LastName,
            }).ToList()
        }).ToList();


        return new TripGetDTO{
            PageNum = page,
            PageSize = pageSize,
            AllPages = allPages,
            Trips = tripDtos
        };
    }


    public async Task<IActionResult> GetClientAsync(int idClient)
    {
        var client = await data.Clients.FirstOrDefaultAsync(c => c.IdClient == idClient);
        if (client == null)
        {
            return new NotFoundObjectResult("Client not found");
        }

        return new OkObjectResult(client);
    }

    

    public async Task<IActionResult> DeleteClient(int idClient)
    {
        var hasTrips = await data.Trips.AnyAsync(t => t.ClientTrips.Any(ct => ct.IdClient == idClient));

        if (hasTrips)
        {
            return new BadRequestObjectResult("Client has trips and cannot be deleted");
        }

        var client = await data.Clients.FirstOrDefaultAsync(t => t.IdClient == idClient);

        if (client == null)
        {
            return new NotFoundObjectResult("Client not found");
        }

        data.Clients.Remove(client);
        await data.SaveChangesAsync();

        return new NoContentResult();
    }
}