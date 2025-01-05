using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Netcode;
using StardewModdingAPI;
using StardewValley;
using StardewValley.ItemTypeDefinitions;
using System.Xml.Serialization;

namespace ProfessionBooks.Framework
{
	[XmlType("Mods_ProfessionBooks_SkillBook")]
	public class Skillbook : StardewValley.Object
	{
		private const int X_OFF = 4;
		private const int Y_OFF = 1;

		private static IModHelper Helper;

		internal static void Init(IModHelper helper)
		{
			Helper = helper;
		}

		public string SkillId
		{
			get => skillId.Value;
			set => skillId.Value = value;
		}
		private NetString skillId = null!;
		private Action<SpriteBatch, Rectangle, Color, float> drawIcon;

		// use item queries to create
		internal Skillbook(string id) : base(Assets.BOOK_ID, 1)
		{
			drawIcon = SkillManager.GetIconDraw(id);
			skillId.Value = id;
		}

		public Skillbook() : base()
		{
			drawIcon = static (s, r, c, d) => { };
		}

		protected override void _PopulateContextTags(HashSet<string> tags)
		{
			base._PopulateContextTags(tags);
			tags.Add($"profession_book_{SkillId}");
		}

		private void OnSkillIdChanged(NetString field, string oldValue, string newValue)
		{
			drawIcon = SkillManager.GetIconDraw(newValue);
			displayName = loadDisplayName();
			MarkContextTagsDirty();
		}

		protected override void initNetFields()
		{
			base.initNetFields();

			skillId = new();
			skillId.fieldChangeVisibleEvent += OnSkillIdChanged;
			NetFields.AddField(skillId, nameof(skillId));
		}

		public override bool performUseAction(GameLocation location)
		{
			if (!Game1.player.canMove || isTemporarilyInvisible)
				return base.performUseAction(location);

			if (Game1.eventUp || Game1.isFestival() || Game1.fadeToBlack)
				return base.performUseAction(location);

			if (Game1.player.swimming.Value || Game1.player.bathingClothes.Value || Game1.player.onBridge.Value)
				return base.performUseAction(location);

			ReadBook(location);

			var professions = SkillManager.GetUnownedForSkill(SkillId, Game1.player).ToList();

			if (professions.Count == 0)
			{
				SkillManager.AddXpToSkill(SkillId, Game1.player, 1000);
				Game1.showGlobalMessage(Helper.Translation.Get("msg.gainedExp"));
				Game1.player.stats.Set($"ProfessionBooks_Skill_{SkillId}", 1);
			}
			else
			{
				var selected = professions[Game1.random.Next(professions.Count)];
				var name = SkillManager.GetProfessionName(selected);
				var an = Utility.AOrAn(name);

				Game1.player.professions.Add(selected);

				var fmat = string.Format(Helper.Translation.Get("msg.learnedProfession"), an, name);
				Game1.showGlobalMessage(fmat);

				if (professions.Count == 1)
					Game1.player.stats.Set($"ProfessionBooks_Skill_{SkillId}", 1);
				else
					Game1.player.stats.Set($"ProfessionBooks_Skill_{SkillId}", 0);
			}

			return true;
		}

		public override void drawInMenu(SpriteBatch spriteBatch, Vector2 location, float scaleSize, float transparency, float layerDepth, StackDrawType drawStackNumber, Color color, bool drawShadow)
		{
			AdjustMenuDrawForRecipes(ref transparency, ref scaleSize);
			if (drawShadow)
				DrawShadow(spriteBatch, location, color, layerDepth);

			ParsedItemData itemData = ItemRegistry.GetDataOrErrorItem(QualifiedItemId);
			float drawnScale = scaleSize;
			int offset = 0;
			var v = this;
			Rectangle sourceRect = itemData.GetSourceRect(offset, ParentSheetIndex);
			spriteBatch.Draw(
				itemData.GetTexture(), location + new Vector2(32f, 32f), sourceRect, color * transparency, 0f, 
				new Vector2(sourceRect.Width / 2, sourceRect.Height / 2), 4f * drawnScale, SpriteEffects.None, layerDepth
			);
			int size = (int)(40f * drawnScale);
			var center = new Vector2(32f);

			Vector2 pos = new(X_OFF * 4f - 4f, Y_OFF * 4f + 4f);
			pos = (pos - center) * drawnScale;
			pos += center;

			drawIcon(
				spriteBatch,
				new((int)location.X + (int)pos.X, (int)location.Y + (int)pos.Y, size, size),
				Color.Black * .3f * transparency,
				MathF.BitIncrement(layerDepth)
			);

			pos = new(X_OFF * 4f, Y_OFF * 4f);
			pos = (pos - center) * drawnScale;
			pos += center;

			drawIcon(
				spriteBatch,
				new((int)location.X + (int)pos.X, (int)location.Y + (int)pos.Y, size, size),
				color * transparency,
				MathF.BitIncrement(layerDepth)
			);

			DrawMenuIcons(spriteBatch, location, scaleSize, transparency, layerDepth, drawStackNumber, color);
		}

