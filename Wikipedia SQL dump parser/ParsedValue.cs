using System;

namespace WpSqlDumpParser
{
	public class ParsedValue
	{
		string value;

		public ParsedValue(string value)
		{
			this.value = value;
		}

		public override string ToString()
		{
			if (value[0] == '\'' && value[value.Length - 1] == '\'')
				return value.Substring(1, value.Length - 2);
			else
				throw new InvalidOperationException();
		}

		public int ToInt32()
		{
			return int.Parse(value);
		}

		public double ToDouble()
		{
			return double.Parse(value, System.Globalization.CultureInfo.InvariantCulture);
		}

		public bool ToBoolean()
		{
			switch (ToInt32())
			{
			case 0:
				return false;
			case 1:
				return true;
			default:
				throw new InvalidOperationException();
			}
		}

		public T ToEnum<T>() where T : struct
		{
			return (T)Enum.Parse(typeof(T), ToString(), true);
		}

		public DateTime ToDateTime()
		{
			return DateTime.ParseExact(ToString(), "yyyyMMddHHmmss", System.Globalization.CultureInfo.InvariantCulture);
		}
	}
}