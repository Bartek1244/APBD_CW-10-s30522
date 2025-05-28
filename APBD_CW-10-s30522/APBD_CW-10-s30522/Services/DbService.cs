using APBD_CW_10_s30522.Data;
using APBD_CW_10_s30522.DTOs;
using APBD_CW_10_s30522.Exceptions;
using APBD_CW_10_s30522.Models;
using Microsoft.EntityFrameworkCore;

namespace APBD_CW_10_s30522.Services;

public interface IDbService
{
    public Task<TripDetailsPagingGetDTO> GetTripsAsync(int page, int pageSize);
    public Task<int> DeleteClientAsync(int idClient);
    public Task<ClientTripGetDTO> AssignClientToTripWithOptionalClientCreationAsync(
        ClientAssignmentDTO clientAssignment, int idTrip);
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

    public async Task<int> DeleteClientAsync(int idClient)
    {
        var client = await data.Clients.FirstOrDefaultAsync(cl => cl.IdClient == idClient);
        
        if (client == null)
        {
            throw new NotFoundException($"Client of id {idClient} does not exist");
        }

        if (await data.ClientTrips.AnyAsync(ct => ct.IdClient == client.IdClient))
        {
            throw new ConflictException($"Client of id {idClient} have trip booked");
        }

        var transaction = await data.Database.BeginTransactionAsync();

        try
        {
            data.Clients.Remove(client);
            await data.SaveChangesAsync();
            await transaction.CommitAsync();
            return idClient;
        }
        catch (Exception)
        {
            await transaction.RollbackAsync();
            throw;
        }
    }

    public async Task<ClientTripGetDTO> AssignClientToTripWithOptionalClientCreationAsync(
        ClientAssignmentDTO clientAssignment, int idTrip)
    {
        var trip = await data.Trips.FirstOrDefaultAsync(t => t.IdTrip == idTrip);
        
        if (trip == null)
        {
            throw new NotFoundException($"Trip of id {idTrip} does not exist");
        }

        if (trip.DateFrom > DateTime.Now)
        {
            throw new ConflictException($"Trip already begun in {trip.DateFrom}");
        }

        if (data.ClientTrips.Select(ct => ct.IdTrip == idTrip).Count() >= trip.MaxPeople)
        {
            throw new ConflictException($"Trip is fully booked");
        }
        
        var client = await data.Clients.FirstOrDefaultAsync(c => c.Pesel == clientAssignment.Pesel);

        if (client != null)
        {
            if (client.FirstName != clientAssignment.FirstName || client.LastName != clientAssignment.LastName ||
                client.Telephone != clientAssignment.Telephone || client.Email != clientAssignment.Email)
            {
                throw new ConflictException($"Client data does not match with client in database");    
            }

            if (await data.ClientTrips.AnyAsync(ct => ct.IdClient == client.IdClient && ct.IdTrip == idTrip))
            {
                throw new ConflictException($"Client of id {client.IdClient} has already booked trip of id {idTrip}");
            }
        }

        var transaction = await data.Database.BeginTransactionAsync();

        try
        {
            if (client == null)
            {
                client = new Client
                {
                    FirstName = clientAssignment.FirstName,
                    LastName = clientAssignment.LastName,
                    Telephone = clientAssignment.Telephone,
                    Pesel = clientAssignment.Pesel,
                    Email = clientAssignment.Email
                };

                await data.Clients.AddAsync(client);
                await data.SaveChangesAsync();
            }

            var clientTrip = new ClientTrip
            {
                IdClient = client.IdClient,
                IdClientNavigation = client,
                IdTrip = idTrip,
                IdTripNavigation = trip,
                PaymentDate = clientAssignment.PaymentDate,
                RegisteredAt = DateTime.Now
            };

            await data.ClientTrips.AddAsync(clientTrip);
            await data.SaveChangesAsync();
            await transaction.CommitAsync();
            return new ClientTripGetDTO
            {
                IdClient = clientTrip.IdClient,
                IdTrip = clientTrip.IdTrip,
                PaymentDate = clientTrip.PaymentDate,
                RegisteredAt = clientTrip.RegisteredAt
            };
        }
        catch (Exception)
        {
            await transaction.RollbackAsync();
            throw;
        }
        
    }
}