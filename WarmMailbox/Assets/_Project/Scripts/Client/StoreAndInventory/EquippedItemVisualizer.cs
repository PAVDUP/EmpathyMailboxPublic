using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Client.StoreAndInventory
{
    [Serializable]
    public class EquippedItemHolder
    {
        public ItemKind itemKind;
        public Image equippedIndicator;
    }

    public class EquippedItemVisualizer : MonoBehaviour
    {
        public ItemManager itemManager;
        public List<EquippedItemHolder> equippedItemHolders;
    
        public void VisualizeEquippedItems()
        {
            var equippedItems = itemManager.GetEquippedItems();
            
            foreach (var equippedItemHolder in equippedItemHolders)
            {
                equippedItemHolder.equippedIndicator.color = Color.clear;
                
                foreach (var equippedItem in equippedItems)
                {
                    if (equippedItem.itemKind == equippedItemHolder.itemKind)
                    {
                        equippedItemHolder.equippedIndicator.sprite = equippedItem.itemSprite;
                        equippedItemHolder.equippedIndicator.color = Color.white;
                    }
                }
            }
        
        }
    }
}