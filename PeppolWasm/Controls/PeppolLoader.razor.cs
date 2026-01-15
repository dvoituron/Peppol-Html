using Microsoft.AspNetCore.Components;
using Microsoft.FluentUI.AspNetCore.Components;
using Microsoft.JSInterop;

namespace PeppolWasm.Controls;

public partial class PeppolLoader
{
  [Inject]
  private IJSRuntime JSRuntime { get; set; } = default!;

  private int Percentage { get; set; }

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
}