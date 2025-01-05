using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ProfessionBooks.API;
using StardewValley;

namespace ProfessionBooks.Framework
{
    public static class SkillManager
	{
		private static readonly Dictionary<string, ISkill> Skills = new(StringComparer.OrdinalIgnoreCase);
		private static readonly Dictionary<int, Func<string>> ProfessionNames = [];

		internal static void Init()
		{
			API.API.AddSkills(Skills);

			foreach ((var k, var v) in Skills)
				API.API.AddProfessions(k, v.Professions);

			var allProfessions = Skills.SelectMany(sk => sk.Value.Professions);
			foreach (var pair in allProfessions)
				ProfessionNames[pair.Key] = pair.Value;
		}

		public static Action<SpriteBatch, Rectangle, Color, float> GetIconDraw(string skill)
		{
			if (!Skills.TryGetValue(skill, out var sk))
				return static (s, r, c, d) => { };

			return sk.DrawIcon;
		}

		public static void AddXpToSkill(string which, Farmer who, int amount)
		{
			if (Skills.TryGetValue(which, out var sk))
				sk.AddXp(amount, who);
		}

		public static IEnumerable<int> GetUnownedForSkill(string skill, Farmer who)
		{
			if (!Skills.TryGetValue(skill, out var sk))
				return [];

			return sk.Professions.Where(p => !who.professions.Contains(p.Key)).Select(p => p.Key);
		}

		public static IEnumerable<KeyValuePair<string, int>> GetAllUnowned(Farmer who)
		{
			return Skills.SelectMany(
				sk => sk.Value.Professions.Where(
					p => !who.professions.Contains(p.Key)
				).Select(
					p => new KeyValuePair<string, int>(sk.Key, p.Key)
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

		public static string GetProfessionName(int which)
		{
			if (!ProfessionNames.TryGetValue(which, out var GetName))
				return "???";

			return GetName();
		}
	}
}
