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
			Assert.Pass();
		}
	}
}