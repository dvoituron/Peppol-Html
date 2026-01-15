# Peppol-Html

A Blazor WebAssembly application for viewing and printing Peppol BIS Billing 3.0 electronic invoices and credit notes.

## Overview

Peppol-Html is a client-side web application that renders UBL 2.1 XML invoices and credit notes into a human-readable HTML format using XSLT transformations. The application runs entirely in the browser using WebAssembly, ensuring privacy as no data is sent to any server.

## Features

- 📄 **Open XML Files** - Load Peppol BIS EN16931 invoices and credit notes (UBL 2.1 format)
- 🖨️ **Print Support** - Print rendered invoices directly from the browser
- 🌍 **Multi-language Support** - Supports multiple languages including English, Swedish, Polish, Icelandic, and Serbian
- 🔒 **Privacy-First** - All processing happens locally in your browser
- ⚡ **No Installation Required** - Runs directly in any modern web browser

## Technology Stack

- [.NET 10](https://dotnet.microsoft.com/) - Runtime framework
- [Blazor WebAssembly](https://dotnet.microsoft.com/apps/aspnet/web-apps/blazor) - Client-side web framework
- [Fluent UI Blazor](https://www.fluentui-blazor.net/) - Microsoft Fluent Design components
- XSLT 1.0 - XML transformation for rendering invoices

## Getting Started

### Prerequisites

- [.NET 10 SDK](https://dotnet.microsoft.com/download/dotnet/10.0) or later

### Running Locally

1. Clone the repository:
   ```bash
   git clone https://github.com/your-username/Peppol-Html.git
   cd Peppol-Html
   ```

2. Navigate to the project folder:
   ```bash
   cd PeppolWasm
   ```

3. Run the application:
   ```bash
   dotnet run
   ```

4. Open your browser and navigate to `https://localhost:5001` (or the URL shown in the terminal)

### Building for Production

```bash
dotnet publish -c Release
```

The published files will be in `PeppolWasm/bin/publish/wwwroot/`.

## Usage

1. Click the **Ouvrir** (Open) button to select a Peppol XML invoice or credit note file
2. The document will be rendered in a human-readable format
3. Click the **Imprimer** (Print) button to print the rendered document

## Supported Document Types

- **Invoices** - UBL 2.1 Invoice documents conforming to Peppol BIS Billing 3.0
- **Credit Notes** - UBL 2.1 Credit Note documents conforming to Peppol BIS Billing 3.0

## Project Structure

```
Peppol-Html/
├── PeppolWasm/                    # Blazor WebAssembly project
│   ├── Pages/                     # Razor pages
│   ├── Layout/                    # Layout components
│   └── wwwroot/                   # Static assets
│       ├── render-billing-3.xsl   # Main XSLT stylesheet
│       ├── billing-3/             # Billing-specific templates and translations
│       ├── common/                # Common code lists and translations
│       └── procurement/           # Procurement-related files
└── README.md
```

## XSLT Credits

The XSLT stylesheets are based on [UniStyles](https://github.com/nickyuy/UBL-Invoice-XSLT) by Unimaze Software, licensed under the Apache License 2.0.

## License

This project is licensed under the MIT License - see the [LICENSE](LICENSE) file for details.

## Author

Denis Voituron

## Contributing

Contributions are welcome! Please feel free to submit a Pull Request.
