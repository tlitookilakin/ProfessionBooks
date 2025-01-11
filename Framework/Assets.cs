using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley.GameData.Objects;
using StardewValley.GameData.Shops;

namespace ProfessionBooks.Framework
{
	public static class Assets
	{
		public const string MOD_ID = "tlitookilakin.ProfessionBooks";
		public const string BOOK_ID = MOD_ID + "_ProfessionBook";
		public const string ITEM_SHEET = "Mods/" + MOD_ID + "/Items";

		private static IModHelper Helper;

		internal static void Init(IModHelper helper)
		{
			Helper = helper;

			helper.Events.Content.AssetRequested += Requested;
		}

		private static void Requested(object? sender, AssetRequestedEventArgs e)
		{
			if (e.NameWithoutLocale.IsEquivalentTo("Data/Objects"))
				e.Edit(AddItem);
			else if (e.NameWithoutLocale.IsEquivalentTo(ITEM_SHEET))
				e.LoadFromModFile<Texture2D>("Assets/items.png", AssetLoadPriority.Low);
			else if (e.NameWithoutLocale.IsEquivalentTo("Data/Shops"))
				e.Edit(EditShops);
		}

		private static void AddItem(IAssetData asset)
		{
			if (asset.Data is not Dictionary<string, ObjectData> data)
				return;

			data[BOOK_ID] = new()
			{
				Name = "Profession Book",
				DisplayName = Helper.Translation.Get("item.book.name"),
				Description = Helper.Translation.Get("item.book.desc"),
				SpriteIndex = 0,
				Texture = ITEM_SHEET,
				Category = StardewValley.Object.skillBooksCategory,
				Type = "Basic",
				Price = 250_000,
				ExcludeFromShippingCollection = true,
				ExcludeFromRandomSale = true,
				ContextTags = ["color_sand"]
			};
		}

		private static void EditShops(IAssetData asset)
		{
			if (asset.Data is not Dictionary<string, ShopData> shops)
				return;

			if (shops.TryGetValue("Bookseller", out var bookseller))
			{
				bookseller.Items.Add(new()
				{
					Id = "ProfessionBook",
					ItemId = "ProfessionBooks_Book",
					MaxItems = 3,
					AvailableStockLimit = LimitedStockMode.Player,
					AvailableStock = 1,
					PerItemCondition = "ProfessionBooks_Available this",
					UseObjectDataPrice = true
				});
			}

			if (shops.TryGetValue("Traveler", out var traveler))
			{
				traveler.Items.Add(new()
				{
					Id = "ProfessionBook",
					ItemId = "ProfessionBooks_Book",
					MaxItems = 1,
					AvailableStockLimit = LimitedStockMode.Player,
					AvailableStock = 1,
					PerItemCondition = "ProfessionBooks_Available this",
					Condition = "SYNCED_RANDOM day cart_profession_book .1",
					UseObjectDataPrice = true
				});
			}
		}
	}
}
