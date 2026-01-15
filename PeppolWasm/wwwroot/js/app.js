function renderXmlContent(xmlContent) {
  const iframe = document.getElementById('xmlFrame');
  if (iframe) {
    const blob = new Blob([xmlContent], { type: 'application/xml' });
    const url = URL.createObjectURL(blob);
    iframe.src = url;
  }
}