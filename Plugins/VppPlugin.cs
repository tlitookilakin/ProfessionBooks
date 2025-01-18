using ProfessionBooks.API;
using StardewModdingAPI;
using StardewValley;
using System.Reflection;

namespace ProfessionBooks.Plugins
{
	public class VppPlugin : IPlugin
	{
		private const string ID = "KediDili.VanillaPlusProfessions";
		private static readonly string[] Skills = ["farming", "fishing", "foraging", "mining", "combat"];

		public static Action<string, IList<KeyValuePair<int, Func<string>>>> AddProfession
			= static (s, p) => { };

		internal VppPlugin(IModHelper helper)
		{
			if (!helper.ModRegistry.IsLoaded(ID))
				return;

			var asm = helper.ModRegistry.GetApi(ID)!.GetType().Assembly;
			var entry = asm.GetType("VanillaPlusProfessions.ModEntry");

			if (entry is null)
				return;

			var field = entry.GetField("Professions", BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);

			if (field is null)
				return;

			var data = field.GetValue(null)!;
			var valType = data.GetType().GetGenericArguments()[1];

			AddProfession = typeof(VppPlugin)
				.GetMethod(nameof(AddProfessionsImpl), BindingFlags.Static | BindingFlags.NonPublic)!
				.MakeGenericMethod(valType).CreateDelegate<Action<string, IList<KeyValuePair<int, Func<string>>>>>(data);
		}

		private static void AddProfessionsImpl<T>(Dictionary<string, T> data, string skill, IList<KeyValuePair<int, Func<string>>> Professions)
			where T : class
		{
			int which = Array.IndexOf(Skills, skill);

			if (which == -1)
				return;

			foreach ((var key, dynamic value) in data)
			{
				if (value.Skill == which)
				{
					string name = key;
					Professions.Add(new(value.ID, (Func<string>)(
						() => Game1.content.LoadString("Strings\\UI:LevelUp_ProfessionName_" + name)
					)));
				}
			}
		}

		public void AddProfessions(string skill, IList<KeyValuePair<int, Func<string>>> Professions)
		{
			AddProfession(skill, Professions);
		}
	}
}
