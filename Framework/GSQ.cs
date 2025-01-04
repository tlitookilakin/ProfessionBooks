using StardewValley;
using StardewValley.Delegates;

namespace ProfessionBooks.Framework
{
	public static class GSQ
	{
		internal static void Init()
		{
			GameStateQuery.Register("ProfessionBooks_Available", IsAvailable);
		}

		public static bool IsAvailable(string[] query, GameStateQueryContext context)
		{
			var which = query.Length < 1 ? "Any" : query[0];
			var player = query.Length < 2 ? "Current" : query[1];

			return GameStateQuery.Helpers.WithPlayer(context.Player, query[1],
				who => who.stats.Get("MasteryExp") != 0 && (
					which.Equals("any", StringComparison.OrdinalIgnoreCase) ?
					SkillManager.GetAllUnowned(who).Any() :
					SkillManager.GetUnownedForSkill(which, who).Any()
				)
			);
		}
	}
}
