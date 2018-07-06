using Pintle.Swagger.Configuration;
using Sitecore.Pipelines;

namespace Pintle.Swagger.Pipelines.Initialize
{
	public class InitializeSwagger
	{
		public void Process(PipelineArgs args)
		{
			SwaggerConfig.Register(SwaggerConfigSettings.ConfiguredInstance);
		}
	}
}
