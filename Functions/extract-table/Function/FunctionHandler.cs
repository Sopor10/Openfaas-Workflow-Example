using System;
using Microsoft.AspNetCore.Http;
using System.IO;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Function
{
	public class Deserializer
	{
		public Deserializer()
		{
		}

		public Input Deserialize(string input)
		{
			JsonSerializerOptions options = new JsonSerializerOptions()
			{
				NumberHandling = JsonNumberHandling.AllowReadingFromString,
				PropertyNameCaseInsensitive = true
			};
			var deserialize = JsonSerializer.Deserialize<Input>(input, options);
			return deserialize;
		}
	}

	public class FunctionHandler
	{
		public Deserializer Deserializer { get; }

		public FunctionHandler(TypedHandler typedHandler, Deserializer deserializer)
		{
			TypedHandler = typedHandler;
			Deserializer = deserializer;
		}

		public TypedHandler TypedHandler { get; }

		public async Task<(int, string)> Handle(HttpRequest request)
		{
			var reader = new StreamReader(request.Body);
			var input = await reader.ReadToEndAsync();

			var deserialize = Deserializer.Deserialize(input);

			var validationResult = await new InputValidator().ValidateAsync(deserialize);
			if (!validationResult.IsValid)
			{
				throw new NotSupportedException(validationResult.ToString());
			}
			var output = await TypedHandler.Handle(deserialize);
			return (200, JsonSerializer.Serialize(output ));
		}
	}
}