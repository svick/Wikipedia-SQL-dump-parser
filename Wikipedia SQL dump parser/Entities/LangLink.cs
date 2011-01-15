using System;
using WpSqlDumpParser.EntityCollections;

namespace WpSqlDumpParser.Entities
{
	class LangLink
	{
		public int FromId { get; protected set; }
		public string Lang { get; protected set; }
		public string Title { get; protected set; }

		public Page From
		{
			get
			{
				var repo = Repository<Page>.Instance;
				if (repo == null)
					throw new InvalidOperationException();
				return repo.FindById(FromId);
			}
		}

		public LangLink(int fromId, string lang, string title)
		{
			FromId = fromId;
			Lang = lang;
			Title = title;
		}
	}
}