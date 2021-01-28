using System;
using Microsoft.Extensions.DependencyInjection;

namespace Function
{
	public static class FunctionInstaller
	{
		//Install your custom dependencies here
		public static void InstallFunction(this IServiceCollection services)
		{
			services.AddTransient<TypedHandler>();
		}

	}
}