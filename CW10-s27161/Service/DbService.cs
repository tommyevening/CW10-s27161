using CW10_s27161.Data;
using CW10_s27161.DTOs;
using CW10_s27161.Models;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CW10_s27161.Service
{
    public interface IDbService
    {
        Task<IActionResult> GetTripsAsync();
    }

    public class DbService(Cw10Context data) : IDbService
    {
        public async Task<IActionResult> GetTripsAsync()
        {

            var trips = await data.Trips
                .Include(t => t.ClientTrips)
                .ThenInclude(ct => ct.IdClientNavigation)
                .Include(t => t.ClientTrips)
                .ThenInclude(ct => ct.IdTripNavigation)
                .ToListAsync();

            var tripDTO = trips.Select(t => new CountryTripGetDTO{
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

            return new OkObjectResult(tripDTO);

        }
    }
}