namespace Boomlagoon.JSON
{
	public class JSONValue
	{
		public JSONValueType Type { get; private set; }

		public string Str { get; set; }

		public double Number { get; set; }

		public JSONObject Obj { get; set; }

		public JSONArray Array { get; set; }

		public bool Boolean { get; set; }

		public JSONValue Parent { get; set; }

		public JSONValue(JSONValueType type)
		{
			Type = type;
		}

		public JSONValue(string str)
		{
			Type = JSONValueType.String;
			Str = str;
		}

		public JSONValue(double number)
		{
			Type = JSONValueType.Number;
			Number = number;
		}

		public JSONValue(JSONObject obj)
		{
			if (obj == null)
			{
				Type = JSONValueType.Null;
				return;
			}
			Type = JSONValueType.Object;
			Obj = obj;
		}

		public JSONValue(JSONArray array)
		{
			Type = JSONValueType.Array;
			Array = array;
		}

		public JSONValue(bool boolean)
		{
			Type = JSONValueType.Boolean;
			Boolean = boolean;
		}

		public JSONValue(JSONValue value)
		{
			Type = value.Type;
			switch (Type)
			{
			case JSONValueType.String:
				Str = value.Str;
				break;
			case JSONValueType.Boolean:
				Boolean = value.Boolean;
				break;
			case JSONValueType.Number:
				Number = value.Number;
				break;
			case JSONValueType.Object:
				if (value.Obj != null)
				{
					Obj = new JSONObject(value.Obj);
				}
				break;
			case JSONValueType.Array:
				Array = new JSONArray(value.Array);
				break;
			}
		}

		public override string ToString()
		{
			switch (Type)
			{
			case JSONValueType.Object:
				return Obj.ToString();
			case JSONValueType.Array:
				return Array.ToString();
			case JSONValueType.Boolean:
				return (!Boolean) ? "false" : "true";
			case JSONValueType.Number:
				return Number.ToString();
			case JSONValueType.String:
				return "\"" + Str + "\"";
			case JSONValueType.Null:
				return "null";
			default:
				return "null";
			}
		}

		public static implicit operator JSONValue(string str)
		{
			return new JSONValue(str);
		}

		public static implicit operator JSONValue(double number)
		{
			return new JSONValue(number);
		}

		public static implicit operator JSONValue(JSONObject obj)
		{
			return new JSONValue(obj);
		}

		public static implicit operator JSONValue(JSONArray array)
		{
			return new JSONValue(array);
		}

		public static implicit operator JSONValue(bool boolean)
		{
			return new JSONValue(boolean);
		}
	}
}
