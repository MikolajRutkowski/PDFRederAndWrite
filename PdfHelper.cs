using System;
using iText.Kernel.Pdf;
using iText.Kernel.Pdf.Canvas;
using iText.Kernel.Font;
using iText.IO.Font.Constants;

namespace PDFReaderAndWrite
{
    public class Pdftext
    {
        public int x { get; set; }
        public int y { get; set; }
        public string text { get; set; }

    }
    public class PdfHelper
    {
        // Funkcja do rysowania kształtów (prostokąt, linia)
        public static void DrawShape(PdfDocument pdf, float x, float y, float width, float height)
        {
            PdfCanvas canvas = new PdfCanvas(pdf.GetFirstPage());

            // Rysujemy prostokąt
            canvas.SetStrokeColorRgb(0, 0, 1)
                  .SetLineWidth(2f)
                  .Rectangle(x, y, width, height)
                  .Stroke();

            // Rysujemy linię
            canvas.MoveTo(x, y)
                  .LineTo(x + width, y + height)
                  .Stroke();
        }

        // Funkcja do dodawania tekstu (liczby) w określonym miejscu
        public static void AddText(PdfDocument pdf, string text, float x, float y)
        {
            PdfCanvas canvas = new PdfCanvas(pdf.GetFirstPage());

            // Ustawiamy czcionkę i rozmiar tekstu
            PdfFont font = PdfFontFactory.CreateFont(StandardFonts.HELVETICA);
            canvas.BeginText()
                  .SetFontAndSize(font, 12)
                  .MoveText(x, y)
                  .ShowText(text)
                  .EndText();
        }

        public static List<PdfText> ExtractTextWithCoordinates(string filePath)
        {
            List<PdfText> extractedTexts = new List<PdfText>();

            using (PdfReader reader = new PdfReader(filePath))
            using (PdfDocument pdf = new PdfDocument(reader))
            {
                // Przechodzimy przez każdą stronę w dokumencie
                for (int pageNumber = 1; pageNumber <= pdf.GetNumberOfPages(); pageNumber++)
                {
                    // Stosujemy niestandardową strategię ekstrakcji tekstu
                    var strategy = new LocationTextExtractionStrategy();
                    string pageText = PdfTextExtractor.GetTextFromPage(pdf.GetPage(pageNumber), strategy);

                    // Pobieramy informacje o pozycjach tekstu
                    foreach (var chunk in strategy.GetLocations())
                    {
                        extractedTexts.Add(new PdfText
                        {
                            X = (int)chunk.X,
                            Y = (int)chunk.Y,
                            Text = chunk.Text
                        });
                    }
                }
            }

            return extractedTexts;
        }

    }

    