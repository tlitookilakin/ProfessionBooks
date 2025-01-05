namespace ProfessionBooks.API
{
	public interface IPlugin
	{
		public void AddProfessions(string skill, IList<KeyValuePair<int, Func<string>>> Professions) { }
		public void AddSkills(IDictionary<string, ISkill> Skills) { }
	}
}
