using ProfessionBooks.Plugins;
using StardewModdingAPI;

namespace ProfessionBooks.API
{
	internal class API : IAPI
	{
		private static readonly List<IPlugin> Plugins = [];
		internal static API api = new();

		internal static void Init(IModHelper helper)
		{
			api.RegisterPlugin(new VanillaPlugin());
			api.RegisterPlugin(new SpacePlugin());
			api.RegisterPlugin(new VppPlugin(helper));
		}

		public void RegisterPlugin(IPlugin plugin)
		{
			Plugins.Add(plugin);
		}

		internal static void AddSkills(IDictionary<string, ISkill> skills)
		{
			foreach (var plugin in Plugins)
				plugin.AddSkills(skills);
		}

		internal static void AddProfessions(string skill, IList<KeyValuePair<int, Func<string>>> professions)
		{
			foreach (var plugin in Plugins)
				plugin.AddProfessions(skill, professions);
		}
	}
}
