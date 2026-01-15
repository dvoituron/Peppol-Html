async function renderXmlContent(xmlContent, xsltPath = '/render-billing-3.xsl') {
  const iframe = document.getElementById('xmlFrame');
  if (!iframe) return;

  try {
    // Build the absolute URL for the XSLT
    const xsltAbsoluteUrl = new URL(xsltPath, window.location.href).href;
    
    // Add or update the xml-stylesheet processing instruction
    // This allows the browser to natively process the XSLT with proper document() resolution
    let processedXml = xmlContent.trim();
    
    // Remove any existing xml-stylesheet processing instructions
    processedXml = processedXml.replace(/<\?xml-stylesheet[^?]*\?>\s*/g, '');
    
    // Find the position after the XML declaration (if present)
    const xmlDeclMatch = processedXml.match(/^<\?xml[^?]*\?>\s*/);
    let insertPosition = 0;
    if (xmlDeclMatch) {
      insertPosition = xmlDeclMatch[0].length;
    }
    
    // Insert the xml-stylesheet processing instruction pointing to our XSLT
    const stylesheetPI = `<?xml-stylesheet type="text/xsl" href="${xsltAbsoluteUrl}"?>\n`;
    processedXml = processedXml.substring(0, insertPosition) + stylesheetPI + processedXml.substring(insertPosition);
    
    // Create a Blob URL for the XML content
    // The blob URL will be same-origin, allowing the XSLT to load document() resources
    const blob = new Blob([processedXml], { type: 'application/xml' });
    const blobUrl = URL.createObjectURL(blob);
    
    // Set the iframe source to the blob URL
    // The browser will apply the XSLT transformation with proper document() resolution
    iframe.src = blobUrl;
    
  } catch (error) {
    console.error('XSLT transformation error:', error);
  }
}

function printXmlFrame() {
  const iframe = document.getElementById('xmlFrame');
  if (!iframe) return;

  try {
    iframe.contentWindow.focus();
    iframe.contentWindow.print();
  } catch (error) {
    console.error('Print error:', error);
  }
}

function attachPrintButtonHandler() {
  const printButton = document.getElementById('PrintButton');
  if (printButton) {
    printButton.addEventListener('click', printXmlFrame);
  }
}
