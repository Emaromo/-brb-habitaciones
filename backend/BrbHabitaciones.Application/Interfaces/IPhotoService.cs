using BrbHabitaciones.Application.DTOs.Photos;

namespace BrbHabitaciones.Application.Interfaces;

public interface IPhotoService
{
    Task<PhotoDto> UploadToRoomAsync(Stream fileStream, string fileName, Guid roomId, Guid requesterId);
    Task<PhotoDto> UploadToPropertyAsync(Stream fileStream, string fileName, Guid propertyId, Guid requesterId);
    Task DeleteAsync(Guid photoId, Guid requesterId);
}
