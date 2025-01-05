using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;

namespace ProfessionBooks.Framework
{
	internal static class Network
	{
		private static IModHelper Helper;
		private static IManifest ModManifest;

		internal static void Init(IModHelper helper, IManifest manifest)
		{
			Helper = helper;
			ModManifest = manifest;

			helper.Events.Multiplayer.ModMessageReceived += NetMessageReceive;
			helper.Events.Multiplayer.PeerConnected += PeerConnected;
			helper.Events.GameLoop.DayStarted += DayStart;
			helper.Events.GameLoop.DayEnding += DayEnd;
		}

		private static void DayEnd(object? sender, DayEndingEventArgs e)
		{
			SyncPlayer();
		}

		private static void DayStart(object? sender, DayStartedEventArgs e)
		{
			SyncPlayer();
		}

		private static void SyncPlayer()
		{
			KeyValuePair<long, uint> data = new(Game1.player.UniqueMultiplayerID, Game1.player.stats.Get("MasteryExp"));
			Helper.Multiplayer.SendMessage(data, "MasterySingle", [ModManifest.UniqueID]);
		}

		private static void PeerConnected(object? sender, PeerConnectedEventArgs e)
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

		private static void NetMessageReceive(object? sender, ModMessageReceivedEventArgs e)
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
