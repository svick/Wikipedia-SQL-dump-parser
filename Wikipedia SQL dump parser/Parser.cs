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
		Dictionary<string, ParsedValue> result;
		List<string> columns;

		public IEnumerable<IDictionary<string, ParsedValue>> Parse(Stream stream)
		{
			reader = new StreamReader(stream);

			buffer.Clear();

			readUntilSuccess(removeCreateTableBeginning);

			columns = new List<string>();

			while (true)
				try
				{
					readUntilSuccess(parseColumnDefinition);
				}
				catch (ParseException)
				{
					break;
				}

			string rowRegexString =
				@"^\(" +
				string.Join(
					",",
					Enumerable.Repeat(@"(-?[\d.]+|[\d.]+e-?\d+|'.*?')", columns.Count)) +
				@"\)";
			rowRegex = new Regex(rowRegexString, RegexOptions.Compiled);

			while (true)
			{
				try
				{
					readUntilSuccess(removeInsertBeginning);
				}
				catch (ParseException)
				{
					break;
				}

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

			reader.Close();
			reader = null;
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