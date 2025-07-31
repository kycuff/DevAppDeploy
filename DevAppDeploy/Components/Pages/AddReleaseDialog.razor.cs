using DevAppDeploy.Models;
using DevAppDeploy.Services.Application;
using DevAppDeploy.Services.Blob;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using MudBlazor;

namespace DevAppDeploy.Components.Pages
{
    public partial class AddReleaseDialog
    {
        [CascadingParameter]
        public required IMudDialogInstance MudDialog { get; set; }

        [Parameter]
        public int ApplicationId { get; set; }

        [Inject]
        public IApplicationService ApplicationService { get; set; }

        [Inject]
        public ISnackbar Snackbar { get; set; }

        [Inject]
        public IBlobStorageService BlobStorageService { get; set; }

        private MudForm form;
        private string version;
        private DateTime releaseDate = DateTime.UtcNow;
        private string unique;

        private FileUploadModel _model = new();
        private MudFileUpload<IReadOnlyList<IBrowserFile>> _fileUpload;
        private bool _isValid;
        private bool _isTouched;
        private string _dragClass = "";

        public class FileUploadModel
        {
            public IReadOnlyList<IBrowserFile> Files { get; set; }
        }

        private void SetDragClass()
        {
            _dragClass = "mud-elevation-6 mud-border-primary";
        }

        private void ClearDragClass()
        {
            _dragClass = "";
        }

        private async Task OpenFilePickerAsync()
        {
            if (_fileUpload != null)
                await _fileUpload.OpenFilePickerAsync();
        }

        private async Task ClearAsync()
        {
            if (_fileUpload != null)
                await _fileUpload.ClearAsync();
        }

        private async Task Submit()
        {
            try
            {
                // Validate the form
                await form.Validate();
                if (!form.IsValid)
                {
                    Snackbar.Add("Please correct the errors in the form before submitting.", Severity.Error);
                    return;
                }

                if (_model.Files is null || !_model.Files.Any())
                {
                    Snackbar.Add("File must be uploaded.", Severity.Error);
                    return;
                }

                var file = _model.Files.First();
                if (!file.Name.EndsWith(".apk"))
                {
                    Snackbar.Add("File is not a valid .apk formatted file.", Severity.Error);
                    _model.Files = [];
                    return;
                }

                string fileUrl = await BlobStorageService.UploadFileAsync(file, "releases");
                Snackbar.Add("Files uploaded successfully!", Severity.Success);

                var releaseModel = new CreateReleaseModel
                {
                    Version = version,
                    ReleaseDate = releaseDate,
                    Unique = unique,
                    ApplicationId = ApplicationId,
                    FileURL = fileUrl
                };

                await ApplicationService.CreateReleaseAsync(releaseModel);

                MudDialog.Close(DialogResult.Ok(true));
            }
            catch (Exception ex)
            {
                Snackbar.Add(ex.Message, Severity.Error);
            }
        }

        private void Cancel() => MudDialog.Cancel();
    }
}