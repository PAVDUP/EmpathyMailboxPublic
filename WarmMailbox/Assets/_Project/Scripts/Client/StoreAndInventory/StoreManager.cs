using LLMClients;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Client.StoreAndInventory
{
    public class StoreManager : MonoBehaviour
    {
        [Header("Store")] 
        public Transform storeWindow;
        public ItemManager itemManager;
        public Transform storeItemParent;
        public Transform storeItemPrefab;
        
        [Header("Purchase Check Window")]
        public CheckWindowHolder purchaseCheckWindow;
        
        [Header("Purchase")]
        public CurrencyClient currencyClient;
        private StoreItemInfoHolder _currentPurchaseItem;
        public UnityEvent onPurchaseSuccess;
        public UnityEvent onPurchaseFail;
        
        public void VisualizeStoreItems()
        {
            foreach (Transform child in storeItemParent)
            {
                Destroy(child.gameObject);
            }
            
            var storeItems = itemManager.GetItems();
            var purchasedItems = itemManager.GetPurchasedItems();
            
            foreach (var storeItem in storeItems)
            {
                var storeItemInstance = Instantiate(storeItemPrefab, storeItemParent);
                var storeItemInfoHolder = storeItemInstance.GetComponent<StoreItemInfoHolder>();
                storeItemInfoHolder.empathyMailboxItem = storeItem;
                storeItemInfoHolder.itemImage.sprite = storeItem.itemSprite;
                storeItemInfoHolder.itemPriceText.text = storeItem.itemPrice.ToString();
                
                Debug.Log("[StoreManager] VisualizeStoreItems: storeItem.itemUniqueId => " + storeItem.itemUniqueId);
                
                bool purchased = false;

                foreach (var purchasedItem in purchasedItems)
                {
                    if (storeItem.itemUniqueId == purchasedItem.itemUniqueId)
                    {
                        Debug.Log("[StoreManager] VisualizeStoreItems: Purchased item found => " + purchasedItem.itemUniqueId);
                        purchased = true;
                        break;
                    }
                }
                
                Debug.Log("[StoreManager] VisualizeStoreItems: purchased Indicators => " + purchased);
                foreach (var purchasedIndicator in storeItemInfoHolder.purchasedIndicators)
                {
                    purchasedIndicator.SetActive(purchased);
                }
                
                
                // Event listener for purchase
                Button storeItemButton = storeItemInstance.GetComponent<Button>();
                
                if (purchased)
                {
                    storeItemButton.interactable = false;
                }
                else
                {
                    storeItemButton.onClick.AddListener(() => OpenPurchaseCheckWindow(storeItemInfoHolder));
                }
            }
            
            storeWindow.gameObject.SetActive(true);
        }

        private void OpenPurchaseCheckWindow(StoreItemInfoHolder empathyMailboxItemHolder)
        {
            _currentPurchaseItem = empathyMailboxItemHolder;
            
            var empathyMailboxItem = empathyMailboxItemHolder.empathyMailboxItem;
            purchaseCheckWindow.itemNameText.text = empathyMailboxItem.itemName;
            purchaseCheckWindow.itemImage.sprite = empathyMailboxItem.itemSprite;

            switch (empathyMailboxItem.itemKind)
            {
                case ItemKind.LeftTop:
                    purchaseCheckWindow.itemKindText.text = "걸이용";
                    break;
                case ItemKind.RightTop:
                    purchaseCheckWindow.itemKindText.text = "액자용";
                    break;
                case ItemKind.Desk:
                    purchaseCheckWindow.itemKindText.text = "책상용";
                    break;
            }
            
            purchaseCheckWindow.itemDescriptionText.text = empathyMailboxItem.itemDescription;
            purchaseCheckWindow.itemPriceText.text = empathyMailboxItem.itemPrice.ToString();
            purchaseCheckWindow.additionalPointsText.text = "+" + empathyMailboxItem.bonusCurrency;
            
            purchaseCheckWindow.gameObject.SetActive(true);
        }
        
        public void PurchaseItem()
        {
            purchaseCheckWindow.gameObject.SetActive(false);
            PurchaseItemInternal(_currentPurchaseItem);
        }
        
        private async void PurchaseItemInternal(StoreItemInfoHolder empathyMailboxItemHolder)
        {
            var empathyMailboxItem = empathyMailboxItemHolder.empathyMailboxItem;
            if (EmpathyMailboxDatabaseManager.Instance.GetCurrency() >= empathyMailboxItem.itemPrice)
            {
                // Update UI First
                empathyMailboxItemHolder.GetComponent<Button>().interactable = false;
                foreach (var purchasedIndicator in empathyMailboxItemHolder.purchasedIndicators)
                {
                    purchasedIndicator.SetActive(true);
                }
                
                await EmpathyMailboxDatabaseManager.Instance.ChangeCurrency(-empathyMailboxItem.itemPrice);
                await EmpathyMailboxDatabaseManager.Instance.PurchaseItem(empathyMailboxItem.itemUniqueId);
                
                currencyClient.UpdateCurrency();
                onPurchaseSuccess.Invoke();
            }
            else
            {
                onPurchaseFail.Invoke();
            }
        }
        
        #region Test

        private async void Awake()
        {
            await EmpathyMailboxDatabaseManager.Instance.ChangeCurrency(50000);
        }

        #endregion
    }
}
