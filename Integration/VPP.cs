using StardewModdingAPI;
using System.Reflection;

namespace ProfessionBooks.Integration
{
	public static class VPP
	{
		private const string ID = "id";
		private static bool enabled = false;
		private static IEnumerable<dynamic> data;

		internal static void Init(IModHelper helper)
		{
			if (!helper.ModRegistry.IsLoaded(ID))
				return;

			var asm = helper.ModRegistry.GetApi(ID)!.GetType().Assembly;
			var entry = asm.GetType("ModEntry");

			if (entry is null)
				return;

			var field = entry.GetField("Professions", BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);

			if (field is null)
				return;

			data = (IEnumerable<dynamic>)field.GetValue(null)!;
			enabled = true;
		}

		public static void AddProfessionsForSkill(ref IReadOnlyList<int> existing, int skill)
		{
			if (!enabled)
				return;

			var add = from p in data where p.Skill == skill select p.ID;
			existing = [.. existing, .. add];
		}
	}
}
