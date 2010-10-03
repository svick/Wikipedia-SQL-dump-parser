using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace WpSqlDumpParser
{
	public class Parser
	{
		static readonly int intermediateBufferSize = 2048;
		static readonly int maxTries = 1;
	
		StringBuilder buffer = new StringBuilder();
		StreamReader reader;
		Regex rowRegex;
		int columns;
		ParsedValue[] result;

		public IEnumerable<ParsedValue[]> Parse(Stream stream, int columns)
		{
			this.columns = columns;
			reader = new StreamReader(stream);

			string rowRegexString =
				@"^\(" +
				string.Join(
					",",
					Enumerable.Repeat(@"([\d.]+|'[^']*')", columns)) +
				@"\)";
			rowRegex = new Regex(rowRegexString, RegexOptions.Compiled);

			buffer.Clear();

			readUntilSuccess(removeBeginning);

			while (true)
			{
				readUntilSuccess(parseValues);
				yield return result;
				if (buffer.Length == 0)
					readIntoBuffer();
				if (buffer[0] == ',')
					buffer.Remove(0, 1);
				else
					break;
			}
		}

		void readIntoBuffer()
		{
			char[] chars = new char[intermediateBufferSize];
			int readChars = reader.Read(chars, 0, intermediateBufferSize);
			buffer.Append(chars, 0, readChars);
		}

		void readUntilSuccess(Func<bool> function)
		{
			int i = 0;
			bool success = function();
			while (!success && i < maxTries)
			{
				readIntoBuffer();
				success = function();
				i++;
			}
			if (!success)
				throw new ParseException(function.Method.Name);
		}

		static readonly Regex beginRegex = new Regex(@".*INSERT INTO `\S+` VALUES ", RegexOptions.Compiled | RegexOptions.Singleline);

		bool removeBeginning()
		{
			Match m = beginRegex.Match(buffer.ToString());
			if (m.Success)
				buffer.Remove(0, m.Length);
			return m.Success;
		}

		bool parseValues()
		{
			Match m = rowRegex.Match(buffer.ToString());
			if (m.Success)
			{
				result = new ParsedValue[columns];
				for (int i = 0; i < columns; i++)
					result[i] = new ParsedValue(m.Groups[i].Value);
				buffer.Remove(0, m.Length);
			}
			return m.Success;
		}
	}
}