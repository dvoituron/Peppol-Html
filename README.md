# Peppol-Viewer

A .NET console application for converting Peppol UBL Invoice and Credit Note XML documents to human-readable HTML format using [XSLT stylesheets](https://github.com/pellea/unimaze-peppol-stylesheets).

## Overview

Peppol-Viewer transforms Peppol compliant XML invoices and credit notes into styled HTML documents that can be viewed in any web browser. The application uses XSLT transformation with support for multiple languages.

## Requirements

- .NET 10.0 SDK or later
- Windows, Linux, or macOS

## Building the Project

```bash
# Clone or navigate to the project directory
cd Peppol-Viewer

# Build the project
dotnet build
```

## Usage

### Command Line

```bash
# Basic usage - converts XML to HTML
dotnet run -- <path-to-xml-file>

# Or use the compiled executable
Peppol-Viewer.exe <path-to-xml-file>
```

### Example

```bash
# Convert a Peppol invoice XML to HTML
dotnet run -- c8d0172f-9c79-4f81-acf3-666d5e69d942.xml
```

The output HTML file will be saved in the same directory as the input XML file, with a `.html` extension.

## Programmatic Usage

You can also use the `PeppolConverter` class directly in your .NET projects:

```csharp
using Peppol_Viewer;

// Convert XML to HTML file
PeppolConverter.ConvertXmlToHtml(
    xmlFilePath: "invoice.xml",
    xsltFilePath: "Stylesheets/render-billing-3.xsl",
    outputHtmlPath: "invoice.html",
    languageCode: "en"  // Optional: defaults to "en"
);

// Or get HTML as a string
string htmlContent = PeppolConverter.ConvertXmlToHtmlString(
    xmlFilePath: "invoice.xml",
    xsltFilePath: "Stylesheets/render-billing-3.xsl",
    languageCode: "en"
);
```

## Supported Languages

The viewer supports multiple languages for document labels and descriptions:

| Code | Language   |
|------|------------|
| `en` | English    |
| `is` | Icelandic  |
| `pl` | Polish     |
| `se` | Swedish    |
| `sr` | Serbian    |

## Project Structure

```
Peppol-Viewer/
├── Program.cs                    # Main application and PeppolConverter class
├── Peppol-Viewer.csproj          # Project file
├── Peppol-Viewer.sln             # Solution file
├── README.md                     # This file
└── Stylesheets/
    ├── render-billing-3.xsl      # Main XSLT stylesheet for invoices
    ├── billing-3/                # Billing-specific templates and translations
    │   ├── CommonTemplates.xsl   # Shared XSLT templates
    │   ├── Headlines-BT_*.xml    # Headline translations
    │   └── UBLInvoiceBaseType_*.xml  # Invoice type translations
    ├── common/                   # Common code lists and translations
    │   ├── UNCL*.xml             # UN/CEFACT code lists
    │   ├── UBL*.xml              # UBL code lists
    │   └── user_config.xsl       # User configuration
    └── procurement/              # Order-related stylesheets
        └── render-order.xsl      # Order rendering stylesheet
```

## Features

- **Peppol BIS Billing 3.0 Support**: Handles both Invoice and Credit Note document types
- **Multi-language Support**: Localized labels and descriptions in 5 languages
- **Responsive HTML Output**: Generated HTML is styled for readability
- **Code List Resolution**: Automatically resolves UN/CEFACT and UBL code values to human-readable descriptions

## Troubleshooting

### Common Issues

1. **"XSLT file not found"**: Ensure the `Stylesheets` folder is in the correct location relative to the executable or working directory.

2. **"XML file not found"**: Verify the path to your Peppol XML document is correct.

3. **Transformation errors**: Ensure your XML document is a valid Peppol BIS Billing 3.0 Invoice or Credit Note.

## License

This project includes XSLT stylesheets for rendering Peppol documents. Please refer to the original Peppol BIS Billing 3.0 documentation for licensing information regarding the stylesheets.
