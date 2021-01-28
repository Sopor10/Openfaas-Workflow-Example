using FluentValidation;

namespace Function
{
	public record Input
	{
		public string Html { get; init; }
		public int Number { get; init; }
	}

	public class InputValidator : AbstractValidator<Input>
	{
		public InputValidator()
		{
			RuleFor(x => x.Html).NotNull().NotEmpty();
			RuleFor(x => x.Number).GreaterThanOrEqualTo(0);
		}
	}
}