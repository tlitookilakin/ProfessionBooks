using ProfessionBooks.Framework;
using SpaceCore;
using StardewModdingAPI;
using StardewValley;

namespace ProfessionBooks.Integration
{
	internal static class Space
	{
		private static IApi api;

		internal static void Init(IModHelper helper)
		{
			api = (IApi)helper.ModRegistry.GetApi("spacechase0.SpaceCore")!;

			api.RegisterSerializerType(typeof(Skillbook));
			SkillManager.AddSkills(GetSkills());
		}

		public static IEnumerable<KeyValuePair<string, ISkill>> GetSkills()
		{
			return Skills.GetSkillList().Select(
				name => new KeyValuePair<string, ISkill>(name, new SpaceSkill(Skills.GetSkill(name))
			));
		}

		public static void AddXp(Farmer who, string skill, int amount)
		{
			api.AddExperienceForCustomSkill(who, skill, amount);
		}
	}
}
