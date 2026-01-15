using Microsoft.AspNetCore.Components;

namespace PeppolWasm.Layout;

public partial class Home
{
  [Parameter]
  public RenderFragment? Body { get; set; }  
}