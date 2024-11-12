using System;
using iText.Kernel.Pdf;
using iText.Kernel.Pdf.Canvas;
using iText.Kernel.Font;
using iText.IO.Font.Constants;
using iText.Kernel.Pdf.Canvas.Parser.Data;
using iText.Kernel.Pdf.Canvas.Parser;
using System.Collections.Generic;
using iText.Kernel.Pdf.Canvas.Parser.Listener;
using iText.Kernel.Pdf.Canvas.Parser.Data;

namespace PDFReaderAndWrite
{
    public class PdfText
    {
        public float X { get; set; }
        public float Y { get; set; }
        public string Text { get; set; }
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
        // Funkcja do odczytywania tekstu z pliku PDF wraz z jego współrzędnymi
        public static List<PdfText> ExtractTextWithCoordinates(string filePath)
        {
            List<PdfText> extractedTexts = new List<PdfText>();

            using (PdfReader reader = new PdfReader(filePath))
            using (PdfDocument pdf = new PdfDocument(reader))
            {
                for (int pageNumber = 1; pageNumber <= pdf.GetNumberOfPages(); pageNumber++)
                {
                    var strategy = new CustomLocationTextExtractionStrategy();
                    PdfCanvasProcessor processor = new PdfCanvasProcessor(strategy);
                    processor.ProcessPageContent(pdf.GetPage(pageNumber));

                    extractedTexts.AddRange(strategy.GetLocations());
                }
            }

            return extractedTexts;
        }
    }


    public class CustomLocationTextExtractionStrategy : IEventListener
    {
        private readonly List<PdfText> locations = new List<PdfText>();
        private const float Tolerance = 5f; // Tolerancja odległości między fragmentami

        public void EventOccurred(IEventData data, EventType type)
        {
            if (type == EventType.RENDER_TEXT)
            {
                var renderInfo = (TextRenderInfo)data;
                string text = renderInfo.GetText();

                if (!string.IsNullOrEmpty(text))
                {
                    float x = renderInfo.GetBaseline().GetStartPoint().Get(0);
                    float y = renderInfo.GetBaseline().GetStartPoint().Get(1);

                    locations.Add(new PdfText
                    {
                        X = x,
                        Y = y,
                        Text = text
                    });
                }
            }
        }

        public List<PdfText> GetLocations()
        {
            return MergeTextFragments(locations);
        }

        private List<PdfText> MergeTextFragments(List<PdfText> fragments)
        {
            List<PdfText> mergedTexts = new List<PdfText>();

            fragments.Sort((a, b) => a.Y != b.Y ? b.Y.CompareTo(a.Y) : a.X.CompareTo(b.X));

            PdfText current = null;
            foreach (var fragment in fragments)
            {
                if (current == null)
                {
                    current = fragment;
                }
                else
                {
                    // Jeśli fragment znajduje się na tej samej linii i jest blisko poprzedniego fragmentu, łączymy je
                    if (Math.Abs(current.Y - fragment.Y) < Tolerance && fragment.X - (current.X + current.Text.Length * 5) < Tolerance)
                    {
                        current.Text += fragment.Text;
                    }
                    else
                    {
                        mergedTexts.Add(current);
                        current = fragment;
                    }
                }
            }

            if (current != null)
            {
                mergedTexts.Add(current);
            }

            return mergedTexts;
        }

        public ICollection<EventType> GetSupportedEvents()
        {
            return null;
        }
    }
    public class TextLocation
    {
        public float X { get; set; }
        public float Y { get; set; }
        public string Text { get; set; }
    }

}