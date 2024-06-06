using System;
using UnityEngine;

namespace Client.StoreAndInventory
{
    [Serializable]
    public enum ItemKind
    {
        LeftTop,
        RightTop,
        Desk
    }
    
    [CreateAssetMenu(fileName = "DefaultItem", menuName = "WarmMailbox/Item", order = 1)]
    public class EmpathyMailboxItem : ScriptableObject
    {
        public string itemName;
        public ItemKind itemKind;
        public string itemUniqueId;
        public Sprite itemSprite;
        public int itemPrice;
        public string additionalWorryPrompt;
        public int bonusCurrency;
        public string itemDescription;
    }
}
