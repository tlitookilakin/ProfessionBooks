namespace ProfessionBooks.Framework
{
	internal class ModUtilities
	{
		private static readonly string[] profession_names = [
			"Rancher", "Tiller", "Coopmaster", "Shepherd", "Artisan", "Agriculturist", "Fisher",
			"Trapper", "Angler", "Pirate", "Mariner", "Luremaster", "Forester", "Gatherer",
			"Lumberjack", "Tapper", "Botanist", "Tracker", "Miner", "Geologist", "Blacksmith",
			"Prospector", "Excavator", "Gemologist", "Fighter", "Scout", "Brute", "Defender",
			"Acrobat", "Desperado"
		];

		public static string GetProfessionName(int which)
		{
			return profession_names[Math.Clamp(which, 0, profession_names.Length)];
		}
	}
}
