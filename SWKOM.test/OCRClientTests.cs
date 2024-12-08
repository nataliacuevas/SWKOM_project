using NUnit.Framework;
using NPaperless.OCRLibrary;
using System.Drawing.Drawing2D;
using System.IO.Pipes;


namespace SWKOM.test
{
    [TestFixture]
    public class OcrClientTests
    {
        private OcrClient _ocrClient;
        private string _testFilesPath;

        [SetUp]
        public void SetUp()
        {
            // Setup OcrClient with default options
            var options = new OcrOptions
            {
                TessDataPath = "./tessdata",
                Language = "eng"
            };
            _ocrClient = new OcrClient(options);

            // Define the path to the TestFiles folder
            _testFilesPath = Path.Combine(AppContext.BaseDirectory, "TestFiles");
        }

        [Test]
        public void OcrPdf_ShouldHandleEmptyStream()
        {
            // Arrange
            using var emptyStream = new MemoryStream();

            // Act & Assert
            var ex = Assert.Throws<ArgumentException>(() => _ocrClient.OcrPdf(emptyStream));
            Assert.That(ex.Message, Does.Contain("stream"), "Expected an exception for empty stream.");
        }

        [Test]
        public void OcrPdf_ShouldHandleCorruptedPdfStream()
        {
            // Arrange
            using var corruptedStream = new MemoryStream(new byte[] { 0x00, 0x01, 0x02 });

            // Act & Assert
            var ex = Assert.Throws<ImageMagick.MagickMissingDelegateErrorException>(() => _ocrClient.OcrPdf(corruptedStream));
            Assert.That(ex, Is.Not.Null, "Expected an exception for corrupted PDF stream.");
        }

        [Test]
        public void OcrPdf_ShouldExtractText_FromValidPdf()
        {
            // Arrange
            var filePath = Path.Combine(AppContext.BaseDirectory, "TestFiles", "HelloWorld.pdf");
            using var validPdfStream = new FileStream(filePath, FileMode.Open, FileAccess.Read);

            // Act
            var result = _ocrClient.OcrPdf(validPdfStream);

            // Assert
            Assert.That(result, Is.Not.Null.And.Not.Empty, "OCR result should not be null or empty.");
        }

        [Test]
        public void OcrPdf_ShouldExtractText_FromLongValidPdf()
        {
            // Arrange
            //var filePath = Path.Combine(AppContext.BaseDirectory, "TestFiles", "semester-project.pdf");
            string filePath = "./TestFiles/semester-project.pdf";
            //using var fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read);
            using var fileStream = new FileStream(filePath, FileMode.Open);
            //
            using StreamReader reader = new StreamReader(fileStream);
            OcrClient ocrClient = new OcrClient(new OcrOptions());
            var ocrContentText = ocrClient.OcrPdf(fileStream);

            // Act
            var result = _ocrClient.OcrPdf(fileStream);

            // Assert
            Assert.That(result, Is.Not.Null.And.Not.Empty, "OCR result should not be null or empty.");
        }

        [Test]
        public void OcrPdf_ShouldThrowException_ForCorruptedPdf()
        {
            // Arrange
            using var corruptedStream = new MemoryStream(new byte[] { 0x00, 0x01 });

            // Act & Assert
            var ex = Assert.Throws<ImageMagick.MagickMissingDelegateErrorException>(() => _ocrClient.OcrPdf(corruptedStream));
            Assert.That(ex, Is.Not.Null, "Expected a missing delegate error for corrupted PDF.");
        }



    }
}