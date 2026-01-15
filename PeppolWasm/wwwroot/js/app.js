async function renderXmlContent(xmlContent, xsltPath = '/render-billing-3.xsl') {
  const iframe = document.getElementById('xmlFrame');
  if (!iframe) return;

  try {
    // Parse the XML content
    const parser = new DOMParser();
    const xmlDoc = parser.parseFromString(xmlContent, 'application/xml');
    
    // Check for XML parsing errors
    const parseError = xmlDoc.querySelector('parsererror');
    if (parseError) {
      console.error('XML parsing error:', parseError.textContent);
      return;
    }

    // Fetch the XSLT stylesheet
    const xsltResponse = await fetch(xsltPath);
    if (!xsltResponse.ok) {
      console.error('Failed to fetch XSLT:', xsltResponse.statusText);
      return;
    }
    const xsltText = await xsltResponse.text();
    const xsltDoc = parser.parseFromString(xsltText, 'application/xml');

    // Check for XSLT parsing errors
    const xsltParseError = xsltDoc.querySelector('parsererror');
    if (xsltParseError) {
      console.error('XSLT parsing error:', xsltParseError.textContent);
      return;
    }

    // Create XSLT processor and apply transformation
    const xsltProcessor = new XSLTProcessor();
    xsltProcessor.importStylesheet(xsltDoc);
    
    const resultDoc = xsltProcessor.transformToDocument(xmlDoc);
    
    // Serialize the result to HTML string
    const serializer = new XMLSerializer();
    const htmlContent = serializer.serializeToString(resultDoc);

    // Write the transformed HTML to the iframe
    const blob = new Blob([htmlContent], { type: 'text/html' });
    const url = URL.createObjectURL(blob);
    iframe.src = url;
    
  } catch (error) {
    console.error('XSLT transformation error:', error);
  }
}