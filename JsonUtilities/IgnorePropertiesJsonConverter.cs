using System.Reflection;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Runtime.Serialization;

namespace JsonUtilities;

public class IgnorePropertiesJsonConverter<T> : JsonConverter<T?> where T : class
{
	private readonly string[]? _propertiesToIgnoreWhenReading;
	private readonly string[]? _propertiesToIgnoreWhenWriting;

	public IgnorePropertiesJsonConverter() : this(null, null) { }

	public IgnorePropertiesJsonConverter(string[]? propertiesToIgnoreWhenReading, string[]? propertiesToIgnoreWhenWriting)
	{
		_propertiesToIgnoreWhenReading = propertiesToIgnoreWhenReading;
		_propertiesToIgnoreWhenWriting = propertiesToIgnoreWhenWriting;
	}

	public override T? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
	{
		if (reader.TokenType == JsonTokenType.Null)
		{
			return null;
		}

		var condition = BindingFlags.Public | BindingFlags.Instance;
		var stringComparer = options?.PropertyNameCaseInsensitive is true
			? StringComparer.OrdinalIgnoreCase
			: StringComparer.Ordinal;

		// Create an instance of T using the default constructor
		var t = (T)FormatterServices.GetUninitializedObject(typeof(T));
		var deserialized = JsonSerializer.Deserialize<JsonElement>(ref reader, options);

		// Get all properties that should be set
		var filteredProperties = typeof(T).GetProperties(condition).GetFilteredMemberInfos(_propertiesToIgnoreWhenReading, options, stringComparer).Cast<PropertyInfo>();

		// Deserialize the properties
		foreach (var property in filteredProperties)
		{
			var gottenProperty = deserialized.GetProperty(options.GetConvertedName(property.Name), stringComparer);
			if (gottenProperty.HasValue is false)
			{
				continue;
			}
			property.SetValue(t, gottenProperty.Value.GetString());
		}

		if (options?.IncludeFields is false)
		{
			return t;
		}

		// Get all fields that should be set
		var filteredFields = typeof(T).GetFields(condition).GetFilteredMemberInfos(_propertiesToIgnoreWhenReading, options, stringComparer).Cast<FieldInfo>();

		foreach (var field in filteredFields)
		{
			if (deserialized.GetProperty(options.GetConvertedName(field.Name), stringComparer) is JsonElement jsonElement)
			{
				field.SetValue(t, jsonElement.GetString());
			}
		}

		// Return the instance of T with the specified properties set to their default values
		return t;
	}


	public override void Write(Utf8JsonWriter writer, T? value, JsonSerializerOptions options)
	{
		if (writer is null)
			throw new ArgumentNullException(nameof(writer));

		writer.WriteStartObject();
		var condition = BindingFlags.Public | BindingFlags.Instance;
		var stringComparer = options?.PropertyNameCaseInsensitive is true
			? StringComparer.OrdinalIgnoreCase
			: StringComparer.Ordinal;

		{
			// Get all properties that should be written to
			var filteredProperties = typeof(T).GetProperties(condition).GetFilteredMemberInfos(_propertiesToIgnoreWhenWriting, options, stringComparer).Cast<PropertyInfo>();

			foreach (var property in filteredProperties)
			{
				var propertyValue = property.GetValue(value);
				writer.WritePropertyName(options?.PropertyNamingPolicy?.ConvertName(property.Name) ?? property.Name);
				JsonSerializer.Serialize(writer, propertyValue, property.PropertyType, options);
			}
		}
		// Get all fields that should be written to
		if (options?.IncludeFields is true)
		{
			var filteredFields = typeof(T).GetFields(condition).GetFilteredMemberInfos(_propertiesToIgnoreWhenWriting, options, stringComparer).Cast<FieldInfo>();
			foreach (var field in filteredFields)
			{
				var fieldValue = field.GetValue(value);
				writer.WritePropertyName(options?.PropertyNamingPolicy?.ConvertName(field.Name) ?? field.Name);
				JsonSerializer.Serialize(writer, fieldValue, field.FieldType, options);
			}
		}
		writer.WriteEndObject();
	}
}
