using DevAppDeploy.Models;
using DevAppDeploy.Services.Application;
using Microsoft.AspNetCore.Components;
using MudBlazor;

namespace DevAppDeploy.Components.Pages
{
    public partial class Applications
    {
        [Inject] private IDialogService DialogService { get; set; }
        [Inject] private IApplicationService ApplicationService { get; set; }
        [Inject] private NavigationManager Navigation { get; set; }

        private List<DisplayApplicationModel> applications;

        protected override async Task OnInitializedAsync()
        {
            applications = await ApplicationService.GetApplicationsAsync();
        }

        private async Task OpenDialogAsync()
        {
            var options = new DialogOptions { CloseOnEscapeKey = true };

            var dialog = DialogService.Show<AddAppDialog>("Add Application", options);
            var result = await dialog.Result;

            if (!result.Canceled)
            {
                applications = await ApplicationService.GetApplicationsAsync();
            }
        }

        private void OnRowClick(TableRowClickEventArgs<DisplayApplicationModel> args)
        {
            var clickedApplication = args.Item;

            if (clickedApplication is not null)
            {
                Navigation.NavigateTo($"/releases/{clickedApplication.Id}");
            }
        }
    }
}