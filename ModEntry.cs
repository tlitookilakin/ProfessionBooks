using ProfessionBooks.Framework;
using ProfessionBooks.Plugins;
using StardewModdingAPI;
using StardewModdingAPI.Events;

namespace ProfessionBooks
{
    public class ModEntry : Mod
	{
		public override void Entry(IModHelper helper)
		{
			helper.Events.GameLoop.GameLaunched += OnLaunch;
		}

		[EventPriority(EventPriority.Low)]
		private void OnLaunch(object? sender, GameLaunchedEventArgs e)
		{
			Network.Init(Helper, ModManifest);
			API.API.Init(Helper);
			Skillbook.Init(Helper);
			Assets.Init(Helper);
			SpacePlugin.Init(Helper);
			SkillManager.Init();
			ItemQuery.Init(Monitor);
			GSQ.Init();
		}

		public override object? GetApi()
		{
			return API.API.api;
		}
	}
}
