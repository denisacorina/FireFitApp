using System.Threading.Tasks;

namespace FireFitBlazor.Domain.ContextInterfaces.FoodContexts.BarcodeProduct
{
    public interface IDeleteBarcodeProductContext
    {
        Task<bool> Execute(int id);
    }
} 