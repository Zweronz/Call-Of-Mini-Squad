using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace Boomlagoon.JSON
{
	public class JSONObject : IEnumerable, IEnumerable<KeyValuePair<string, JSONValue>>
	{
		private enum JSONParsingState
		{
			Object = 0,
			Array = 1,
			EndObject = 2,
			EndArray = 3,
			Key = 4,
			Value = 5,
			KeyValueSeparator = 6,
			ValueSeparator = 7,
			String = 8,
			Number = 9,
			Boolean = 10,
			Null = 11
		}

		private readonly IDictionary<string, JSONValue> values = new Dictionary<string, JSONValue>();

		public JSONValue this[string key]
		{
			get
			{
				return GetValue(key);
			}
			set
			{
				values[key] = value;
			}
		}

		public JSONObject()
		{
		}

		public JSONObject(JSONObject other)
		{
			values = new Dictionary<string, JSONValue>();
			if (other == null)
			{
				return;
			}
			foreach (KeyValuePair<string, JSONValue> value in other.values)
			{
				values[value.Key] = new JSONValue(value.Value);
			}
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return values.GetEnumerator();
		}

		public bool ContainsKey(string key)
		{
			return values.ContainsKey(key);
		}

		public JSONValue GetValue(string key)
		{
			JSONValue value;
			values.TryGetValue(key, out value);
			return value;
		}

		public string GetString(string key)
		{
			JSONValue value = GetValue(key);
			if (value == null)
			{
				return string.Empty;
			}
			return value.Str;
		}

		public double GetNumber(string key)
		{
			JSONValue value = GetValue(key);
			if (value == null)
			{
				return double.NaN;
			}
			return value.Number;
		}

		public JSONObject GetObject(string key)
		{
			JSONValue value = GetValue(key);
			if (value == null)
			{
				return null;
			}
			return value.Obj;
		}

		public bool GetBoolean(string key)
		{
			JSONValue value = GetValue(key);
			if (value == null)
			{
				return false;
			}
			return value.Boolean;
		}

		public JSONArray GetArray(string key)
		{
			JSONValue value = GetValue(key);
			if (value == null)
			{
				return null;
			}
			return value.Array;
		}

		public void Add(string key, JSONValue value)
		{
			values[key] = value;
		}

		public void Add(KeyValuePair<string, JSONValue> pair)
		{
			values[pair.Key] = pair.Value;
		}

		public static JSONObject Parse(string jsonString)
		{
			if (string.IsNullOrEmpty(jsonString))
			{
				return null;
			}
			JSONValue jSONValue = null;
			List<string> list = new List<string>();
			JSONParsingState jSONParsingState = JSONParsingState.Object;
			int num;
			for (num = 0; num < jsonString.Length; num++)
			{
				num = SkipWhitespace(jsonString, num);
				switch (jSONParsingState)
				{
				case JSONParsingState.Object:
				{
					if (jsonString[num] != '{')
					{
						return Fail('{', num);
					}
					JSONValue jSONValue3 = new JSONObject();
					if (jSONValue != null)
					{
						jSONValue3.Parent = jSONValue;
					}
					jSONValue = jSONValue3;
					jSONParsingState = JSONParsingState.Key;
					break;
				}
				case JSONParsingState.EndObject:
					if (jsonString[num] != '}')
					{
						return Fail('}', num);
					}
					if (jSONValue.Parent == null)
					{
						return jSONValue.Obj;
					}
					switch (jSONValue.Parent.Type)
					{
					case JSONValueType.Object:
						jSONValue.Parent.Obj.values[list.Pop()] = new JSONValue(jSONValue.Obj);
						break;
					case JSONValueType.Array:
						jSONValue.Parent.Array.Add(new JSONValue(jSONValue.Obj));
						break;
					default:
						return Fail("valid object", num);
					}
					jSONValue = jSONValue.Parent;
					jSONParsingState = JSONParsingState.ValueSeparator;
					break;
				case JSONParsingState.Key:
				{
					if (jsonString[num] == '}')
					{
						num--;
						jSONParsingState = JSONParsingState.EndObject;
						break;
					}
					string text2 = ParseString(jsonString, ref num);
					if (text2 == null)
					{
						return Fail("key string", num);
					}
					list.Add(text2);
					jSONParsingState = JSONParsingState.KeyValueSeparator;
					break;
				}
				case JSONParsingState.KeyValueSeparator:
					if (jsonString[num] != ':')
					{
						return Fail(':', num);
					}
					jSONParsingState = JSONParsingState.Value;
					break;
				case JSONParsingState.ValueSeparator:
					switch (jsonString[num])
					{
					case ',':
						jSONParsingState = ((jSONValue.Type != JSONValueType.Object) ? JSONParsingState.Value : JSONParsingState.Key);
						break;
					case '}':
						jSONParsingState = JSONParsingState.EndObject;
						num--;
						break;
					case ']':
						jSONParsingState = JSONParsingState.EndArray;
						num--;
						break;
					default:
						return Fail(", } ]", num);
					}
					break;
				case JSONParsingState.Value:
				{
					char c = jsonString[num];
					if (c == '"')
					{
						jSONParsingState = JSONParsingState.String;
					}
					else if (char.IsDigit(c) || c == '-')
					{
						jSONParsingState = JSONParsingState.Number;
					}
					else
					{
						switch (c)
						{
						case '{':
							jSONParsingState = JSONParsingState.Object;
							break;
						case '[':
							jSONParsingState = JSONParsingState.Array;
							break;
						case ']':
							if (jSONValue.Type == JSONValueType.Array)
							{
								jSONParsingState = JSONParsingState.EndArray;
								break;
							}
							return Fail("valid array", num);
						case 'f':
						case 't':
							jSONParsingState = JSONParsingState.Boolean;
							break;
						case 'n':
							jSONParsingState = JSONParsingState.Null;
							break;
						default:
							return Fail("beginning of value", num);
						}
					}
					num--;
					break;
				}
				case JSONParsingState.String:
				{
					string text = ParseString(jsonString, ref num);
					if (text == null)
					{
						return Fail("string value", num);
					}
					switch (jSONValue.Type)
					{
					case JSONValueType.Object:
						jSONValue.Obj.values[list.Pop()] = new JSONValue(text);
						break;
					case JSONValueType.Array:
						jSONValue.Array.Add(text);
						break;
					default:
						return null;
					}
					jSONParsingState = JSONParsingState.ValueSeparator;
					break;
				}
				case JSONParsingState.Number:
				{
					double num2 = ParseNumber(jsonString, ref num);
					if (double.IsNaN(num2))
					{
						return Fail("valid number", num);
					}
					switch (jSONValue.Type)
					{
					case JSONValueType.Object:
						jSONValue.Obj.values[list.Pop()] = new JSONValue(num2);
						break;
					case JSONValueType.Array:
						jSONValue.Array.Add(num2);
						break;
					default:
						return null;
					}
					jSONParsingState = JSONParsingState.ValueSeparator;
					break;
				}
				case JSONParsingState.Boolean:
					if (jsonString[num] == 't')
					{
						if (jsonString.Length < num + 4 || jsonString[num + 1] != 'r' || jsonString[num + 2] != 'u' || jsonString[num + 3] != 'e')
						{
							return Fail("true", num);
						}
						switch (jSONValue.Type)
						{
						case JSONValueType.Object:
							jSONValue.Obj.values[list.Pop()] = new JSONValue(true);
							break;
						case JSONValueType.Array:
							jSONValue.Array.Add(new JSONValue(true));
							break;
						default:
							return null;
						}
						num += 3;
					}
					else
					{
						if (jsonString.Length < num + 5 || jsonString[num + 1] != 'a' || jsonString[num + 2] != 'l' || jsonString[num + 3] != 's' || jsonString[num + 4] != 'e')
						{
							return Fail("false", num);
						}
						switch (jSONValue.Type)
						{
						case JSONValueType.Object:
							jSONValue.Obj.values[list.Pop()] = new JSONValue(false);
							break;
						case JSONValueType.Array:
							jSONValue.Array.Add(new JSONValue(false));
							break;
						default:
							return null;
						}
						num += 4;
					}
					jSONParsingState = JSONParsingState.ValueSeparator;
					break;
				case JSONParsingState.Array:
				{
					if (jsonString[num] != '[')
					{
						return Fail('[', num);
					}
					JSONValue jSONValue2 = new JSONArray();
					if (jSONValue != null)
					{
						jSONValue2.Parent = jSONValue;
					}
					jSONValue = jSONValue2;
					jSONParsingState = JSONParsingState.Value;
					break;
				}
				case JSONParsingState.EndArray:
					if (jsonString[num] != ']')
					{
						return Fail(']', num);
					}
					if (jSONValue.Parent == null)
					{
						return jSONValue.Obj;
					}
					switch (jSONValue.Parent.Type)
					{
					case JSONValueType.Object:
						jSONValue.Parent.Obj.values[list.Pop()] = new JSONValue(jSONValue.Array);
						break;
					case JSONValueType.Array:
						jSONValue.Parent.Array.Add(new JSONValue(jSONValue.Array));
						break;
					default:
						return Fail("valid object", num);
					}
					jSONValue = jSONValue.Parent;
					jSONParsingState = JSONParsingState.ValueSeparator;
					break;
				case JSONParsingState.Null:
					if (jsonString[num] == 'n')
					{
						if (jsonString.Length < num + 4 || jsonString[num + 1] != 'u' || jsonString[num + 2] != 'l' || jsonString[num + 3] != 'l')
						{
							return Fail("null", num);
						}
						switch (jSONValue.Type)
						{
						case JSONValueType.Object:
							jSONValue.Obj.values[list.Pop()] = new JSONValue(JSONValueType.Null);
							break;
						case JSONValueType.Array:
							jSONValue.Array.Add(new JSONValue(JSONValueType.Null));
							break;
						default:
							return null;
						}
						num += 3;
					}
					jSONParsingState = JSONParsingState.ValueSeparator;
					break;
				}
			}
			return null;
		}

		private static int SkipWhitespace(string str, int pos)
		{
			while (pos < str.Length && char.IsWhiteSpace(str[pos]))
			{
				pos++;
			}
			return pos;
		}

		private static string ParseString(string str, ref int startPosition)
		{
			if (str[startPosition] != '"' || startPosition + 1 >= str.Length)
			{
				Fail('"', startPosition);
				return null;
			}
			int num = str.IndexOf('"', startPosition + 1);
			if (num <= startPosition)
			{
				Fail('"', startPosition + 1);
				return null;
			}
			while (str[num - 1] == '\\')
			{
				num = str.IndexOf('"', num + 1);
				if (num <= startPosition)
				{
					Fail('"', startPosition + 1);
					return null;
				}
			}
			string result = string.Empty;
			if (num > startPosition + 1)
			{
				result = str.Substring(startPosition + 1, num - startPosition - 1);
			}
			startPosition = num;
			return result;
		}

		private static double ParseNumber(string str, ref int startPosition)
		{
			if (startPosition >= str.Length || (!char.IsDigit(str[startPosition]) && str[startPosition] != '-'))
			{
				return double.NaN;
			}
			int i;
			for (i = startPosition + 1; i < str.Length && str[i] != ',' && str[i] != ']' && str[i] != '}'; i++)
			{
			}
			double result;
			if (!double.TryParse(str.Substring(startPosition, i - startPosition), NumberStyles.Float, CultureInfo.InvariantCulture, out result))
			{
				return double.NaN;
			}
			startPosition = i - 1;
			return result;
		}

		private static JSONObject Fail(char expected, int position)
		{
			return Fail(new string(expected, 1), position);
		}

		private static JSONObject Fail(string expected, int position)
		{
			return null;
		}

		public override string ToString()
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.Append('{');
			foreach (KeyValuePair<string, JSONValue> value in values)
			{
				stringBuilder.Append("\"" + value.Key + "\"");
				stringBuilder.Append(':');
				stringBuilder.Append(value.Value.ToString());
				stringBuilder.Append(',');
			}
			if (values.Count > 0)
			{
				stringBuilder.Remove(stringBuilder.Length - 1, 1);
			}
			stringBuilder.Append('}');
			return stringBuilder.ToString();
		}

		public IEnumerator<KeyValuePair<string, JSONValue>> GetEnumerator()
		{
			return values.GetEnumerator();
		}

		public void Clear()
		{
			values.Clear();
		}

		public void Remove(string key)
		{
			if (values.ContainsKey(key))
			{
				values.Remove(key);
			}
		}
	}
}
