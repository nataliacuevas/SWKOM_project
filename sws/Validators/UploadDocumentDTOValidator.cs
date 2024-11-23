
using System;
using FluentValidation;
using System.Linq.Expressions;


using sws.SL.DTOs;

namespace sws.Validators
{
	public class UploadDocumentDTOValidator : AbstractValidator<UploadDocumentDTO>
    {
		public UploadDocumentDTOValidator() { 

        RuleFor(doc => doc.Name)
                .NotEmpty().WithMessage("Document name is required.")
                .MaximumLength(100).WithMessage("Document name cannot exceed 100 characters.")
                .MinimumLength(3).WithMessage("Name must be at least 3 characters long.");

            RuleFor(doc => doc.File)
                    .NotEmpty().WithMessage("Document content is required.");
            //    .MaximumLength(5000).WithMessage("Document content cannot exceed 5000 characters.");
        }
    }
}

