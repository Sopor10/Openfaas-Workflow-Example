using System.Threading.Tasks;
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
		async public Task Test1()
		{
			var sut = CreateTestObject();

			var result = await sut.Handle(new Input() { Html = "Test" });

			Assert.That(result, Is.EqualTo(new Output() { Table = null }));
		}

		[Test]
		async public Task CanDeserializeEmptyNumberInput()
		{
			var sut = CreateTestObject();

			var result = new Deserializer().Deserialize("{\"Html\":\"Test\"}");

			Assert.That(result, Is.EqualTo(new Input() { Html = "Test" }));
			Assert.That(result.Number, Is.EqualTo(0));
		}

		[Test]
		async public Task DeserializeNumber()
		{
			var sut = CreateTestObject();

			var result = new Deserializer().Deserialize("{\"Html\":\"Test\", \"Number\" : \"1\"}");

			Assert.That(result, Is.EqualTo(new Input() { Html = "Test", Number = 1}));
		}

		private TypedHandler CreateTestObject()
		{
			return new TypedHandler();
		}
	}
}