using NUnit.Framework;
using sws.Validators;
using sws.SL.DTOs;
using FluentValidation.TestHelper;

namespace SWKOM.test
{
    [TestFixture]
    public class UploadDocumentDTOValidatorTests
    {
        private UploadDocumentDTOValidator _validator;

        [SetUp]
        public void Setup()
        {
            _validator = new UploadDocumentDTOValidator();
        }

        [Test]
        public void Should_HaveError_When_NameIsEmpty()
        {
            var dto = new UploadDocumentDTO { Name = "", Content = "Some content" };
            var result = _validator.TestValidate(dto);
            result.ShouldHaveValidationErrorFor(doc => doc.Name)
                  .WithErrorMessage("Document name is required.");
        }

        [Test]
        public void Should_HaveError_When_NameIsTooShort()
        {
            var dto = new UploadDocumentDTO { Name = "AB", Content = "Some content" };
            var result = _validator.TestValidate(dto);
            result.ShouldHaveValidationErrorFor(doc => doc.Name)
                  .WithErrorMessage("Name must be at least 3 characters long.");
        }

        [Test]
        public void Should_HaveError_When_NameIsTooLong()
        {
            var longName = new string('A', 101);
            var dto = new UploadDocumentDTO { Name = longName, Content = "Some content" };
            var result = _validator.TestValidate(dto);
            result.ShouldHaveValidationErrorFor(doc => doc.Name)
                  .WithErrorMessage("Document name cannot exceed 100 characters.");
        }

        [Test]
        public void Should_HaveError_When_ContentIsEmpty()
        {
            var dto = new UploadDocumentDTO { Name = "Valid Name", Content = "" };
            var result = _validator.TestValidate(dto);
            result.ShouldHaveValidationErrorFor(doc => doc.Content)
                  .WithErrorMessage("Document content is required.");
        }

        [Test]
        public void Should_HaveError_When_ContentExceedsMaxLength()
        {
            var longContent = new string('A', 5001);
            var dto = new UploadDocumentDTO { Name = "Valid Name", Content = longContent };
            var result = _validator.TestValidate(dto);
            result.ShouldHaveValidationErrorFor(doc => doc.Content)
                  .WithErrorMessage("Document content cannot exceed 5000 characters.");
        }

        [Test]
        public void Should_NotHaveError_When_DocumentIsValid()
        {
            var dto = new UploadDocumentDTO
            {
                Name = "Valid Name",
                Content = "This is valid content."
            };

            var result = _validator.TestValidate(dto);
            result.ShouldNotHaveAnyValidationErrors();
        }
    }
}
