#nullable enable
using FireFit.Shared.Contracts;
using Microsoft.Maui.ApplicationModel;
using Microsoft.Maui.Media;

namespace FireFit.Maui.Services;

public class MauiMediaService : IMediaService
{
    public async Task<Stream?> CapturePhotoAsync(CancellationToken ct = default)
    {
        if (!MediaPicker.Default.IsCaptureSupported) return null;
        var photo = await MediaPicker.Default.CapturePhotoAsync(new MediaPickerOptions { Title = "Capture meal" });
        if (photo is null) return null;
        return await photo.OpenReadAsync();
    }

    public async Task<Stream?> PickPhotoAsync(CancellationToken ct = default)
    {
        var photo = await MediaPicker.Default.PickPhotoAsync();
        if (photo is null) return null;
        return await photo.OpenReadAsync();
    }
}
