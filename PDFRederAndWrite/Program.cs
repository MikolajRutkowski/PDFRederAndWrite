
using iText.Kernel.Pdf;



namespace PDFReaderAndWrite
{
    internal class Program
    {
       
            static void Main(string[] args)
            {
                string inputPath = "aaa.pdf";   // Ścieżka do istniejącego pliku PDF
                string outputPath = "output.pdf"; // Ścieżka do nowego pliku z dodatkowymi elementami

                // Otwieramy istniejący plik PDF i zapisujemy zmiany do nowego pliku
                using (PdfReader reader = new PdfReader(inputPath))
                using (PdfWriter writer = new PdfWriter(outputPath))
                using (PdfDocument pdf = new PdfDocument(reader, writer))
                {
                    List<PdfText> pdfTexts = new List<PdfText>();
                    pdfTexts = PdfHelper.ExtractTextWithCoordinates(inputPath);
                    foreach (PdfText pdfText in pdfTexts)
                    {
                    Console.WriteLine(pdfText.Text + " " + pdfText.X + " " + pdfText.Y + " ");
                    }
                    //PdfHelper.AddText(pdf, "Dodatkowy tekst", 150, 200);
                }

                
            }
        

       
    }

}

