namespace Pintle.Swagger.HttpHandlers
{
	using System;
	using System.Net;
	using System.Net.Http;
	using System.Threading;
	using System.Threading.Tasks;
	using Sitecore;

	public class SwaggerSitecoreAuthMessageHandler : DelegatingHandler
	{
		protected override Task<HttpResponseMessage> SendAsync(
			HttpRequestMessage request, 
			CancellationToken cancellationToken)
		{
			var isAuthenticated = Context.User?.IsAuthenticated ?? false;
			var isAdministrator = Context.User?.IsAdministrator ?? false;

			if (this.IsSwaggerRequest(request) && (!isAuthenticated || !isAdministrator))
			{
				var uri = request.RequestUri;
				var currentOrigin = uri.AbsoluteUri.Replace(uri.PathAndQuery, string.Empty);
				var logiPage = currentOrigin + @"/sitecore/login?returnurl=/swagger";

				var response = new HttpResponseMessage(HttpStatusCode.Redirect);
				response.Headers.Location = new Uri(logiPage);

				return Task.FromResult(response);
			}
			else
			{
				return base.SendAsync(request, cancellationToken);
			}
		}

		private bool IsSwaggerRequest(HttpRequestMessage request)
		{
			return request.RequestUri.PathAndQuery.ToLowerInvariant().StartsWith("/swagger");
		}
	}
}
