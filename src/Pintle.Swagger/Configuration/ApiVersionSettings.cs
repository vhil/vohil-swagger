namespace Pintle.Swagger.Configuration
{
	public class ApiVersionSettings
	{
		public ApiVersionSettings(string name, string routeTemplate, string description = null)
		{
			this.Name = name;
			this.RouteTemplate = routeTemplate;
			this.Description = description ?? string.Empty;
		}

		public virtual string Name { get; }
		public virtual string RouteTemplate { get; }
		public virtual string Description { get; }
	}
}