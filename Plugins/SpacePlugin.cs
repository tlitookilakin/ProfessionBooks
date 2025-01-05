using ProfessionBooks.API;
using ProfessionBooks.Framework;
using SpaceCore;
using StardewModdingAPI;

namespace ProfessionBooks.Plugins
{
    internal class SpacePlugin : IPlugin
    {
        private static IApi api;

        internal static void Init(IModHelper helper)
        {
            api = (IApi)helper.ModRegistry.GetApi("spacechase0.SpaceCore")!;

            api.RegisterSerializerType(typeof(Skillbook));
        }

        public static IEnumerable<KeyValuePair<string, ISkill>> GetSkills()
        {
            return Skills.GetSkillList().Select(
                name => new KeyValuePair<string, ISkill>(name, new SpaceSkill(Skills.GetSkill(name))
            ));
		}

		public void AddSkills(IDictionary<string, ISkill> Skills)
        {
            foreach (var pair in GetSkills())
                Skills.Add(pair.Key, pair.Value);
        }
    }
}
