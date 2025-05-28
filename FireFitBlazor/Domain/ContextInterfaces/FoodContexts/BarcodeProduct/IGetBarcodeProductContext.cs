using System.Threading.Tasks;
using FireFitBlazor.Domain.Models;


    public interface IGetBarcodeProductContext
    {
        Task<BarcodeProduct> Execute(string barcode);
    }
