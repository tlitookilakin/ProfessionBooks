using ProfessionBooks.API;

namespace ProfessionBooks.Plugins
{
	public class VanillaPlugin : IPlugin
	{
		private static readonly string[] Skills = ["farming", "fishing", "foraging", "mining", "combat"];

		public void AddSkills(IDictionary<string, ISkill> skills)
		{
			for (int i = 0; i < Skills.Length; i++)
				skills[Skills[i]] = new VanillaSkill(i);
		}
	}
}
