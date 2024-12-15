using System.Text;
using ImageMagick;
using Microsoft.Extensions.Options;
using Tesseract;

namespace NPaperless.OCRLibrary;

public class OcrClient : IOcrClient
{
    private readonly string tessDataPath;
    private readonly string language;

    public OcrClient(OcrOptions options)
    {
        this.tessDataPath = options.TessDataPath;
        this.language = options.Language;
    }

    public string OcrPdf(Stream pdfStream)
    {
        var stringBuilder = new StringBuilder(); // to store the extracted text from all pages of the PDF

        using (var magickImages = new MagickImageCollection()) // ImageMagick collection to process PDF pages as images
        {
            magickImages.Read(pdfStream); // Read the PDF stream and load its pages into the collection
            foreach (var magickImage in magickImages)
            {
                // Set the resolution and format of the image 
                magickImage.Density = new Density(300, 300);
                magickImage.Format = MagickFormat.Png;

                // Initialize Tesseract OCR engine with the specified data path and language
                using (var tesseractEngine = new TesseractEngine(tessDataPath, language, EngineMode.Default))
                {
                    //process current image page for OCR
                    using (var page = tesseractEngine.Process(Pix.LoadFromMemory(magickImage.ToByteArray())))
                    {
                        var extractedText = page.GetText();
                        stringBuilder.Append(extractedText);
                    }
                }
            }
        }
        // Return the combined text extracted from all pages of the PDF
        return stringBuilder.ToString();
    }
}