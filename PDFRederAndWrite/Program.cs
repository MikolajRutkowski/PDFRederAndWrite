
using iText.Kernel.Pdf;



namespace PDFReaderAndWrite
{
    internal class Program
    {
       
            static void Main(string[] args)
            {
                string inputPath = "TESTC2.pdf";   // Ścieżka do istniejącego pliku PDF
                string outputPath = "outputtttttt.pdf"; // Ścieżka do nowego pliku z dodatkowymi elementami

                // Otwieramy istniejący plik PDF i zapisujemy zmiany do nowego pliku
                
                    List<PdfText> pdfTexts = new List<PdfText>();
                     var lisa = PdfHelper.ExtractFormFields(inputPath);
                    foreach (var pdfTextt in lisa)
                    {
                    Console.WriteLine(pdfTextt);
                    if("ZR" == pdfTextt)
                    {
                    Console.WriteLine("Tu zmiana");
                    PdfHelper.ReplaceTextInFormFields(inputPath,outputPath, "ZR", "TT");
                    }
                    }
                    
                

                
            }
        

       
    }

}