		public override void draw(SpriteBatch spriteBatch, int xNonTile, int yNonTile, float layerDepth, float alpha = 1)
		{
			base.draw(spriteBatch, xNonTile, yNonTile, layerDepth, alpha);
			drawIcon(
				spriteBatch,
				new(xNonTile + (X_OFF - 1) * 4, yNonTile + (Y_OFF + 1) * 4, 40, 40),
				Color.Black * alpha * .3f,
				layerDepth = MathF.BitIncrement(layerDepth)
			);
			drawIcon(
				spriteBatch,
				new(xNonTile + X_OFF * 4, yNonTile + Y_OFF * 4, 40, 40),
				Color.White * alpha,
				MathF.BitIncrement(layerDepth)
			);
		}

		public override void drawWhenHeld(SpriteBatch spriteBatch, Vector2 objectPosition, Farmer f)
		{
			base.drawWhenHeld(spriteBatch, objectPosition, f);
			float drawLayer = Math.Max(0f, (float)(f.StandingPixel.Y + 3) / 10000f);

			drawIcon(
				spriteBatch,
				new((int)(objectPosition.X + (X_OFF - 1) * 4f), (int)(objectPosition.Y + (Y_OFF + 1) * 4f), 40, 40),
				Color.Black * .3f,
				drawLayer = MathF.BitIncrement(drawLayer)
			);
			drawIcon(
				spriteBatch,
				new((int)(objectPosition.X + X_OFF * 4f), (int)(objectPosition.Y + Y_OFF * 4f), 40, 40),
				Color.White,
				MathF.BitIncrement(drawLayer)
			);
		}

		public override void DrawMenuIcons(SpriteBatch sb, Vector2 location, float scale_size, float transparency, float layer_depth, StackDrawType drawStackNumber, Color color)
		{
			base.DrawMenuIcons(sb, location, scale_size, transparency, layer_depth, drawStackNumber, color);

			if (drawStackNumber != 0 && Quality is 0 && Game1.stats.Get($"ProfessionBooks_Skill_{SkillId}") != 0)
			{
				sb.Draw(
					Game1.mouseCursors_1_6, location + new Vector2(12f, 44f), new Rectangle(244, 271, 9, 11), 
					color * transparency, 0f, new Vector2(4f, 4f), 3f * scale_size * 1f, SpriteEffects.None, layer_depth
				);
			}
		}

		protected override Item GetOneNew()
		{
			return new Skillbook() {
				ItemId = ItemId,
				ParentSheetIndex = ParentSheetIndex,
				Stack = 1
			};
		}

		protected override void GetOneCopyFrom(Item source)
		{
			base.GetOneCopyFrom(source);

			if (source is Skillbook book)
			{
				SkillId = book.SkillId;
			}
		}

		protected override string loadDisplayName()
		{
			return string.Format(base.loadDisplayName(), SkillManager.GetDisplayName(skillId.Value ?? ""));
		}

		public override string getDescription()
		{
			return string.Format(base.getDescription(), SkillManager.GetDisplayName(skillId.Value ?? ""));
		}

		public override bool canStackWith(ISalable other)
		{
			return other is Skillbook book && book.SkillId == SkillId && base.canStackWith(other);
		}

		private void ReadBook(GameLocation where)
		{
			Game1.player.canMove = false;
			Game1.player.freezePause = 1030;
			Game1.player.faceDirection(2);
			Game1.player.FarmerSprite.animateOnce(
			[
				new FarmerSprite.AnimationFrame(57, 1000, secondaryArm: false, flip: false, Farmer.canMoveNow, behaviorAtEndOfFrame: true)
				{
					frameEndBehavior = delegate
					{
						where.removeTemporarySpritesWithID(1987654);
						Utility.addRainbowStarExplosion(where, Game1.player.getStandingPosition() + new Vector2(-40f, -156f), 8);
					}
				}
			]);
			Game1.MusicDuckTimer = 4000f;
			Game1.playSound("book_read");
			Game1.Multiplayer.broadcastSprites(where, new TemporaryAnimatedSprite("LooseSprites\\Book_Animation", new Rectangle(0, 0, 20, 20), 10f, 45, 1, Game1.player.getStandingPosition() + new Vector2(-48f, -156f), flicker: false, flipped: false, Game1.player.getDrawLayer() + 0.001f, 0f, Color.White, 4f, 0f, 0f, 0f)
			{
				holdLastFrame = true,
				id = 1987654
			});
			Color? c = ItemContextTagManager.GetColorFromTags(this);
			if (c.HasValue)
			{
				Game1.Multiplayer.broadcastSprites(where, new TemporaryAnimatedSprite("LooseSprites\\Book_Animation", new Rectangle(0, 20, 20, 20), 10f, 45, 1, Game1.player.getStandingPosition() + new Vector2(-48f, -156f), flicker: false, flipped: false, Game1.player.getDrawLayer() + 0.0012f, 0f, c.Value, 4f, 0f, 0f, 0f)
				{
					holdLastFrame = true,
					id = 1987654
				});
			}

			if (!Game1.player.mailReceived.Contains("read_a_book"))
			{
				Game1.player.mailReceived.Add("read_a_book");
			}
		}
	}
}
