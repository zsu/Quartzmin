#if ( NETSTANDARD || NETCOREAPP || NET6 )
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace Quartzmin.Helpers;


public class NewtonsoftJsonResult : IActionResult
{
	public int? StatusCode { get; set; }

	private readonly object _value;

	public NewtonsoftJsonResult(object value)
	{
		_value = value;
	}

	public async Task ExecuteResultAsync(ActionContext context)
	{
		var response = context.HttpContext.Response;
		response.ContentType = "application/json; charset=utf-8";

		await using var writer = new StreamWriter(response.Body);
		var serializer = JsonSerializer.Create(new JsonSerializerSettings
		{
			ContractResolver = new DefaultContractResolver(), // PascalCase as default
		});
		serializer.Serialize(writer, _value);
		await writer.FlushAsync();
		
		if (StatusCode != null)
		{
			response.StatusCode = StatusCode.Value;
		}

	}
}
#endif
