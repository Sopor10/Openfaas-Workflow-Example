using System;
using System.ComponentModel;
using Microsoft.AspNetCore.Http;
using System.IO;
using System.Net.Http;
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

        public TypedHandler TypedHandler { get; }

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
            return (200, JsonSerializer.Serialize(output));
        }
    }
    public record Input
    {
        public string Url { get; init; }
    }

    public class InputValidator : AbstractValidator<Input>
    {
        public InputValidator()
        {
            RuleFor(x => x.Url).NotNull().NotEmpty();
        }
    }

    public record Output
    {
        public string Html { get; init; }
        public string Number { get; init; } = "1";
    }

    public class TypedHandler
    {
        public async Task<Output> Handle(Input input)
        {
            input = input ?? throw new ArgumentNullException();

            using var client = new HttpClient();

            var content = await client.GetStringAsync(input.Url);

            Console.WriteLine(content);
            return new Output() { Html = content };
        }
    }
}