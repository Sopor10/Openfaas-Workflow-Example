using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.ComponentModel;
using Microsoft.AspNetCore.Http;
using System.IO;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
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
		public Table Table { get; init; }
	}

	public record Table
	{
		public List<string> Header { get; init; }

		public List<List<string>> Data { get; init; }
	}

	public class InputValidator : AbstractValidator<Input>
	{
		public InputValidator()
		{
			RuleFor(x => x.Table).NotNull().NotEmpty();
		}
	}

	public record Output
	{
		public string Message { get; init; }
	}

	public class TypedHandler
	{
		public async Task<Output> Handle(Input input)
		{
			input = input ?? throw new ArgumentNullException();

			var result = input.Table.Data
				.Select(x => x.ToArray())
				.Where(x => x[0] == "Langenhagen")
				.Select(
					x => "Die aktuelle 7-Tage-Inzidenz für " + x[0] + " beträgt " + x[3] +
					     ".\n Aktuell sind "+x[1]+" Menschen infiziert.")
				.Aggregate("",(x,y)=> x+"\n"+y);

			return new Output() { Message = result };
		}
	}
}