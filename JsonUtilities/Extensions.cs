using System.Reflection;
using System.Text.Json;

namespace JsonUtilities;

public static class Extensions
{
	public static JsonElement? GetProperty(this JsonElement element, string propertyName, StringComparer stringComparer)
	{
		if (stringComparer is null)
			throw new ArgumentNullException(nameof(stringComparer));
		foreach (var property in element.EnumerateObject().OfType<JsonProperty>())
		{
			if (stringComparer.Compare(property.Name, propertyName) is 0)
			{
				return property.Value;
			}
		}
		return null;
	}

	public static IEnumerable<MemberInfo> GetFilteredMemberInfos(this MemberInfo[] memberinfos, string[]? ignoredFields, JsonSerializerOptions? options, StringComparer stringComparer)
		=> memberinfos.Where(field => field.Name.IsIncluded(ignoredFields, options, stringComparer));

	public static bool IsIncluded(this string name, string[]? propertiesToIgnoreWhenWriting, JsonSerializerOptions? options, StringComparer stringComparer)
		=> propertiesToIgnoreWhenWriting?.Contains(GetConvertedName(options, name), stringComparer) is false;

	public static string GetConvertedName(this JsonSerializerOptions? options, string name) => options?.PropertyNamingPolicy?.ConvertName(name) ?? name;
}
