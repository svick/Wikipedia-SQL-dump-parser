using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace WpSqlDumpParser
{
	public class Images : Dump<Image>
	{
		static Images instance;
		public static Images Instance
		{
			get
			{
				if (instance == null)
					instance = new Images();
				return instance;
			}
		}

		private Images()
		{ }

		public override string Name
		{
			get { return "image"; }
		}

		public override IEnumerable<Image> Get(Stream stream)
		{
			Parser parser = new Parser();
			var result = from values in parser.Parse(stream)
									 select new Image(
										 values["name"].ToString(),
										 values["size"].ToInt32(),
										 values["width"].ToInt32(),
										 values["height"].ToInt32(),
										 values["media_type"].ToEnum<MediaType>(),
										 values["major_mime"].ToEnum<MajorMime>(),
										 values["minor_mime"].ToString(),
										 values["user"].ToInt32(),
										 values["user_text"].ToString(),
										 values["timestamp"].ToDateTime(),
										 values["sha1"].ToString()
										 );

			if (Limiter != null)
				result = Limiter(result);
			return result;

		}
	}
}