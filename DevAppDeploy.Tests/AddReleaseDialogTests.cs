using DevAppDeploy.Components.Pages;
using DevAppDeploy.Models;
using DevAppDeploy.Services.Application;
using DevAppDeploy.Services.Blob;
using Microsoft.AspNetCore.Components.Forms;
using MudBlazor;
using NSubstitute;

namespace DevAppDeploy.Tests;
public class AddReleaseDialogTests
{
    [Fact]
    public async Task Submit_ValidFormAndApkFile_CreatesReleaseAndClosesDialog()
    {
        // Arrange
        var appService = Substitute.For<IApplicationService>();
        var blobService = Substitute.For<IBlobStorageService>();
        var snackbar = Substitute.For<ISnackbar>();
        var dialog = Substitute.For<IMudDialogInstance>();

        var component = new AddReleaseDialog
        {
            ApplicationService = appService,
            BlobStorageService = blobService,
            Snackbar = snackbar,
            MudDialog = dialog,
            ApplicationId = 1
        };

        // Set private fields using reflection
        var form = Substitute.For<MudForm>();
        //form.IsValid.Returns(true);
        typeof(AddReleaseDialog)
            .GetField("form", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
            .SetValue(component, form);

        var file = Substitute.For<IBrowserFile>();
        file.Name.Returns("test.apk");
        var files = new List<IBrowserFile> { file };
        typeof(AddReleaseDialog)
            .GetField("_model", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
            .SetValue(component, new AddReleaseDialog.FileUploadModel { Files = files });

        blobService.UploadFileAsync(file, "releases").Returns(Task.FromResult("http://blob/file.apk"));

        // Act
        var submitMethod = typeof(AddReleaseDialog)
            .GetMethod("Submit", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        var task = (Task)submitMethod.Invoke(component, null);
        await task;

        // Assert
        await blobService.Received(1).UploadFileAsync(file, "releases");
        await appService.Received(1).CreateReleaseAsync(Arg.Any<CreateReleaseModel>());
        dialog.Received(1).Close(Arg.Any<DialogResult>());
    }

    [Fact]
    public async Task Submit_InvalidForm_DoesNotCreateReleaseOrCloseDialog()
    {
        var appService = Substitute.For<IApplicationService>();
        var blobService = Substitute.For<IBlobStorageService>();
        var snackbar = Substitute.For<ISnackbar>();
        var dialog = Substitute.For<IMudDialogInstance>();

        var component = new AddReleaseDialog
        {
            ApplicationService = appService,
            BlobStorageService = blobService,
            Snackbar = snackbar,
            MudDialog = dialog,
            ApplicationId = 1
        };

        // Use a real MudForm instance to avoid null reference
        var form = new MudForm();
        typeof(AddReleaseDialog)
            .GetField("form", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
            .SetValue(component, form);

        var submitMethod = typeof(AddReleaseDialog)
            .GetMethod("Submit", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        var task = (Task)submitMethod.Invoke(component, null);
        await task;

        await blobService.DidNotReceive().UploadFileAsync(Arg.Any<IBrowserFile>(), Arg.Any<string>());
        await appService.DidNotReceive().CreateReleaseAsync(Arg.Any<CreateReleaseModel>());
        dialog.DidNotReceive().Close(Arg.Any<DialogResult>());
    }

    [Fact]
    public async Task Submit_NonApkFile_ShowsErrorAndDoesNotCreateReleaseOrCloseDialog()
    {
        // Arrange
        var appService = Substitute.For<IApplicationService>();
        var blobService = Substitute.For<IBlobStorageService>();
        var snackbar = Substitute.For<ISnackbar>();
        var dialog = Substitute.For<IMudDialogInstance>();

        var component = new AddReleaseDialog
        {
            ApplicationService = appService,
            BlobStorageService = blobService,
            Snackbar = snackbar,
            MudDialog = dialog,
            ApplicationId = 1
        };

        // Set up a valid form (using a real instance)
        var form = new MudForm();
        typeof(AddReleaseDialog)
            .GetField("form", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
            .SetValue(component, form);

        // Set up a non-APK file
        var file = Substitute.For<IBrowserFile>();
        file.Name.Returns("not-an-apk.txt");
        var files = new List<IBrowserFile> { file };
        typeof(AddReleaseDialog)
            .GetField("_model", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
            .SetValue(component, new AddReleaseDialog.FileUploadModel { Files = files });

        // Act
        var submitMethod = typeof(AddReleaseDialog)
            .GetMethod("Submit", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        var task = (Task)submitMethod.Invoke(component, null);
        await task;

        // Assert
        await blobService.DidNotReceive().UploadFileAsync(Arg.Any<IBrowserFile>(), Arg.Any<string>());
        await appService.DidNotReceive().CreateReleaseAsync(Arg.Any<CreateReleaseModel>());
        dialog.DidNotReceive().Close(Arg.Any<DialogResult>());
        snackbar.Received().Add(Arg.Is<string>(msg => msg.Contains("APK", StringComparison.OrdinalIgnoreCase)), Severity.Error);
    }

    [Fact]
    public async Task Submit_NoFileSelected_ShowsErrorAndDoesNotCreateReleaseOrCloseDialog()
    {
        // Arrange
        var appService = Substitute.For<IApplicationService>();
        var blobService = Substitute.For<IBlobStorageService>();
        var snackbar = Substitute.For<ISnackbar>();
        var dialog = Substitute.For<IMudDialogInstance>();

        var component = new AddReleaseDialog
        {
            ApplicationService = appService,
            BlobStorageService = blobService,
            Snackbar = snackbar,
            MudDialog = dialog,
            ApplicationId = 1
        };

        // Set up a valid form (using a real instance)
        var form = new MudForm();
        typeof(AddReleaseDialog)
            .GetField("form", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
            .SetValue(component, form);

        // No files selected
        typeof(AddReleaseDialog)
            .GetField("_model", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
            .SetValue(component, new AddReleaseDialog.FileUploadModel { Files = new List<IBrowserFile>() });

        // Act
        var submitMethod = typeof(AddReleaseDialog)
            .GetMethod("Submit", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        var task = (Task)submitMethod.Invoke(component, null);
        await task;

        // Assert
        await blobService.DidNotReceive().UploadFileAsync(Arg.Any<IBrowserFile>(), Arg.Any<string>());
        await appService.DidNotReceive().CreateReleaseAsync(Arg.Any<CreateReleaseModel>());
        dialog.DidNotReceive().Close(Arg.Any<DialogResult>());
        snackbar.Received().Add(Arg.Is<string>(msg => msg.Contains("file", StringComparison.OrdinalIgnoreCase)), Severity.Error);
    }

    [Fact]
    public async Task Submit_ApplicationServiceThrows_ShowsErrorAndDoesNotCloseDialog()
    {
        // Arrange
        var appService = Substitute.For<IApplicationService>();
        var blobService = Substitute.For<IBlobStorageService>();
        var snackbar = Substitute.For<ISnackbar>();
        var dialog = Substitute.For<IMudDialogInstance>();

        var component = new AddReleaseDialog
        {
            ApplicationService = appService,
            BlobStorageService = blobService,
            Snackbar = snackbar,
            MudDialog = dialog,
            ApplicationId = 1
        };

        // Set up a valid form (using a real instance)
        var form = new MudForm();
        typeof(AddReleaseDialog)
            .GetField("form", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
            .SetValue(component, form);

        // Set up a valid APK file
        var file = Substitute.For<IBrowserFile>();
        file.Name.Returns("test.apk");
        var files = new List<IBrowserFile> { file };
        typeof(AddReleaseDialog)
            .GetField("_model", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
            .SetValue(component, new AddReleaseDialog.FileUploadModel { Files = files });

        blobService.UploadFileAsync(file, "releases").Returns(Task.FromResult("http://blob/file.apk"));
        appService.CreateReleaseAsync(Arg.Any<CreateReleaseModel>()).Returns<Task>(x => throw new Exception("Create failed"));

        // Act
        var submitMethod = typeof(AddReleaseDialog)
            .GetMethod("Submit", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        var task = (Task)submitMethod.Invoke(component, null);
        await task;

        // Assert
        await blobService.Received(1).UploadFileAsync(file, "releases");
        await appService.Received(1).CreateReleaseAsync(Arg.Any<CreateReleaseModel>());
        dialog.DidNotReceive().Close(Arg.Any<DialogResult>());
        snackbar.Received().Add(Arg.Is<string>(msg => msg.Contains("Create failed", StringComparison.OrdinalIgnoreCase)), Severity.Error);
    }

}

