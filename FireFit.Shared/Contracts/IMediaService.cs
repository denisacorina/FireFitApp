using System.IO;

namespace FireFit.Shared.Contracts;

public interface IMediaService
{
    Task<Stream?> CapturePhotoAsync(CancellationToken ct = default);
    Task<Stream?> PickPhotoAsync(CancellationToken ct = default);
}

