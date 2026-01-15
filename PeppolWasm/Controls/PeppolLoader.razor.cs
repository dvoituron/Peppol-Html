using Microsoft.AspNetCore.Components;
using Microsoft.FluentUI.AspNetCore.Components;
using Microsoft.JSInterop;

namespace PeppolWasm.Controls;

public partial class PeppolLoader
{
  private FluentInputFileEventArgs[] Files = [];

  [Inject]
  private IJSRuntime JSRuntime { get; set; } = default!;

  private async Task OnCompletedAsync(IEnumerable<FluentInputFileEventArgs> files)
  {
    var file = files.FirstOrDefault();

    if (file?.LocalFile != null)
    {
      // Read the XML content
      var xmlContent = await File.ReadAllTextAsync(file.LocalFile.FullName);
      
      // Add the stylesheet reference after the XML declaration
      const string xmlDeclaration = "<?xml version=\"1.0\" encoding=\"UTF-8\"?>";
      const string stylesheetRef = "<?xml-stylesheet type=\"text/xsl\" href=\"render-billing-3.xsl\"?>";
      
      if (xmlContent.Contains(xmlDeclaration))
      {
        xmlContent = xmlContent.Replace(xmlDeclaration, $"{xmlDeclaration}\n{stylesheetRef}");
      }
      else
      {
        // If no XML declaration, add both at the beginning
        xmlContent = $"{xmlDeclaration}\n{stylesheetRef}\n{xmlContent}";
      }

      // Store the content and navigate to the converted page
      await JSRuntime.InvokeVoidAsync("renderXmlContent", xmlContent);
    }      
  }
}