using FluentValidation;

namespace TrickingLibrary.Api.Form.Validation
{
    public class CreateCategoryFormValidation : AbstractValidator<CreateCategoryForm>
    {
        public CreateCategoryFormValidation()
        {
            RuleFor(x => x.Name).NotEmpty();
            RuleFor(x => x.Description).NotEmpty();
        }
    }
}