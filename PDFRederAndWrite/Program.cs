using System;
using iText.IO.Font.Constants;
using iText.Kernel.Font;
using iText.Kernel.Pdf;
using iText.Kernel.Pdf.Canvas;
using iText.Kernel.Pdf.Canvas.Parser;
using iText.Layout;

namespace PDFReaderAndWrite
{
    internal class Program
    {
        static void Main(string[] args)
        {
            string inputPath = @"z.pdf";
            string outputPath = @"Wynik.pdf";
            string searchText = "Gadanina";
            string textToAdd = "Nowy tekst";

            List<string> lines = new List<string>();
            try
            {
                SearchAndAddTextToPDF(inputPath, outputPath, searchText, textToAdd);
               // Console.WriteLine("Operacja zakończona! Plik wynikowy zapisany jako Wynik.pdf.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Wystąpił błąd: {ex.Message}");
            }
        }

        static void SearchAndAddTextToPDF(string inputPath, string outputPath, string searchText, string textToAdd)
        {
            // Upewnij się, że plik wyjściowy nie jest otwarty ani uszkodzony
            if (System.IO.File.Exists(outputPath))
            {
                System.IO.File.Delete(outputPath);
            }

            // Otwórz plik wejściowy i stwórz nowy plik wyjściowy
            using (PdfReader reader = new PdfReader(inputPath))
            using (PdfWriter writer = new PdfWriter(outputPath))
            using (PdfDocument pdfDoc = new PdfDocument(reader, writer))
            {
                Document document = new Document(pdfDoc);

                // Iteruj przez strony
                for (int page = 1; page <= pdfDoc.GetNumberOfPages(); page++)
                {
                    string pageText = PdfTextExtractor.GetTextFromPage(pdfDoc.GetPage(page));

                    if (pageText != null && pageText.Contains(searchText))
                    {
                        Console.WriteLine($"Znaleziono tekst na stronie {page}");

                        PdfCanvas canvas = new PdfCanvas(pdfDoc.GetPage(page));
                        
                    }
                }

                document.Close();
            }
        }
    }
}
