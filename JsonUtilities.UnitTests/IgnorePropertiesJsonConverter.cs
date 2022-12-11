using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Text.Json;

namespace JsonUtilities.UnitTests;

[TestClass]
public partial class UnitTest1
{
	private readonly JsonSerializerOptions _options = new()
	{
		PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
		PropertyNameCaseInsensitive = true,
		ReadCommentHandling = JsonCommentHandling.Skip,
		AllowTrailingCommas = true,
		WriteIndented = true,
		DictionaryKeyPolicy = JsonNamingPolicy.CamelCase,
		IncludeFields = true
	};

	[TestMethod]
	public void TestMethod1()
	{
		var myRecord = new MyRecordType("Test1", "Test2") { MyField = "Test3" };
		var myRecordSerialized = JsonSerializer.Serialize(myRecord, _options);
		Assert.AreEqual(@"{""myProperty1"": ""Test1"",""myField"": ""Test3""}", myRecordSerialized);
	}

	[TestMethod]
	public void TestMethod2()
	{
		var myRecordJson = @"{""myProperty1"":""Test1"",""myProperty2"":""Test2"",""MyField"":""Test3""}";
		var myRecordDeserialized = JsonSerializer.Deserialize<MyRecordType>(myRecordJson, _options);
		Assert.IsNotNull(myRecordDeserialized);
		Assert.AreEqual("MyRecordType { MyProperty1 = , MyProperty2 = Test2, MyField = Test3 }", myRecordDeserialized.ToString());
	}

}
