using System.Collections.Generic;
using System.Collections.Immutable;
using System.Threading.Tasks;
using FluentAssertions;
using NUnit.Framework;

namespace Function.Test
{
	public class FunctionTests
	{
		[SetUp]
		public void Setup()
		{
		}

		[Test]
		public async Task TypedHandlerShouldGenerateMessageWhenTheDataIncludesLangenhagen()
		{
			var langenhagen = new List<string>() { "Langenhagen", "0", "100", "10", "1000" };

			var result = await new TypedHandler().Handle(new Input
			{
				Table = new Table
				{
					Data = new List<List<string>>()
					{
						langenhagen
					}
				}
			});
			result.Message.Should().Contain("Langenhagen beträgt 10");
		}
		[Test]
		public async Task TypedHandlerShouldNotIncludeHannover()
		{
			var hannover = new List<string>() { "Hannover", "10", "1", "100", "5000" };

			var result = await new TypedHandler().Handle(new Input
			{
				Table = new Table
				{
					Data = new List<List<string>>()
					{
						hannover
					}
				}
			});
			result.Message.Should().BeEmpty();
		}
	}
}