using FireFit.Shared.DTOs;

namespace FireFit.Shared.Contracts;

public interface IProfileService
{
    Task<UserDto?> GetAsync(CancellationToken ct = default);
    Task UpdateAsync(ProfileUpdateDto dto, CancellationToken ct = default);
    Task<string?> UploadPhotoAsync(Stream stream, string fileName, CancellationToken ct = default);
}
