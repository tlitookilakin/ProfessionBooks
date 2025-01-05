using ProfessionBooks.API;
using StardewModdingAPI;
using StardewValley;
using System.Reflection;

namespace ProfessionBooks.Plugins
{
	public class VppPlugin : IPlugin
	{
		private const string ID = "KediDili.VanillaPlusProfessions";
		private bool enabled = false;
		private IEnumerable<dynamic> data = null!;
		private static readonly string[] Skills = ["farming", "fishing", "foraging", "mining", "combat"];

		internal VppPlugin(IModHelper helper)
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

		public void AddProfessions(string skill, IList<KeyValuePair<int, Func<string>>> Professions)
		{
			if (!enabled)
				return;

			int which = Array.IndexOf(Skills, skill);

			if (which == -1)
				return;

			foreach (var pair in data) 
			{
				if (pair.Value.Skill == which) 
				{
					string name = pair.Key;
					Professions.Add(new(pair.Value.ID, (Func<string>)(
						() => Game1.content.LoadString("Strings\\UI:LevelUp_ProfessionName_" + name)
					)));
				}
			}
		}
	}
}
