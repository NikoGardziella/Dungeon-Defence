

namespace DungeonDefence
{
	using System.Xml.Serialization;
	using System.IO;
	using System.Collections.Generic;
	using System;
	public static class Data
	{
		public const int minGoldCollect = 10;
		public const int minElixirCollect = 10;
		public const int minDarkElixirCollect = 10;
		public enum BuildingID
		{
			townhall, goldmine, goldstorage , elixirmine, elixirstorage, darkelixirmine, darkelixirstorage, buildershut
		}
		public class Player
		{
			public int gold = 0;
			public int elixir = 0;
			public int gems = 0;
			public DateTime nowTime;
			public List<Building> buildings = new List<Building>();
		}

		public class Building
		{
			public BuildingID id = BuildingID.townhall;

			public int level = 0;
			public long databaseID = 0;
			public int x = 0;
			public int y = 0;
			public int columns = 0;
			public int rows = 0;
			public int storage = 0;
			public DateTime boost;
			public int health = 100;
			public float damage = 0;
			public int capacity = 0;
			public float speed = 0;
			public float radius = 0;
			public DateTime constructionTime;
			public bool isConstructing = false;
			public int buildTime = 0;
		}


		
		public class ServerBuilding
		{
			public string id = "";
			public int level = 0;
			public long databaseID = 0;
			public int requiredElixir = 0;
			public int requiredGold = 0;
			public int requiredGems = 0;
			public int requiredDarkElixir = 0;
			public int columns = 0;
			public int rows = 0;
			public int buildTime = 0;
			public int gainedXP = 0;
		}

		public static string Serialize<T>(this T target)
		{
			XmlSerializer xml = new XmlSerializer(typeof(T));
			StringWriter writer = new StringWriter();
			xml.Serialize(writer, target);
			return writer.ToString();
		}

		public static T Deserialize<T>(this string target)
		{
			XmlSerializer xml = new XmlSerializer(typeof(T));
			StringReader reader = new StringReader(target);
			return (T)xml.Deserialize(reader);
		}
	}
}

