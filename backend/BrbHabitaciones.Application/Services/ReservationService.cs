using BrbHabitaciones.Application.DTOs.Reservations;
using BrbHabitaciones.Application.Interfaces;
using BrbHabitaciones.Domain.Entities;
using BrbHabitaciones.Domain.Enums;

namespace BrbHabitaciones.Application.Services;

public class ReservationService(
    IReservationRepository repo,
    IRoomRepository roomRepo) : IReservationService
{
    public Task<IEnumerable<DateOnly>> GetBookedDatesAsync(Guid roomId, DateOnly from, DateOnly to) =>
        repo.GetBookedDatesAsync(roomId, from, to);

    public async Task<ReservationDto> CreateAsync(CreateReservationRequest request, Guid clientId)
    {
        var today = DateOnly.FromDateTime(DateTime.UtcNow);

        if (request.CheckInDate < today)
            throw new InvalidOperationException("La fecha de entrada no puede ser en el pasado.");

        if (request.CheckOutDate <= request.CheckInDate)
            throw new InvalidOperationException("La fecha de salida debe ser posterior a la de entrada.");

        var nights = request.CheckOutDate.DayNumber - request.CheckInDate.DayNumber;

        var room = await roomRepo.GetByIdAsync(request.RoomId)
            ?? throw new KeyNotFoundException("Habitación no encontrada.");

        if (!room.IsActive)
            throw new InvalidOperationException("Esta habitación no está disponible.");

        if (request.GuestCount < 1 || request.GuestCount > room.Capacity)
            throw new InvalidOperationException($"Capacidad máxima: {room.Capacity} huéspedes.");

        var hasOverlap = await repo.HasOverlapAsync(request.RoomId, request.CheckInDate, request.CheckOutDate);
        if (hasOverlap)
            throw new InvalidOperationException("Las fechas seleccionadas no están disponibles. Elegí otras fechas.");

        var reservation = new Reservation
        {
            RoomId = request.RoomId,
            ClientId = clientId,
            CheckInDate = request.CheckInDate,
            CheckOutDate = request.CheckOutDate,
            GuestCount = request.GuestCount,
            TotalPrice = nights * room.PricePerNight,
            Status = ReservationStatus.Confirmada,
        };

        // Block each date in [checkIn, checkOut)
        for (var i = 0; i < nights; i++)
            reservation.Availability.Add(new Availability
            {
                RoomId = request.RoomId,
                Date = request.CheckInDate.AddDays(i),
            });

        await repo.AddAsync(reservation);
        await repo.SaveChangesAsync();

        var full = await repo.GetByIdAsync(reservation.Id);
        return MapToDto(full!);
    }

    public async Task<IEnumerable<ReservationDto>> GetMyAsync(Guid userId, string role)
    {
        var reservations = role is "DuenoAlojamiento" or "Administrador"
            ? await repo.GetByOwnerAsync(userId)
            : await repo.GetByClientAsync(userId);
        return reservations.Select(MapToDto);
    }

    public async Task<ReservationDto?> GetByIdAsync(Guid id)
    {
        var r = await repo.GetByIdAsync(id);
        return r is null ? null : MapToDto(r);
    }

    public async Task<ReservationDto> CancelAsync(Guid id, Guid requesterId, string? reason = null)
    {
        var r = await repo.GetByIdAsync(id)
            ?? throw new KeyNotFoundException("Reserva no encontrada.");

        if (r.Status == ReservationStatus.Cancelada)
            throw new InvalidOperationException("La reserva ya está cancelada.");

        var isClient = r.ClientId == requesterId;
        var isOwner = r.Room.Property.OwnerId == requesterId;

        if (!isClient && !isOwner)
            throw new UnauthorizedAccessException("No tenés permiso para cancelar esta reserva.");

        r.Status = ReservationStatus.Cancelada;
        r.CancellationReason = reason;
        r.UpdatedAt = DateTime.UtcNow;

        // Free up the blocked dates
        r.Availability.Clear();

        await repo.SaveChangesAsync();

        return MapToDto(r);
    }

    private static ReservationDto MapToDto(Reservation r) => new(
        r.Id,
        r.RoomId,
        r.Room.Title,
        r.Room.PropertyId,
        r.Room.Property.Name,
        r.Room.Property.City,
        r.ClientId,
        $"{r.Client.FirstName} {r.Client.LastName}",
        r.CheckInDate,
        r.CheckOutDate,
        r.GuestCount,
        r.CheckOutDate.DayNumber - r.CheckInDate.DayNumber,
        r.TotalPrice,
        r.Status.ToString(),
        r.CancellationReason,
        r.CreatedAt);
}
