using System.Text;
using System.Text.Json;

namespace IntegrationTests;

public class JsonHttpContent : StringContent
{
	public JsonHttpContent(object contents)
		: base(JsonSerializer.Serialize(contents), Encoding.UTF8, "application/json")
	{
	}
}