using System.Text.RegularExpressions;
using System.Xml;
using System.Xml.Xsl;

namespace Peppol_Viewer;

public class Program
{
    public static void Main(string[] args)
    {
        // Example usage
        Console.WriteLine("Peppol Invoice/Credit Note Viewer");
        Console.WriteLine("==================================");
        
        // Get the base directory where stylesheets are located
        string baseDirectory = AppDomain.CurrentDomain.BaseDirectory;
        string stylesheetsPath = Path.Combine(baseDirectory, "..", "..", "..", "Stylesheets");
        
        // For development, use relative path from project root
        if (!Directory.Exists(stylesheetsPath))
        {
            stylesheetsPath = Path.Combine(Directory.GetCurrentDirectory(), "Stylesheets");
        }
        
        string xsltPath = Path.Combine(stylesheetsPath, "render-billing-3.xsl");
        
        Console.WriteLine($"Stylesheets path: {Path.GetFullPath(stylesheetsPath)}");
        Console.WriteLine($"XSLT path exists: {File.Exists(xsltPath)}");
        
        // Example: Convert a specific XML file if provided as argument
        if (args.Length > 0 && File.Exists(args[0]))
        {
            string xmlFilePath = args[0];
            string outputPath = Path.ChangeExtension(xmlFilePath, ".html");
            
            try
            {
                PeppolConverter.ConvertXmlToHtml(xmlFilePath, xsltPath, outputPath);
                Console.WriteLine($"Successfully converted: {xmlFilePath}");
                Console.WriteLine($"Output saved to: {outputPath}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error converting file: {ex.Message}");
                if (ex.InnerException != null)
                {
                    Console.WriteLine($"Inner Exception: {ex.InnerException.Message}");
                    if (ex.InnerException.InnerException != null)
                    {
                        Console.WriteLine($"Inner Inner Exception: {ex.InnerException.InnerException.Message}");
                    }
                }
            }
        }
        else
        {
            Console.WriteLine("\nUsage: Peppol-Viewer <path-to-xml-file>");
            Console.WriteLine("Example: Peppol-Viewer c8d0172f-9c79-4f81-acf3-666d5e69d942.xml");
        }
    }
}

/// <summary>
/// Provides methods to convert Peppol UBL XML documents to HTML using XSLT stylesheets.
/// </summary>
public static class PeppolConverter
{
    /// <summary>
    /// Converts a Peppol UBL Invoice or Credit Note XML file to HTML using the XSLT stylesheet.
    /// </summary>
    /// <param name="xmlFilePath">The path to the input XML file (Invoice or Credit Note).</param>
    /// <param name="xsltFilePath">The path to the XSLT stylesheet (render-billing-3.xsl).</param>
    /// <param name="outputHtmlPath">The path where the output HTML file will be saved.</param>
    /// <param name="languageCode">The language code for localization (default: "en"). Supported: en, is, pl, se, sr.</param>
    /// <exception cref="FileNotFoundException">Thrown when the XML or XSLT file is not found.</exception>
    /// <exception cref="XsltException">Thrown when there's an error in the XSLT transformation.</exception>
    public static void ConvertXmlToHtml(string xmlFilePath, string xsltFilePath, string outputHtmlPath, string languageCode = "en")
    {
        if (!File.Exists(xmlFilePath))
        {
            throw new FileNotFoundException($"XML file not found: {xmlFilePath}");
        }

        if (!File.Exists(xsltFilePath))
        {
            throw new FileNotFoundException($"XSLT file not found: {xsltFilePath}");
        }

        // Get the absolute paths
        string xsltFullPath = Path.GetFullPath(xsltFilePath);
        string xmlFullPath = Path.GetFullPath(xmlFilePath);

        // Create XsltSettings that enable document() function and scripts
        var xsltSettings = new XsltSettings(enableDocumentFunction: true, enableScript: false);

        // Create a custom XmlResolver that handles relative paths from the XSLT location
        var resolver = new XmlUrlResolver();

        // Load the XSLT stylesheet using the file path directly (preserves base URI for relative imports)
        var xslt = new XslCompiledTransform();
        xslt.Load(xsltFullPath, xsltSettings, resolver);

        // Create XSLT arguments for passing parameters
        var xsltArgs = new XsltArgumentList();
        xsltArgs.AddParam("languageCode", "", languageCode);

        // Ensure output directory exists
        string? outputDir = Path.GetDirectoryName(outputHtmlPath);
        if (!string.IsNullOrEmpty(outputDir) && !Directory.Exists(outputDir))
        {
            Directory.CreateDirectory(outputDir);
        }

        // Create XmlReader with XmlResolver set to allow document() function
        var readerSettings = new XmlReaderSettings
        {
            DtdProcessing = DtdProcessing.Parse,
            XmlResolver = resolver
        };

        // Create XmlWriter settings for HTML output
        var writerSettings = new XmlWriterSettings
        {
            Indent = true,
            OmitXmlDeclaration = true,
            ConformanceLevel = ConformanceLevel.Auto
        };

        // Perform the transformation with XmlResolver to allow document() function to resolve external URIs
        using var xmlReader = XmlReader.Create(xmlFullPath, readerSettings);
        using var stringWriter = new StringWriter();
        using var xmlWriter = XmlWriter.Create(stringWriter, writerSettings);
        xslt.Transform(xmlReader, xsltArgs, xmlWriter, resolver);
        xmlWriter.Flush();
        
        // Post-process: Fix escaped characters in style tags
        string htmlContent = FixStyleTagEscaping(stringWriter.ToString());
        //string htmlContent = stringWriter.ToString();

        // Post-process: Add flex-direction: row to table_body styles if missing
        htmlContent = htmlContent.Replace("<style>", "<style> .items_table_body_small_screen_holder { flex-direction: column !important; } ", StringComparison.OrdinalIgnoreCase);

        File.WriteAllText(outputHtmlPath, htmlContent, System.Text.Encoding.UTF8);
    }

