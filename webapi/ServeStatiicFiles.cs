﻿using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;

namespace webapi
{
	public static class ServeStaticFiles
	{
		// Serves static files for client UI
		[FunctionName("ui")]
		public static IActionResult Run(
			[HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "ui/{p1?}/{p2?}/{p3?}")] HttpRequest req,
			string p1,
			string p2,
			string p3,
            System.Threading.ExecutionContext context
		)
		{
			string root = context.FunctionAppDirectory;

			// Sanitizing input, just in case
			string path = $"/api/ui/{Path.GetFileName(p1)}";
			path += p2 != null ? $"/{Path.GetFileName(p2)}" : "";
			path += p3 != null ? $"/{Path.GetFileName(p3)}" : "";

			var contentType = FileMap.FirstOrDefault((kv => path.StartsWith(kv[0])));
			if (contentType != null)
			{
				return File.Exists(root + path) ?
					(IActionResult)new FileStreamResult(File.OpenRead(root + path), contentType[1]) :
					new NotFoundResult();
			}
			// Returning index.html by default, to support client routing
			return new FileStreamResult(File.OpenRead($"{root}/api/ui/index.html"), "text/html; charset=UTF-8");
		}

		private static readonly string[][] FileMap = new string[][]
		{
		new [] {"/api/ui/static/css/", "text/css; charset=utf-8"},
		new [] {"/api/ui/static/media/", "image/svg+xml; charset=UTF-8"},
		new [] {"/api/ui/static/js/", "application/javascript; charset=UTF-8"},
		new [] {"/api/ui/manifest.json", "application/json; charset=UTF-8"},
		new [] {"/api/ui/service-worker.js", "application/javascript; charset=UTF-8"},
		new [] {"/api/ui/favicon.ico", "image/x-icon"}
		};
	}
}
