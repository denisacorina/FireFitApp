using System.Threading.Tasks;
using FireFitBlazor.Domain.Models;


    public interface IAddBarcodeProductContext
    {
        Task<bool> Execute(BarcodeProduct product);
    }
