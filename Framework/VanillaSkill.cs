using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ProfessionBooks.Integration;
using StardewValley;

namespace ProfessionBooks.Framework
{
	public class VanillaSkill : ISkill
	{
		private readonly int Which;
		private Rectangle icon_src;

		public IReadOnlyList<int> Professions => professions;
		private readonly IReadOnlyList<int> professions;
		private readonly int[] iconX = [10, 20, 60, 30, 120, 50];

		public string DisplayName
			=> Farmer.getSkillDisplayNameFromIndex(Which);

		public void DrawIcon(SpriteBatch batch, Rectangle dest, Color c, float depth)
		{
			batch.Draw(Game1.mouseCursors, dest, icon_src, c, 0f, default, SpriteEffects.None, depth);
		}

		public VanillaSkill(int which)
		{
			Which = which;
			icon_src = new(iconX[which], 428, 10, 10);

			// vanilla professions
			int basep = which * 6;
			professions = [basep, basep + 1, basep + 2, basep + 3, basep + 4, basep + 5];

			// integrations
			VPP.AddProfessionsForSkill(ref professions, which);
		}
	}
}
