using NUnit.Framework;
using sws.Validators;
using sws.SL.DTOs;
using FluentValidation.TestHelper;
using Microsoft.Extensions.Logging;
using Moq;
using sws.DAL.Repositories;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Http;

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
            var fileMock = new Mock<IFormFile>();
            var dto = new UploadDocumentDTO { Name = "", File = fileMock.Object };
            var result = _validator.TestValidate(dto);
            result.ShouldHaveValidationErrorFor(doc => doc.Name)
                  .WithErrorMessage("Document name is required.");
        }

        [Test]
        public void Should_HaveError_When_NameIsTooShort()
        {
            var fileMock = new Mock<IFormFile>();
            var dto = new UploadDocumentDTO { Name = "AB", File = fileMock.Object };
            var result = _validator.TestValidate(dto);
            result.ShouldHaveValidationErrorFor(doc => doc.Name)
                  .WithErrorMessage("Name must be at least 3 characters long.");
        }

        [Test]
        public void Should_HaveError_When_NameIsTooLong()
        {
            var fileMock = new Mock<IFormFile>();

            var longName = new string('A', 101);
            var dto = new UploadDocumentDTO { Name = longName, File = fileMock.Object };
            var result = _validator.TestValidate(dto);
            result.ShouldHaveValidationErrorFor(doc => doc.Name)
                  .WithErrorMessage("Document name cannot exceed 100 characters.");
        }

        [Test]
        public void Should_HaveError_When_NoUploadedFile()
        {
            var dto = new UploadDocumentDTO { Name = "Valid Name", File = null };
            var result = _validator.TestValidate(dto);
            result.ShouldHaveValidationErrorFor(doc => doc.File)
                  .WithErrorMessage("Document content is required.");
        }
    
        [Test]
        public void Should_NotHaveError_When_DocumentIsValid()
        {
            var fileMock = new Mock<IFormFile>();
            var dto = new UploadDocumentDTO { Name = "Valid Name", File = fileMock.Object };

            var result = _validator.TestValidate(dto);
            result.ShouldNotHaveAnyValidationErrors();
        }
    }
}
