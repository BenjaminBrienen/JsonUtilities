using System;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace JsonUtilities;

public class JsonConverterWithArgsAttribute : JsonConverterAttribute
{
    public JsonConverterWithArgsAttribute(Type converterType, params object[] constructorParameters)
        : base(null!)
    {
        ConverterType = converterType;
        ConstructorParameters = constructorParameters;
    }
	
	public override JsonConverter? CreateConverter(Type typeToConvert)
    {
		if (ConverterType is null)
			throw new JsonException();
		var result = Activator.CreateInstance(ConverterType, ConstructorParameters);
		if (result is null)
			throw new JsonException($"{ConverterType.Name} does not have a constructor matching the supplied arguments: {string.Join(", ", ConstructorParameters.Select(o => o.ToString()).ToArray())}");
        return (JsonConverter)result;
    }

    public new Type ConverterType { get; }
    public object[] ConstructorParameters { get; }
}
