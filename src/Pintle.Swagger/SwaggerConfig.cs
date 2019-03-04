namespace Pintle.Swagger
{
	using System;
	using System.IO;
	using System.Linq;
	using System.Web.Http;
	using System.Xml.Linq;
	using HttpHandlers;
	using Swashbuckle.Application;
	using Configuration;

	public class SwaggerConfig
	{
		public static void Register(SwaggerConfigSettings settings)
		{
			GlobalConfiguration.Configuration.MessageHandlers.Add(new SwaggerSitecoreAuthMessageHandler());

			GlobalConfiguration.Configuration
				.EnableSwagger(c =>
				{
					c.MultipleApiVersions(
						(apiDesc, targetApiVersion) =>
						{
							var defaultRoute = settings.ApiVersions.FirstOrDefault(x => x.RouteTemplate == "*");

							foreach (var apiVersion in settings.ApiVersions)
							{
								if (apiDesc.Route.RouteTemplate.StartsWith(apiVersion.RouteTemplate, StringComparison.InvariantCultureIgnoreCase)
									&& targetApiVersion.Equals(apiVersion.Name, StringComparison.InvariantCultureIgnoreCase))
								{
									return true;
								}
							}

							if (defaultRoute != null && targetApiVersion.Equals(defaultRoute.Name, StringComparison.InvariantCultureIgnoreCase))
							{
								return true;
							}

							return false;
						},
						vc =>
						{
							foreach (var apiVersion in settings.ApiVersions)
							{
								vc.Version(apiVersion.Name.ToLowerInvariant(), apiVersion.Description);
							}
						});

					c.UseFullTypeNameInSchemaIds();

					c.ResolveConflictingActions(apiDescriptions => apiDescriptions.First());
					c.DescribeAllEnumsAsStrings(true);
					c.IncludeXmlComments(GetCombinedXmlComments(settings));
				})
				.EnableSwaggerUi(c =>
				{
					c.DisableValidator();
				});
		}

		private static string GetCombinedXmlComments(SwaggerConfigSettings settings)
		{
			try
			{
				XElement xml = null;

				var targetFileNames = AppDomain.CurrentDomain.GetAssemblies()
					.Select(x => x.GetName().Name)
					.Select(x => $"{x.ToLowerInvariant().Replace(".dll", "")}.xml");

				var binFolder = System.Web.Hosting.HostingEnvironment.MapPath("~/bin");

				foreach (var fileName in targetFileNames)
				{
					try
					{
						var fullXmlFilePath = Path.Combine(binFolder, fileName);

						if (File.Exists(fullXmlFilePath))
						{
							if (xml == null)
							{
								xml = XElement.Load(fullXmlFilePath);
							}
							else
							{
								var dependentXml = XElement.Load(fullXmlFilePath);
								foreach (var ele in dependentXml.Descendants())
								{
									xml.Add(ele);
								}
							}
						}
					}
					catch
					{
						//
					}
				}

				var targetDir = Path.GetDirectoryName(settings.CombinedXmlDocPath);
				if (!string.IsNullOrEmpty(targetDir))
				{
					if (!Directory.Exists(targetDir))
					{
						Directory.CreateDirectory(targetDir);
					}
				}

				xml?.Save(settings.CombinedXmlDocPath);
			}
			catch
			{
				//
			}

			return settings.CombinedXmlDocPath;
		}
	}
}
