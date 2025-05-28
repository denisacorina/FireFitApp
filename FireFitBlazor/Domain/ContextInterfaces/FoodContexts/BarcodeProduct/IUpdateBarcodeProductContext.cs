using System.Threading.Tasks;
using FireFitBlazor.Domain.Models;


    public interface IUpdateBarcodeProductContext
    {
        Task<bool> Execute(BarcodeProduct product);
    }
