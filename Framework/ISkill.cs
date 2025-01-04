using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace ProfessionBooks.Framework
{
	public interface ISkill
	{
		public void DrawIcon(SpriteBatch batch, Rectangle dest, Color c, float depth);
		public IReadOnlyList<int> Professions { get; }
		public string DisplayName { get; }
	}
}
