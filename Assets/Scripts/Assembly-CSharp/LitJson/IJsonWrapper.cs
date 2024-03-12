using System.Collections;
using System.Collections.Specialized;

namespace LitJson
{
	public interface IJsonWrapper : IDictionary, IList, ICollection, IEnumerable, IOrderedDictionary
	{
		bool IsArray { get; }

		bool IsBoolean { get; }

		bool IsFloat { get; }

		bool IsDouble { get; }

		bool IsInt { get; }

		bool IsLong { get; }

		bool IsObject { get; }

		bool IsString { get; }

		bool GetBoolean();

		float GetFloat();

		double GetDouble();

		int GetInt();

		JsonType GetJsonType();

		long GetLong();

		string GetString();

		void SetBoolean(bool val);

		void SetFloat(float val);

		void SetDouble(double val);

		void SetInt(int val);

		void SetJsonType(JsonType type);

		void SetLong(long val);

		void SetString(string val);

		string ToJson();

		void ToJson(JsonWriter writer);
	}
}