    /// <summary>
    /// Fixes escaped &lt; and &gt; characters within style tags.
    /// </summary>
    private static string FixStyleTagEscaping(string html)
    {
        // Use regex to find style tag content and unescape &lt; and &gt;
        return Regex.Replace(html, @"(<style[^>]*>)(.*?)(</style>)", 
            match => match.Groups[1].Value + 
                     match.Groups[2].Value.Replace("&gt;", " > ").Replace("&lt;", " < ") + 
                     match.Groups[3].Value,
            RegexOptions.Singleline | RegexOptions.IgnoreCase);
    }

       /// <summary>
    /// Converts a Peppol UBL Invoice or Credit Note XML file to HTML and returns the HTML as a string.
    /// </summary>
    /// <param name="xmlFilePath">The path to the input XML file (Invoice or Credit Note).</param>
    /// <param name="xsltFilePath">The path to the XSLT stylesheet (render-billing-3.xsl).</param>
    /// <param name="languageCode">The language code for localization (default: "en"). Supported: en, is, pl, se, sr.</param>
    /// <returns>The generated HTML content as a string.</returns>
    /// <exception cref="FileNotFoundException">Thrown when the XML or XSLT file is not found.</exception>
    /// <exception cref="XsltException">Thrown when there's an error in the XSLT transformation.</exception>
    public static string ConvertXmlToHtmlString(string xmlFilePath, string xsltFilePath, string languageCode = "en")
    {
        if (!File.Exists(xmlFilePath))
        {
            throw new FileNotFoundException($"XML file not found: {xmlFilePath}");
        }

        if (!File.Exists(xsltFilePath))
        {
            throw new FileNotFoundException($"XSLT file not found: {xsltFilePath}");
        }

        // Get the absolute paths
        string xsltFullPath = Path.GetFullPath(xsltFilePath);
        string xmlFullPath = Path.GetFullPath(xmlFilePath);

        // Create XsltSettings that enable document() function and scripts
        var xsltSettings = new XsltSettings(enableDocumentFunction: true, enableScript: false);

        // Create a custom XmlResolver that handles relative paths from the XSLT location
        var resolver = new XmlUrlResolver();

        // Load the XSLT stylesheet using the file path directly (preserves base URI for relative imports)
        var xslt = new XslCompiledTransform();
        xslt.Load(xsltFullPath, xsltSettings, resolver);

        // Create XSLT arguments for passing parameters
        var xsltArgs = new XsltArgumentList();
        xsltArgs.AddParam("languageCode", "", languageCode);

        // Create XmlReader with XmlResolver set to allow document() function
        var readerSettings = new XmlReaderSettings
        {
            DtdProcessing = DtdProcessing.Parse,
            XmlResolver = resolver
        };

        // Create XmlWriter settings for HTML output
        var writerSettings = new XmlWriterSettings
        {
            Indent = true,
            OmitXmlDeclaration = true,
            ConformanceLevel = ConformanceLevel.Auto
        };

        // Perform the transformation
        using var xmlReader = XmlReader.Create(xmlFullPath, readerSettings);
        using var stringWriter = new StringWriter();
        using var xmlWriter = XmlWriter.Create(stringWriter, writerSettings);
        xslt.Transform(xmlReader, xsltArgs, xmlWriter, resolver);
        xmlWriter.Flush();
        
        // Post-process: Fix escaped characters in style tags
        return FixStyleTagEscaping(stringWriter.ToString());
    }

