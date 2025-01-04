using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ProfessionBooks.Integration;
using StardewModdingAPI;
using StardewValley;

namespace ProfessionBooks.Framework
{
	public static class SkillManager
	{
		private static IMonitor Monitor = null!;
		private static IModHelper Helper = null!;
		private static readonly Dictionary<string, ISkill> Skills = new(StringComparer.OrdinalIgnoreCase);
		private static readonly string[] vanillaSkills = ["farming", "fishing", "foraging", "mining", "combat"];

		internal static void Init(IModHelper helper, IMonitor monitor)
		{
			Monitor = monitor;
			Helper = helper;

			for (int i = 0; i < vanillaSkills.Length; i++)
				Skills[vanillaSkills[i]] = new VanillaSkill(i);
		}

		public static void AddSkills(IEnumerable<KeyValuePair<string, ISkill>> toAdd)
		{
			foreach (var pair in toAdd)
				Skills.TryAdd(pair.Key, pair.Value);
		}

		public static Action<SpriteBatch, Rectangle, Color, float> GetIconDraw(string skill)
		{
			if (!Skills.TryGetValue(skill, out var sk))
				return static (s, r, c, d) => { };

			return sk.DrawIcon;
		}

		public static void AddXpToSkill(string which, Farmer who, int amount)
		{
			int ind = Array.IndexOf(vanillaSkills, which);
			if (ind >= 0)
				who.gainExperience(ind, amount);
			else
				Space.AddXp(who, which, amount);
		}

		public static IEnumerable<int> GetUnownedForSkill(string skill, Farmer who)
		{
			if (!Skills.TryGetValue(skill, out var sk))
				return [];

			return sk.Professions.Where(p => !who.professions.Contains(p));
		}

		public static IEnumerable<KeyValuePair<string, int>> GetAllUnowned(Farmer who)
		{
			return Skills.SelectMany(
				sk => sk.Value.Professions.Where(
					p => !who.professions.Contains(p)
				).Select(
					p => new KeyValuePair<string, int>(sk.Key, p)
				)
			);
		}

		public static string GetDisplayName(string skill)
		{
			return Skills.TryGetValue(skill, out var sk) ? sk.DisplayName : skill;
		}

		public static IEnumerable<string> AllSkills()
		{
			return Skills.Keys;
		}

		public static bool HasSkill(string skill)
		{
			return Skills.ContainsKey(skill);
		}
	}
}
