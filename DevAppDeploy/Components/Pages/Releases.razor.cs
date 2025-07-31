using DevAppDeploy.Models;
using DevAppDeploy.Services.Application;
using DevAppDeploy.Services.Blob;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.JSInterop;
using MudBlazor;

namespace DevAppDeploy.Components.Pages
{
    public partial class Releases
    {
        [Parameter]
        public int ApplicationId { get; set; }
        [Inject] private AuthenticationStateProvider AuthenticationStateProvider { get; set; }
        [Inject] private IAuthorizationService AuthorizationService { get; set; }
        [Inject] private IDialogService DialogService { get; set; }
        [Inject] private IApplicationService ApplicationService { get; set; } = default!;
        [Inject] private IBlobStorageService BlobStorageService { get; set; }
        [Inject] private IJSRuntime JS { get; set; }

        private List<DisplayReleaseModel> releases; 
        private DisplayReleaseModel? _selectedRelease; 
        private DisplayApplicationModel? _application;
        private bool _open = false;
        private Anchor _anchor = Anchor.End;
        private bool _overlayAutoClose = true;
        private bool IsDeveloper { get; set; }

        protected override async Task OnInitializedAsync()
        {
            releases = await ApplicationService.GetReleasesByApplicationId(ApplicationId);
            var applications = await ApplicationService.GetApplicationsAsync();
            _application = applications.FirstOrDefault(a => a.Id == ApplicationId);

            var authState = await AuthenticationStateProvider.GetAuthenticationStateAsync();
            var user = authState.User;
            IsDeveloper = user.IsInRole("developer");
        }

        private async Task OpenDialogAsync()
        {
            var parameters = new DialogParameters { ["ApplicationId"] = ApplicationId };
            var options = new DialogOptions { CloseOnEscapeKey = true };

            var dialog = DialogService.Show<AddReleaseDialog>("Add Release", parameters, options);
            var result = await dialog.Result;

            if (!result.Canceled)
            {
                releases = await ApplicationService.GetReleasesByApplicationId(ApplicationId);
            }
        }

        private async Task OnDeleteButtonClicked(int releaseId)
        {
            // Find the release to get its FileURL before deleting
            var release = releases.FirstOrDefault(r => r.Id == releaseId);
            var fileUrl = release?.FileURL;

            // Delete the release from the database
            await ApplicationService.DeleteReleaseByIdAsync(releaseId);

            // Delete the file from blob storage if the URL exists
            if (!string.IsNullOrEmpty(fileUrl))
            {
                await BlobStorageService.DeleteFileByUrlAsync(fileUrl);
            }

            // Refresh the releases list
            releases = await ApplicationService.GetReleasesByApplicationId(ApplicationId);
            StateHasChanged();
        }


        private void OpenDrawer(Anchor anchor, DisplayReleaseModel release)
        {
            _selectedRelease = release;
            _anchor = anchor;
            _open = true;
        }

        private async Task DownloadReleaseAsync()
        {
            if (_selectedRelease == null)
                return;

            var releaseUrl = _selectedRelease.FileURL;
            if (releaseUrl == null || string.IsNullOrEmpty(releaseUrl))
                return;

            var fileName = Path.GetFileName(new Uri(releaseUrl).LocalPath);
            using var stream = await BlobStorageService.DownloadFileByUrlAsync(releaseUrl);

            using var ms = new MemoryStream();
            await stream.CopyToAsync(ms);
            var bytes = ms.ToArray();

            await JS.InvokeVoidAsync("downloadFileFromBytes", fileName, Convert.ToBase64String(bytes));
        }

    }
}