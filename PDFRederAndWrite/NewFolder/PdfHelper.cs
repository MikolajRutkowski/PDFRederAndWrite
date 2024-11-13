using System;
using System.Collections.Generic;
using iText.Kernel.Pdf;
using iText.Kernel.Pdf.Canvas;
using iText.Kernel.Font;
using iText.IO.Font.Constants;
using iText.Kernel.Pdf.Canvas.Parser;
using iText.Kernel.Pdf.Canvas.Parser.Listener;
using iText.Kernel.Pdf.Canvas.Parser.Data;
using iText.Forms;

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
        // Funkcja do odczytywania wszystkich tekstów z pól formularza PDF
        public static List<string> ExtractFormFields(string filePath, bool withKey = false)
        {
            List<string> fieldValues = new List<string>();

            using (PdfReader reader = new PdfReader(filePath))
            using (PdfDocument pdfDoc = new PdfDocument(reader))
            {
                PdfAcroForm form = PdfAcroForm.GetAcroForm(pdfDoc, false);
                if (form != null)
                {
                    var fields = form.GetFormFields();
                    foreach (var field in fields)
                    {
                        string fieldValue = field.Value.GetValueAsString();
                        if (withKey) { fieldValues.Add($"{field.Key}: {fieldValue}"); }
                        else { 
                            
                            fieldValues.Add(fieldValue); 
                        }
                        
                    }
                }
            }

            return fieldValues;
        }

        // Funkcja do zamiany tekstu w polach formularza PDF
        public static void ReplaceTextInFormFields(string inputPath, string outputPath, string searchText, string replacementText)
        {
            using (PdfReader reader = new PdfReader(inputPath))
            using (PdfWriter writer = new PdfWriter(outputPath))
            using (PdfDocument pdfDoc = new PdfDocument(reader, writer))
            {
                PdfAcroForm form = PdfAcroForm.GetAcroForm(pdfDoc, true);
                if (form != null)
                {
                    var fields = form.GetFormFields();
                    foreach (var field in fields)
                    {
                        string fieldValue = field.Value.GetValueAsString();
                        if (fieldValue.Contains(searchText))
                        {
                            // Zamiana tekstu
                            string newValue = fieldValue.Replace(searchText, replacementText);
                            field.Value.SetValue(newValue);
                        }
                    }
                }
            }
        }



    }

    public class CustomLocationTextExtractionStrategy : IEventListener
    {
        private readonly List<PdfText> locations = new List<PdfText>();
        private const float Tolerance = 5f;

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
}
