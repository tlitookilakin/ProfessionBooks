using ProfessionBooks.Framework;
using ProfessionBooks.Integration;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;

namespace ProfessionBooks
{
	public class ModEntry : Mod
	{
		public override void Entry(IModHelper helper)
		{
			helper.Events.GameLoop.GameLaunched += OnLaunch;
			helper.Events.Multiplayer.ModMessageReceived += NetMessageReceive;
			helper.Events.Multiplayer.PeerConnected += PeerConnected;
			helper.Events.GameLoop.DayStarted += DayStart;
			helper.Events.GameLoop.DayEnding += DayEnd;
		}

		[EventPriority(EventPriority.Low)]
		private void OnLaunch(object? sender, GameLaunchedEventArgs e)
		{
			Skillbook.Init(Helper);
			Assets.Init(Helper);
			VPP.Init(Helper);
			SkillManager.Init(Helper, Monitor);
			Space.Init(Helper);
			ItemQuery.Init(Monitor);
			GSQ.Init();
		}

		private void DayEnd(object? sender, DayEndingEventArgs e)
		{
			SyncPlayer();
		}

		private void DayStart(object? sender, DayStartedEventArgs e)
		{
			SyncPlayer();
		}

		private void SyncPlayer()
		{
			KeyValuePair<long, uint> data = new(Game1.player.UniqueMultiplayerID, Game1.player.stats.Get("MasteryExp"));
			Helper.Multiplayer.SendMessage(data, "MasterySingle", [ModManifest.UniqueID]);
		}

		private void PeerConnected(object? sender, PeerConnectedEventArgs e)
		{
			if (Game1.IsMasterGame)
			{
				Dictionary<long, uint> data = new(
					Game1.otherFarmers.Select(pair => new KeyValuePair<long, uint>(pair.Key, pair.Value.stats.Get("MasteryExp")))
				);
				data[Game1.MasterPlayer.UniqueMultiplayerID] = Game1.MasterPlayer.stats.Get("MasteryExp");
				Helper.Multiplayer.SendMessage(data, "MasteryAll", [ModManifest.UniqueID], [e.Peer.PlayerID]);
			}
		}

		private void NetMessageReceive(object? sender, ModMessageReceivedEventArgs e)
		{
			if (e.FromModID != ModManifest.UniqueID)
				return;

			if (e.Type == "MasterySingle")
			{
				var pair = e.ReadAs<KeyValuePair<long, uint>>();
				SetMastery(pair.Key, pair.Value);
			}
			else if (e.Type == "MasteryAll")
			{
				var map = e.ReadAs<Dictionary<long, uint>>();
				foreach (var pair in map)
					SetMastery(pair.Key, pair.Value);

				SyncPlayer();
			}
		}

		private static void SetMastery(long id, uint amount)
		{
			var who = Game1.GetPlayer(id, true);

			if (who is null || who.IsLocalPlayer)
				return;

			who.stats.Set("MasteryExp", amount);
		}
	}
}
