using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WpSqlDumpParser
{
	public class Page : IObjectWithId
	{
		public int Id { get; protected set; }
		public int NamespaceId { get; protected set; }
		public string Title { get; protected set; }
		public bool IsRedirect { get; protected set; }

		public Namespace Namespace
		{
			get
			{
				var repo = Repository<Namespace>.Instance;
				if (repo == null)
					throw new InvalidOperationException();
				return repo.FindById(NamespaceId);
			}
		}

		public string FullName
		{
			get
			{
				string namespaceName = Namespace.Name;
				if (namespaceName == "")
					return Title;
				else
					return namespaceName + ':' + Title;
			}
		}

		public Page(int id, int namespaceId, string title, bool isRedirect)
		{
			Id = id;
			NamespaceId = namespaceId;
			Title = title;
			IsRedirect = isRedirect;
		}
	}
}