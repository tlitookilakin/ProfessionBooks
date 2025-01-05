using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ProfessionBooks.API;
using SpaceCore;
using StardewValley;
using static SpaceCore.Skills;

namespace ProfessionBooks.Plugins
{
    public class SpaceSkill : ISkill
    {

        public string DisplayName => source.GetName();

		public IList<KeyValuePair<int, Func<string>>> Professions { get; init; }

		private Skill source;

        public SpaceSkill(Skill source)
        {
            this.source = source;
            Professions = source.Professions.Select(p => new KeyValuePair<int, Func<string>>(p.GetVanillaId(), p.GetName)).ToList();
        }

        public void DrawIcon(SpriteBatch batch, Rectangle dest, Color c, float depth)
        {
            batch.Draw(source.SkillsPageIcon, dest, null, c, 0f, default, SpriteEffects.None, depth);
		}

		public void AddXp(int amount, Farmer who)
		{
			Skills.AddExperience(who, source.Id, amount);
		}
	}
}
