using System;
using System.ComponentModel;
using Microsoft.AspNetCore.Http;
using System.IO;
using System.Text.Json;
using System.Threading.Tasks;
using FluentValidation;

namespace Function
{
	public class FunctionHandler
	{
		public FunctionHandler(TypedHandler typedHandler)
		{
			this.TypedHandler = typedHandler;
		}

		public TypedHandler TypedHandler { get;  }

		public async Task<(int, string)> Handle(HttpRequest request)
		{
			var reader = new StreamReader(request.Body);
			var input = await reader.ReadToEndAsync();
            
			var deserialize = JsonSerializer.Deserialize<Input>(input);
			var validationResult = await new InputValidator().ValidateAsync(deserialize);
			if (!validationResult.IsValid)
			{
				throw new InvalidEnumArgumentException(validationResult.ToString());
			}
			var output = await TypedHandler.Handle(deserialize);
			return (200, JsonSerializer.Serialize(output ));
		}
	}
	public record Input
	{
		public string Data { get; init; }
	}

	public class InputValidator : AbstractValidator<Input>
	{
		public InputValidator()
		{
			RuleFor(x => x.Data).NotNull().NotEmpty();
		}
	}

	public record Output
	{
		public string Data { get; init; }
	}

	public class TypedHandler
	{
		public async Task<Output> Handle(Input input)
		{
			input = input ?? throw new ArgumentNullException();
			return new Output() { Data = "Your input was: " + input.Data };
		}
	}
}