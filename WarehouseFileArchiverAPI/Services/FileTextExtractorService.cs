using DocumentFormat.OpenXml.Packaging;
using WarehouseFileArchiverAPI.Interfaces;

namespace WarehouseFileArchiverAPI.Services
{
    public class FileTextExtractorService : IFileTextExtractorService
    {
        public async Task<string> ExtractTextAsync(IFormFile file)
        {
            var ext = Path.GetExtension(file.FileName).ToLower();
            using var stream = file.OpenReadStream();

            if (ext == ".pdf")
                return await ExtractFromPdfAsync(stream);
            if (ext == ".docx" || ext == ".doc")
                return ExtractFromWord(stream);

            return string.Empty;
        }

        private async Task<string> ExtractFromPdfAsync(Stream stream)
        {
            using var reader = new iText.Kernel.Pdf.PdfReader(stream);
            using var doc = new iText.Kernel.Pdf.PdfDocument(reader);
            var strategy = new iText.Kernel.Pdf.Canvas.Parser.Listener.SimpleTextExtractionStrategy();
            var text = "";

            for (int i = 1; i <= doc.GetNumberOfPages(); i++)
            {
                var pageText = iText.Kernel.Pdf.Canvas.Parser.PdfTextExtractor.GetTextFromPage(doc.GetPage(i), strategy);
                text += pageText;
            }

            return text;
        }

        private string ExtractFromWord(Stream stream)
        {
            using var wordDoc = WordprocessingDocument.Open(stream, false);
            return wordDoc.MainDocumentPart.Document.Body.InnerText;
        }
    }

}