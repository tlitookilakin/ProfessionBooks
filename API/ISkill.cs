using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewValley;

namespace ProfessionBooks.API
{
    public interface ISkill
    {
        public void DrawIcon(SpriteBatch batch, Rectangle dest, Color c, float depth);
        public IList<KeyValuePair<int, Func<string>>> Professions { get; }
        public string DisplayName { get; }
        public void AddXp(int amount, Farmer who);
    }
}
