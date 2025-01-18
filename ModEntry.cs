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

		private void OnLaunch(object? sender, GameLaunchedEventArgs e)
		{
			Network.Init(Helper, ModManifest);
			API.API.Init(Helper);
			Skillbook.Init(Helper);
			Assets.Init(Helper);
			SpacePlugin.Init(Helper);
			ItemQuery.Init(Monitor);
			GSQ.Init();

			Helper.Events.GameLoop.OneSecondUpdateTicked += LateInit;
		}

		[EventPriority(EventPriority.Low)]
		private void LateInit(object? sender, OneSecondUpdateTickedEventArgs e)
		{
			Helper.Events.GameLoop.OneSecondUpdateTicked -= LateInit;

			// must be here to make sure all skills are registered
			SkillManager.Init();
		}

		public override object? GetApi()
		{
			return API.API.api;
		}
	}
}
