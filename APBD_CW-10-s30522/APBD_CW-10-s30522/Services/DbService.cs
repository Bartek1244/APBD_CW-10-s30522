using APBD_CW_10_s30522.Data;
using APBD_CW_10_s30522.DTOs;
using Microsoft.EntityFrameworkCore;

namespace APBD_CW_10_s30522.Services;

public interface IDbService
{
    public Task<TripDetailsPagingGetDTO> GetTripsAsync(int page, int pageSize);
}

public class DbService(ApbdContext data) : IDbService
{
    public async Task<TripDetailsPagingGetDTO> GetTripsAsync(int page, int pageSize)
    {
        return new TripDetailsPagingGetDTO
        {
            PageNum = page,
            PageSize = pageSize,
            AllPages = (int)Math.Ceiling((double)data.Trips.Count() / pageSize),
            Trips = await data.Trips.Select(trip => new TripDetailGetDTO
            {
                Name = trip.Name,
                Description = trip.Description,
                DateFrom = trip.DateFrom,
                DateTo = trip.DateTo,
                MaxPeople = trip.MaxPeople,
                Countries = trip.IdCountries.Select(cnt => new CountryBasicGetDTO
                {
                    Name = cnt.Name
                }).ToList(),
                Clients = trip.ClientTrips.Select(ct => new ClientBasicGetDTO
                {
                    FirstName = ct.IdClientNavigation.FirstName,
                    LastName = ct.IdClientNavigation.LastName
                }).ToList()
            }).OrderByDescending(trip => trip.DateFrom)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync()
        };
    }
}