using Radzen;
using Radzen.Blazor;
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

namespace FireFit.UI.Shared.Components
{
    public partial class CircularProgressBar : ComponentBase
    {
[Parameter] public int StrokeBottom { get; set; } = 5;
    [Parameter] public int TargetPercent { get; set; } = 75;
    [Parameter] public string ColorSlice { get; set; } = "#EC407A";
    [Parameter] public string ColorCircle { get; set; } = "#f1f1f1";
    [Parameter] public bool Round { get; set; } = true;
    [Parameter] public int Size { get; set; } = 120;
    private string ProgressStyle => $"--percent: {TargetPercent}";
    private ElementReference ProgressRef;
    private DotNetObjectReference<CircularProgressBar> ObjectReference;
    private int CurrentPercent { get; set; } = 75;
    private double CircleCircumference => 2 * Math.PI * (Size / 2 - 10);

    private double CalculateOffset => CircleCircumference - (CircleCircumference * CurrentPercent) / 100;

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
        {
            ObjectReference = DotNetObjectReference.Create(this);
            await JSRuntime.InvokeVoidAsync("initProgressBar", ProgressRef, ObjectReference, CurrentPercent);
        }
    }

    [JSInvokable]
    public void UpdateProgress(int newPercent)
    {
        try
        {
            CurrentPercent = newPercent;
            StateHasChanged();
        }
        catch(Exception e){
            Console.WriteLine(e.Message);
        }
    }

    public async ValueTask DisposeAsync()
    {
        if (ObjectReference != null)
        {
            ObjectReference.Dispose();
        }
    }
    }
}

