using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.JSInterop;
using System.Text.Json;
using System.Globalization;
using CsvHelper;
using CsvHelper.Configuration.Attributes;
using Radzen;
using Radzen.Blazor;
using Microsoft.AspNetCore.Components.Web;
using FireFitBlazor.Components;

namespace FireFitBlazor.Application
{
    public partial class WeightInputDialog : ComponentBase
    {
[Parameter] public decimal? Weight { get; set; }

    private void Save()
    {
        if (Weight.HasValue && Weight.Value > 0)
            DialogService.Close(Weight.Value);
    }
    }
}
