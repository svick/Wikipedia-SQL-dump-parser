using System;

namespace WpSqlDumpParser
{
	public enum MediaType
	{
		Unknow,
		Bitmap,
		Drawing,
		Audio,
		Video,
		Multimedia,
		Office,
		Text,
		Executable,
		Archive
	}

	public enum MajorMime
	{
		Unknown,
		Application,
		Audio,
		Image,
		Text,
		Video,
		Message,
		Model,
		Multipart
	}
	
	public class Image
	{
		public string Name { get; protected set; }
		public int Size { get; protected set; }
		public int Width { get; protected set; }
		public int Height { get; protected set; }
		public MediaType MediaType { get; protected set; }
		public MajorMime MajorMime { get; protected set; }
		public string MinorMime { get; protected set; }
		public int UserId { get; protected set; }
		public string UserName { get; protected set; }
		public DateTime Timestamp { get; protected set; }
		public string Sha1 { get; protected set; }

		public Image(string name, int size, int width, int height, MediaType mediaType, MajorMime majorMime, string minorMime, int userId, string userName, DateTime timestamp, string sha1)
		{
			Name = name;
			Size = size;
			Width = width;
			Height = height;
			MediaType = mediaType;
			MajorMime = majorMime;
			MinorMime = minorMime;
			UserId = userId;
			UserName = userName;
			Timestamp = timestamp;
			Sha1 = sha1;
		}
	}
}