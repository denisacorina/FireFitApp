using System.Collections.Generic;
using System.Threading.Tasks;
using FireFitBlazor.Domain.Models;

    public interface IGetBarcodeProductListContext
    {
        Task<IEnumerable<BarcodeProduct>> Execute();
        Task<IEnumerable<BarcodeProduct>> ExecuteByCategory(string category);
    }
