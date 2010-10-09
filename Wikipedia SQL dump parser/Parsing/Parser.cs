using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace WpSqlDumpParser.Parsing
{
	public class Parser
	{
		static readonly int intermediateBufferSize = 2048;
		static readonly int parseValuesMaxTries = 13;
	
		StringBuilder buffer = new StringBuilder();
		StreamReader reader;
		Regex rowRegex;
		Dictionary<string, ParsedValue> result;
		List<string> columns;

		public IEnumerable<IDictionary<string, ParsedValue>> Parse(Stream stream)
		{
			reader = new StreamReader(stream);

			buffer.Clear();

			readUntilSuccess(removeCreateTableBeginning, 1);

			columns = new List<string>();

			while (true)
				try
				{
					readUntilSuccess(parseColumnDefinition, 1);
				}
				catch (ParseException)
				{
					break;
				}

			string rowRegexString =
				@"^\(" +
				string.Join(
					",",
					Enumerable.Repeat(@"(-?[\d.]+|[\d.]+e-?\d+|'(?:|[^']*(?:[^'\\]|\\\\))(?:\\'(?:|[^']*(?:[^'\\]|\\\\)))*')", columns.Count)) +
				@"\)";
			rowRegex = new Regex(rowRegexString, RegexOptions.Compiled | RegexOptions.Singleline);

			while (true)
			{
				try
				{
					readUntilSuccess(removeInsertBeginning, 1);
				}
				catch (ParseException)
				{
					break;
				}

				while (true)
				{
					readUntilSuccess(parseValues, parseValuesMaxTries);
					yield return result;
					if (buffer.Length == 0)
						readIntoBuffer(0);
					if (buffer[0] == ',')
						buffer.Remove(0, 1);
					else
						break;
				}
			}

			reader.Close();
			reader = null;
		}

		bool readIntoBuffer(int i)
		{
			int size = intermediateBufferSize << i;
			char[] chars = new char[size];
			int readChars = reader.Read(chars, 0, size);
			buffer.Append(chars, 0, readChars);
			return readChars > 0;
		}

		void readUntilSuccess(Func<bool> function, int maxTries)
		{
			int i = 0;
			bool success = function();
			while (!success && i < maxTries)
			{
				if (!readIntoBuffer(i))
					break;
				success = function();
				i++;
			}
			if (!success)
				throw new ParseException(function.Method.Name);
		}

		bool removeBeginning(Regex beginningRegex)
		{
			Match m = beginningRegex.Match(buffer.ToString());
			if (m.Success)
				buffer.Remove(0, m.Length);
			return m.Success;
		}

		static readonly Regex insertBeginningRegex = new Regex(@"^.*INSERT INTO `\S+` VALUES ", RegexOptions.Compiled | RegexOptions.Singleline);

		bool removeInsertBeginning()
		{
			return removeBeginning(insertBeginningRegex);
		}

		static readonly Regex createTableBeginningRegex = new Regex(@"^.*CREATE TABLE `\S+` \(", RegexOptions.Compiled | RegexOptions.Singleline);

		bool removeCreateTableBeginning()
		{
			return removeBeginning(createTableBeginningRegex);
		}

		bool parseValues()
		{
			Match m = rowRegex.Match(buffer.ToString());
			if (m.Success)
			{
				result = new Dictionary<string, ParsedValue>(columns.Count);
				for (int i = 0; i < columns.Count; i++)
				{
					result[columns[i]] = new ParsedValue(m.Groups[i + 1].Value);
				}
				buffer.Remove(0, m.Length);
			}
			return m.Success;
		}

		static readonly Regex columnDefinitionRegex = new Regex(@"^\s*`([a-z0-9_]+)`[^`]+(?:,|\))", RegexOptions.Compiled | RegexOptions.Singleline);

		bool parseColumnDefinition()
		{
			Match m = columnDefinitionRegex.Match(buffer.ToString());
			if (m.Success)
			{
				string fullColumnName = m.Groups[1].Value;
				int firstUnderscore = fullColumnName.IndexOf('_');
				string columnName = fullColumnName.Substring(firstUnderscore + 1);
				columns.Add(columnName);
				buffer.Remove(0, m.Length);
			}
			return m.Success;
		}
	}
}