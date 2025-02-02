using System.Xml.Serialization;

namespace GeneratorsDataProcessor.Helpers;

public static class CommonHelper
{
	public static T DeserializeXml<T>(string filePath)
	{
		using (var reader = new StreamReader(filePath))
		{
			var serializer = new XmlSerializer(typeof(T));
			return (T)serializer.Deserialize(reader);
		}
	}

	public static void SerializeXml<T>(T data, string filePath)
	{
		if (!File.Exists(filePath))
		{
			File.Create(filePath).Dispose();
		}

		using (var writer = new StreamWriter(filePath))
		{
			var serializer = new XmlSerializer(typeof(T));
			serializer.Serialize(writer, data);
		}
	}
}
