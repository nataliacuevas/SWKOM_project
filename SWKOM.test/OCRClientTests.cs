using System;
using System.IO;
using NUnit.Framework;
using NPaperless.OCRLibrary;
using System.Drawing.Drawing2D;

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



    }
}
