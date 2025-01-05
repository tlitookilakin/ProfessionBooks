using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ProfessionBooks.API;
using ProfessionBooks.Framework;
using StardewValley;

namespace ProfessionBooks.Plugins
{
    public class VanillaSkill : ISkill
    {

        private readonly int Which;
        private Rectangle icon_src;

        private readonly IList<KeyValuePair<int, Func<string>>> professions;
        private readonly int[] iconX = [10, 20, 60, 30, 120, 50];

        public string DisplayName
            => Farmer.getSkillDisplayNameFromIndex(Which);

        public IList<KeyValuePair<int, Func<string>>> Professions => professions;

		public void DrawIcon(SpriteBatch batch, Rectangle dest, Color c, float depth)
        {
            batch.Draw(Game1.mouseCursors, dest, icon_src, c, 0f, default, SpriteEffects.None, depth);
        }

		public void AddXp(int amount, Farmer who)
		{
			who.gainExperience(Which, amount);
		}

		public VanillaSkill(int which)
        {
            Which = which;
            icon_src = new(iconX[which], 428, 10, 10);

            // vanilla professions
            int basep = which * 6;

            professions = [];
            for (int i = 0; i < 6; i++)
                professions.Add(GetPair(basep + i));
        }

        private static KeyValuePair<int, Func<string>> GetPair(int profession)
        {
            var name = ModUtilities.GetProfessionName(profession);

            return new(profession, () => Game1.content.LoadString("Strings\\UI:LevelUp_ProfessionName_" + name));
        }
    }
}
