using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WpSqlDumpParser
{
	class Namespace : IObjectWithId
	{
		public int Id { get; protected set; }
		public string Name { get; protected set; }

		public Namespace(int id, string name)
		{
			Id = id;
			Name = name;
		}
	}
}