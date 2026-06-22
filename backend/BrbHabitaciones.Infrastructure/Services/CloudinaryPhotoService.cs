using BrbHabitaciones.Application.DTOs.Photos;
using BrbHabitaciones.Application.Interfaces;
using BrbHabitaciones.Domain.Entities;
using BrbHabitaciones.Infrastructure.Data;
using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace BrbHabitaciones.Infrastructure.Services;

public class CloudinaryPhotoService : IPhotoService
{
    private readonly Cloudinary _cloudinary;
    private readonly AppDbContext _db;

    public CloudinaryPhotoService(IConfiguration config, AppDbContext db)
    {
        _db = db;
        var account = new Account(
            config["Cloudinary:CloudName"],
            config["Cloudinary:ApiKey"],
            config["Cloudinary:ApiSecret"]);
        _cloudinary = new Cloudinary(account) { Api = { Secure = true } };
    }

    public async Task<PhotoDto> UploadToRoomAsync(Stream fileStream, string fileName, Guid roomId, Guid requesterId)
    {
        var room = await _db.Rooms
            .Include(r => r.Property)
            .FirstOrDefaultAsync(r => r.Id == roomId && r.DeletedAt == null)
            ?? throw new KeyNotFoundException("Habitación no encontrada.");

        if (room.Property.OwnerId != requesterId)
            throw new UnauthorizedAccessException("No tenés permiso para subir fotos a esta habitación.");

        return await UploadAndSaveAsync(fileStream, fileName, roomId: roomId, propertyId: null);
    }

    public async Task<PhotoDto> UploadToPropertyAsync(Stream fileStream, string fileName, Guid propertyId, Guid requesterId)
    {
        var property = await _db.Properties
            .FirstOrDefaultAsync(p => p.Id == propertyId && p.DeletedAt == null)
            ?? throw new KeyNotFoundException("Propiedad no encontrada.");

        if (property.OwnerId != requesterId)
            throw new UnauthorizedAccessException("No tenés permiso para subir fotos a esta propiedad.");

        return await UploadAndSaveAsync(fileStream, fileName, roomId: null, propertyId: propertyId);
    }

    public async Task DeleteAsync(Guid photoId, Guid requesterId)
    {
        var photo = await _db.Photos
            .Include(ph => ph.Room).ThenInclude(r => r!.Property)
            .Include(ph => ph.Property)
            .FirstOrDefaultAsync(ph => ph.Id == photoId)
            ?? throw new KeyNotFoundException("Foto no encontrada.");

        var ownerId = photo.Room?.Property?.OwnerId ?? photo.Property?.OwnerId;
        if (ownerId != requesterId)
            throw new UnauthorizedAccessException("No tenés permiso para eliminar esta foto.");

        if (!string.IsNullOrEmpty(photo.PublicId))
            await _cloudinary.DestroyAsync(new DeletionParams(photo.PublicId));

        _db.Photos.Remove(photo);
        await _db.SaveChangesAsync();
    }

    private async Task<PhotoDto> UploadAndSaveAsync(Stream fileStream, string fileName, Guid? roomId, Guid? propertyId)
    {
        var uploadParams = new ImageUploadParams
        {
            File = new FileDescription(fileName, fileStream),
            Folder = "brb-habitaciones",
            Transformation = new Transformation().Width(1200).Height(800).Crop("fill").Quality("auto:good")
        };

        var result = await _cloudinary.UploadAsync(uploadParams);
        if (result.Error is not null)
            throw new InvalidOperationException($"Error al subir imagen: {result.Error.Message}");

        var photo = new Photo
        {
            RoomId = roomId,
            PropertyId = propertyId,
            Url = result.SecureUrl.ToString(),
            PublicId = result.PublicId,
        };

        _db.Photos.Add(photo);
        await _db.SaveChangesAsync();

        return new PhotoDto(photo.Id, photo.Url, photo.PublicId, photo.IsCover, photo.DisplayOrder);
    }
}