    /// <summary>
    /// Converts XML content (as a string) to HTML using the XSLT stylesheet.
    /// </summary>
    /// <param name="xmlContent">The XML content as a string.</param>
    /// <param name="xsltFilePath">The path to the XSLT stylesheet (render-billing-3.xsl).</param>
    /// <param name="languageCode">The language code for localization (default: "en"). Supported: en, is, pl, se, sr.</param>
    /// <returns>The generated HTML content as a string.</returns>
    /// <exception cref="FileNotFoundException">Thrown when the XSLT file is not found.</exception>
    /// <exception cref="XsltException">Thrown when there's an error in the XSLT transformation.</exception>
    public static string ConvertXmlContentToHtml(string xmlContent, string xsltFilePath, string languageCode = "en")
    {
        if (!File.Exists(xsltFilePath))
        {
            throw new FileNotFoundException($"XSLT file not found: {xsltFilePath}");
        }

        // Get the absolute path to the XSLT file to ensure relative document() references work
        string xsltFullPath = Path.GetFullPath(xsltFilePath);

        // Create XsltSettings that enable document() function and scripts
        var xsltSettings = new XsltSettings(enableDocumentFunction: true, enableScript: false);

        // Create a custom XmlResolver that handles relative paths from the XSLT location
        var resolver = new XmlUrlResolver();

        // Load the XSLT stylesheet using the file path directly (preserves base URI for relative imports)
        var xslt = new XslCompiledTransform();
        xslt.Load(xsltFullPath, xsltSettings, resolver);

        // Create XSLT arguments for passing parameters
        var xsltArgs = new XsltArgumentList();
        xsltArgs.AddParam("languageCode", "", languageCode);

        // Create XmlReader with XmlResolver set to allow document() function
        var readerSettings = new XmlReaderSettings
        {
            DtdProcessing = DtdProcessing.Parse,
            XmlResolver = resolver
        };

        // Create XmlWriter settings for HTML output
        var writerSettings = new XmlWriterSettings
        {
            Indent = true,
            OmitXmlDeclaration = true,
            ConformanceLevel = ConformanceLevel.Auto
        };

        // Perform the transformation
        using var stringReader = new StringReader(xmlContent);
        using var xmlReader = XmlReader.Create(stringReader, readerSettings);
        using var stringWriter = new StringWriter();
        using var xmlWriter = XmlWriter.Create(stringWriter, writerSettings);
        xslt.Transform(xmlReader, xsltArgs, xmlWriter, resolver);
        xmlWriter.Flush();
        
        // Post-process: Fix escaped characters in style tags
        return FixStyleTagEscaping(stringWriter.ToString());
    }

    /// <summary>
    /// Gets the default XSLT path based on the application's location.
    /// </summary>
    /// <param name="stylesheetsFolder">Optional: Custom path to the Stylesheets folder.</param>
    /// <returns>The full path to the render-billing-3.xsl file.</returns>
    public static string GetDefaultXsltPath(string? stylesheetsFolder = null)
    {
        if (string.IsNullOrEmpty(stylesheetsFolder))
        {
            // Try relative to executable
            string baseDirectory = AppDomain.CurrentDomain.BaseDirectory;
            stylesheetsFolder = Path.Combine(baseDirectory, "..", "..", "..", "Stylesheets");
            
            // If not found, try current directory
            if (!Directory.Exists(stylesheetsFolder))
            {
                stylesheetsFolder = Path.Combine(Directory.GetCurrentDirectory(), "Stylesheets");
            }
        }

        return Path.Combine(stylesheetsFolder, "render-billing-3.xsl");
    }
}
