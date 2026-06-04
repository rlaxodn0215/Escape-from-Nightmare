using UnityEngine;

namespace EscapeFromNightmare
{
	[CreateAssetMenu(fileName = "ItemDefinition", menuName = "Escape From Nightmare/Item Definition")]
	public class ItemDefinition : ScriptableObject
	{
		[SerializeField] private string itemId;
		[SerializeField] private string displayName;
		[SerializeField] private Sprite icon;

		public string ItemId => itemId;
		public string DisplayName => displayName;
		public Sprite Icon => icon;

		private void OnValidate()
		{
			if (string.IsNullOrWhiteSpace(itemId))
			{
				itemId = name;
			}

			if (string.IsNullOrWhiteSpace(displayName))
			{
				displayName = itemId;
			}
		}
	}
}
