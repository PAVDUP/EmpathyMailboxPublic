using System.Collections.Generic;
using System.Threading.Tasks;
using LLMClients;
using UnityEngine;

namespace Client.StoreAndInventory
{
    public class ItemManager : MonoBehaviour
    {
        public List<EmpathyMailboxItem> empathyMailboxItems;
        private Dictionary<string, EmpathyMailboxItem> _empathyMailboxItemDictionary = new();

        private void Awake()
        {
            Initialize();
        }

        private void Initialize()
        {
            foreach (var empathyMailboxItem in empathyMailboxItems)
            {
                if (!_empathyMailboxItemDictionary.TryAdd(empathyMailboxItem.itemUniqueId, empathyMailboxItem))
                {
                    Debug.LogError("[ItemManager] Initialize: Failed to add item to dictionary => There is repeated itemUniqueId.");
                }
            }
        }

        #region Fetch
        
        public List<EmpathyMailboxItem> GetItems()
        {
            return empathyMailboxItems;
        }
        
        public EmpathyMailboxItem GetItem(string itemUniqueId)
        {
            return _empathyMailboxItemDictionary.GetValueOrDefault(itemUniqueId);
        }

        public List<EmpathyMailboxItem> GetPurchasedItems()
        {
            List<string> purchasedItemIds = EmpathyMailboxDatabaseManager.Instance.GetPurchasedItemsIds();
            List<EmpathyMailboxItem> purchasedItems = new List<EmpathyMailboxItem>();

            foreach (var purchasedItemId in purchasedItemIds)
            {
                if (_empathyMailboxItemDictionary.TryGetValue(purchasedItemId, out var value))
                {
                    purchasedItems.Add(value);
                }
            }

            return purchasedItems;
        }

        public List<EmpathyMailboxItem> GetEquippedItems()
        {
            List<string> equippedItemIds = EmpathyMailboxDatabaseManager.Instance.GetEquippedItems();
            List<EmpathyMailboxItem> equippedItems = new List<EmpathyMailboxItem>();

            foreach (var equippedItemId in equippedItemIds)
            {
                if (_empathyMailboxItemDictionary.TryGetValue(equippedItemId, out var value))
                {
                    equippedItems.Add(value);
                }
            }

            return equippedItems;
        }
        
        public List<EmpathyMailboxItem> GetItemsWithKind (ItemKind kind)
        {
            List<EmpathyMailboxItem> itemsWithKind = new List<EmpathyMailboxItem>();

            foreach (var empathyMailboxItem in empathyMailboxItems)
            {
                if (empathyMailboxItem.itemKind == kind)
                {
                    itemsWithKind.Add(empathyMailboxItem);
                }
            }

            return itemsWithKind;
        }
        
        public List<EmpathyMailboxItem> GetPurchasedItemsWithKind (ItemKind kind)
        {
            // ItemIds 가져 와서, 순회 돌면서, kind에 맞는 아이템들만 뽑아서 리턴하는 방식. 이게 훨씬 깔끔함.
            List<string> purchasedItemIds = EmpathyMailboxDatabaseManager.Instance.GetPurchasedItemsIds();
            List<EmpathyMailboxItem> purchasedItemsWithKind = new List<EmpathyMailboxItem>();
            
            foreach (var purchasedItemId in purchasedItemIds)
            {
                if (_empathyMailboxItemDictionary.TryGetValue(purchasedItemId, out var value))
                {
                    if (value.itemKind == kind)
                    {
                        purchasedItemsWithKind.Add(value);
                    }
                }
            }

            return purchasedItemsWithKind;
        }
        
        public EmpathyMailboxItem GetEquippedItemWithKind (ItemKind kind)
        {
            List<string> equippedItemIds = EmpathyMailboxDatabaseManager.Instance.GetEquippedItems();
            
            foreach (var equippedItemId in equippedItemIds)
            {
                if (_empathyMailboxItemDictionary.TryGetValue(equippedItemId, out var value))
                {
                    if (value.itemKind == kind)
                    {
                        return value;
                    }
                }
            }
            
            return null;
        }
        
        #endregion

        #region Update

        public async Task EquipItem(EmpathyMailboxItem newEquipItem)
        {
            await EmpathyMailboxDatabaseManager.Instance.EquipItem(newEquipItem.itemUniqueId);
        }
        
        public async Task EquipItem(EmpathyMailboxItem oldEquippedItem, EmpathyMailboxItem newEquippedItem)
        {
            await EmpathyMailboxDatabaseManager.Instance.EquipItem(oldEquippedItem.itemUniqueId, newEquippedItem.itemUniqueId);
        }

        #endregion
    }
}
