namespace Pintle.Swagger.Configuration
{
	using System.Collections.Generic;
	using System.Xml;
	using Sitecore.Exceptions;
	using Sitecore.Configuration;

	public class SwaggerConfigSettings
	{
		public static SwaggerConfigSettings ConfiguredInstance => Factory
			.CreateObject(SwaggerSettingsNodePath, true) as SwaggerConfigSettings;

		private const string NameAttribute = "name";
		private const string RouteTemplateAttribute = "routeTemplate";
		private const string DescriptionAttribute = "description";

		public const string SwaggerSettingsNodePath = "pintle/swagger/swaggerSettings";

		private readonly IDictionary<string, ApiVersionSettings> apiVersions;
		private string combinedXmlDocPath;

		public SwaggerConfigSettings()
		{
			this.CombinedXmlDocPath = string.Empty;
			this.apiVersions = new Dictionary<string, ApiVersionSettings>();
		}

		public virtual IEnumerable<ApiVersionSettings> ApiVersions => this.apiVersions.Values;

		public virtual string CombinedXmlDocPath
		{
			get
			{
				if (this.combinedXmlDocPath.StartsWith("/"))
				{
					return System.Web.Hosting.HostingEnvironment.MapPath(this.combinedXmlDocPath);
				}

				return this.combinedXmlDocPath;
			}
			set => this.combinedXmlDocPath = value;
		}

		public void AddApiVersion(string key, XmlNode node)
		{
			this.AddApiVersion(node);
		}

		public void AddApiVersion(XmlNode node)
		{
			var name = Sitecore.Xml.XmlUtil.GetAttribute(NameAttribute, node)?.Trim();
			var routeTemplate = Sitecore.Xml.XmlUtil.GetAttribute(RouteTemplateAttribute, node)?.Trim();
			var description = Sitecore.Xml.XmlUtil.GetAttribute(DescriptionAttribute, node)?.Trim();

			this.ValidateApiVersionNodeAttribute(name, NameAttribute);
			this.ValidateApiVersionNodeAttribute(routeTemplate, RouteTemplateAttribute);

			if (!this.apiVersions.ContainsKey(name))
			{
				this.apiVersions.Add(name, new ApiVersionSettings(name, routeTemplate, description));
			}
			else
			{
				throw new ConfigurationException($"Api version names should be unique. Unable to add api version node with name '{name}' as it already exists. Please check {SwaggerSettingsNodePath} sitecore configuration node.");
			}
		}

		private void ValidateApiVersionNodeAttribute(string value, string name)
		{
			if (string.IsNullOrEmpty(value))
			{
				throw new ConfigurationException(
					$"Api version node must contain non empty '{name}' attribute. " +
					$"Please check {SwaggerSettingsNodePath} sitecore configuration node.");
			}
		}
	}
}
