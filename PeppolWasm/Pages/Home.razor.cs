using Microsoft.AspNetCore.Components;
using Microsoft.FluentUI.AspNetCore.Components;
using Microsoft.JSInterop;

namespace PeppolWasm.Pages;

public partial class Home
{
  [Inject]
  private IJSRuntime JSRuntime { get; set; } = default!;

  [Inject]
  public IDialogService DialogService { get; set; } = default!;

  private int Percentage { get; set; }

  protected override async Task OnInitializedAsync()
  {
    await DialogService.RegisterInputFileAsync("UploadButton", OnCompletedAsync, options => { /* ... */ });
  }

  protected override async Task OnAfterRenderAsync(bool firstRender)
  {
    if (firstRender)
    {
      await JSRuntime.InvokeVoidAsync("attachPrintButtonHandler");
    }
  }


  private async Task OnCompletedAsync(IEnumerable<FluentInputFileEventArgs> files)
  {
    var file = files.FirstOrDefault();

    if (file?.LocalFile != null)
    {
      // Read the XML content
      var xmlContent = await File.ReadAllTextAsync(file.LocalFile.FullName);

      // Apply XSLT transformation and render in iframe
      await JSRuntime.InvokeVoidAsync("renderXmlContent", xmlContent);
    }

    Percentage = 0;
  }

  public async ValueTask DisposeAsync()
  {
    await DialogService.UnregisterInputFileAsync("UploadButton");
  }
}