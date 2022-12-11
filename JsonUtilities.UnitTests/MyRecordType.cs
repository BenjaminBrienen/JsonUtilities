namespace JsonUtilities.UnitTests;

#pragma warning disable CA1051

[JsonConverterWithArgs(
	typeof(IgnorePropertiesJsonConverter<MyRecordType>),
	new string[] { nameof(MyProperty1) },
	new string[] { nameof(MyProperty2) }
)]
public record MyRecordType(string? MyProperty1 = default, string? MyProperty2 = default)
{
	public string? MyField;
}
