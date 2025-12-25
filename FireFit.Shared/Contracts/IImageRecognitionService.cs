using FireFit.Shared.DTOs;

namespace FireFit.Shared.Contracts;

public interface IImageRecognitionService
{
    Task<IReadOnlyList<IngredientDto>> DetectIngredientsAsync(Stream imageStream, CancellationToken ct = default);
}

