using ProfessionBooks.Framework;
using StardewModdingAPI;
using StardewValley;

namespace ProfessionBooks.Integration
{
	public interface IItemSpawnerAPI
	{
		internal static IItemSpawnerAPI? api;

		private const string ID = "CJBok.ItemSpawner";

		internal static void Init(IModHelper helper)
		{
			if (helper.ModRegistry.IsLoaded(ID))
			{
				api = helper.ModRegistry.GetApi<IItemSpawnerAPI>(ID);
				if (api is null)
					return;

				api.BlacklistItem("(O)" + Assets.BOOK_ID);
				api.VariantsRequested += Requested;
			}
		}

		static void Requested(object? sender, IVariantsRequestedEventArgs e)
		{
			if (e.BaseId is not Assets.BOOK_ID)
				return;

			foreach (var skill in SkillManager.AllSkills())
				e.TryAddVariant(Assets.BOOK_ID + '/' + skill, _ => new Skillbook(skill));
		}

		public interface IVariantsRequestedEventArgs
		{
			/// <summary>The item to provide variants for</summary>
			public string BaseId { get; }

			/// <summary>Add an item variant if valid</summary>
			/// <param name="variantId">The variant identifier- for example, the item id of a flavor item.</param>
			/// <param name="createItem">Creates an instance of the item</param>
			public void TryAddVariant(string variantId, Func<object, Item> createItem);
		}

		/// <summary>
		/// Prevent an item from being displayed in the item spawner.
		/// Should only be used for placeholder items. <br/>
		/// Does not disable variants for this item.
		/// </summary>
		/// <param name="qualifiedId">The qualified item id</param>
		public void BlacklistItem(string qualifiedId);

		/// <summary>
		/// Can be used to add custom variants to an existing item.
		/// </summary>
		public event EventHandler<IVariantsRequestedEventArgs>? VariantsRequested;
	}
}
