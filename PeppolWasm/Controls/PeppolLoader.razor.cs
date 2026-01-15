using Microsoft.FluentUI.AspNetCore.Components;

namespace PeppolWasm.Controls;

public partial class PeppolLoader
{
  int ProgressPercent = 0;
  FluentInputFileEventArgs[] Files = [];

  private async Task OnCompletedAsync(IEnumerable<FluentInputFileEventArgs> files)
  {
    var file = files.FirstOrDefault();
    ProgressPercent = 0;    
  }
}