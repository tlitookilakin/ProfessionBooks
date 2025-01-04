using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ProfessionBooks.Framework;
using SpaceCore;

namespace ProfessionBooks.Integration
{
	public class SpaceSkill : ISkill
	{
		public IReadOnlyList<int> Professions { get; init; }

		public string DisplayName => source.GetName();

		private Skills.Skill source;

		public SpaceSkill(Skills.Skill source)
		{
			this.source = source;
			Professions = source.Professions.Select(p => p.GetVanillaId()).ToList();
		}

		public void DrawIcon(SpriteBatch batch, Rectangle dest, Color c, float depth)
		{
			batch.Draw(source.SkillsPageIcon, dest, null, c, 0f, default, SpriteEffects.None, depth);
		}
	}
}
