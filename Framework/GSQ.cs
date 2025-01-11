using StardewValley;
using StardewValley.Delegates;
using StardewValley.Extensions;

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
			var which = query.Length < 2 ? "Any" : query[1];
			var player = query.Length < 3 ? "Target" : query[2];

			return GameStateQuery.Helpers.WithPlayer(context.Player, player,
				who => who.stats.Get("MasteryExp") != 0 && (

					(which.EqualsIgnoreCase("any") && 
						SkillManager.GetAllUnowned(who).Any()) ||

					(which.EqualsIgnoreCase("this") && context.TargetItem is Skillbook book && 
						SkillManager.GetUnownedForSkill(book.SkillId, who).Any()) ||

					SkillManager.GetUnownedForSkill(which, who).Any()
				)
			);
		}
	}
}
