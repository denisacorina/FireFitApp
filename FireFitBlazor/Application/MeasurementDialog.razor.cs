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
using FireFitBlazor.Domain.Models;
using Radzen.Blazor;
using Radzen;
using FireFitBlazor.Domain.ContextInterfaces.ProgressContexts;
using static Microsoft.AspNetCore.Components.Web.RenderMode;

namespace FireFitBlazor.Application
{
    public partial class MeasurementDialog : ComponentBase
    {
[Parameter] public string? UserId { get; set; }
        [Inject] DialogService DialogService { get; set; } = default!;
    private IUpdateUserProgressContext UpdateUserProgressContext { get; set; } = default!;
    [Inject] IBodyMeasurementContext BodyMeasurementContext { get; set; } = default!;

    public BodyMeasurementResult bodyMeasurementResult = new();

    private void Cancel()
    {
        DialogService.Close();
    }

    private async Task SubmitMeasurement()
    {
        var newMeasurement = BodyMeasurement.Create(
            userId: UserId, 
            weight: bodyMeasurementResult.Weight,
            bodyFatPercentage: bodyMeasurementResult.BodyFatPercentage,
            chest: bodyMeasurementResult.Chest,
            waist: bodyMeasurementResult.Waist,
            hips: bodyMeasurementResult.Hips,
            notes: bodyMeasurementResult.Notes
        );

        var result = await BodyMeasurementContext.AddMeasurement(newMeasurement);

        DialogService.Close(newMeasurement);
    }

    public class BodyMeasurementResult
    {
        public decimal? Weight { get; set; }
        public decimal? BodyFatPercentage { get; set; }
        public decimal? Chest { get; set; }
        public decimal? Waist { get; set; }
        public decimal? Hips { get; set; }
        public decimal? LeftArm { get; set; }
        public decimal? RightArm { get; set; }
        public decimal? LeftThigh { get; set; }
        public decimal? RightThigh { get; set; }
        public decimal? LeftCalf { get; set; }
        public decimal? RightCalf { get; set; }
        public string? Notes { get; set; }
    }
    }
}
