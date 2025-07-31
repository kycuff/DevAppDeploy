using DevAppDeploy.Data.Enums;
using DevAppDeploy.Models;
using DevAppDeploy.Services.Application;
using Microsoft.AspNetCore.Components;
using MudBlazor;

namespace DevAppDeploy.Components.Pages
{
    public partial class AddAppDialog
    {
        [CascadingParameter]
        public required IMudDialogInstance MudDialog { get; set; }

        public required string appName;
        public required string operatingSystem;
        public required string environment;

        private List<string>? operatingSystems;
        private List<string>? environments;

        [Inject]
        public IApplicationService ApplicationService { get; set; }

        protected override void OnInitialized()
        {
            operatingSystems = Enum.GetNames(typeof(OperatingSystemEnum)).ToList();
            environments = Enum.GetNames(typeof(EnvironmentEnum)).ToList();
        }

        private async Task Submit()
        {
            var model = new CreateApplicationModel
            {
                AppName = appName,
                OperatingSystem = Enum.Parse<OperatingSystemEnum>(operatingSystem),
                Environments = Enum.Parse<EnvironmentEnum>(environment)
            };

            await ApplicationService.CreateApplicationAsync(model);
            MudDialog.Close(DialogResult.Ok(true));
        }

        private void Cancel() => MudDialog.Cancel();
    }
}