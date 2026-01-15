using Microsoft.AspNetCore.Components;

namespace PeppolWasm.Pages;

public partial class Home
{
  [Parameter]
  public RenderFragment? Body { get; set; }  
}