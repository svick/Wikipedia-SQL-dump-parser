using System.Collections.Generic;

namespace WpCategoryCycles
{
	class Category
	{
		public string Title { get; protected set; }
		public IList<Category> Children { get; protected set; }

		public Category(string title)
		{
			Title = title;
			Children = new List<Category>();
		}
	}
}