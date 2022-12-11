using System.Globalization;
using System.Text.Json;
using System.Text.Json.Serialization;
using RestSharp;
using RestSharp.Serializers.Json;

namespace JsonUtilities;

public class SnakeCaseNamingPolicy : JsonNamingPolicy
{
	public static Func<SystemTextJsonSerializer> SnakeCaseSerializer
		=> () => s_snakeCaseSerializer;

	private static readonly SystemTextJsonSerializer s_snakeCaseSerializer
		= new SystemTextJsonSerializer(
			new JsonSerializerOptions
			{ PropertyNamingPolicy = new SnakeCaseNamingPolicy() });

#pragma warning disable CA1308
	public override string ConvertName(string name) =>
		string.Concat(name.Select((x, i) => i > 0 && char.IsUpper(x) ? "_" + x : x.ToString())).ToLowerInvariant();
}
