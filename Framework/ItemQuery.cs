using StardewModdingAPI;
using StardewValley.Extensions;
using StardewValley.Internal;

namespace ProfessionBooks.Framework
{
	public static class ItemQuery
	{
		private static IMonitor Monitor;

		internal static void Init(IMonitor monitor)
		{
			Monitor = monitor;
			ItemQueryResolver.Register("ProfessionBooks_Book", BookItem);
		}

		public static IEnumerable<ItemQueryResult> BookItem(
			string key, string arguments, ItemQueryContext context, bool avoidRepeat, HashSet<string> avoidItemIds, Action<string, string> logError
		)
		{
			var which = arguments.Trim();

			if (which.EqualsIgnoreCase("all") || which.EqualsIgnoreCase("any") || which == "")
			{
				return SkillManager.AllSkills().Select(p => new ItemQueryResult(new Skillbook(p)));
			}

			if (!SkillManager.HasSkill(which))
				return ItemQueryResolver.Helpers.ErrorResult(
					key, arguments, 
					(q, m) => Monitor.Log($"Error in query '{q}': {m}", LogLevel.Warn), 
					$"Skill '{which}' does not exist"
				);

			return [new(new Skillbook(which))];
		}
	}
}
