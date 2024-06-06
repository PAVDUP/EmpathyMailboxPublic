using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace Client.StoreAndInventory
{
    public class InventoryManager : MonoBehaviour
    {
        [Header("Inventory")] 
        public Transform inventoryWindow;
        public ItemManager itemManager;
        public Transform inventoryItemParent;
        public Transform inventoryItemPrefab;
        private InventoryItemInfoHolder _currentSelectedInventoryItem;
        
        private bool _isEquipping = false;

        [Header("Hover")] 
        public InventoryHoverUIHolder hoverPanel;
        private Coroutine _hoverPanelCoroutine;
        private bool _isHovering;
        
        [FormerlySerializedAs("onEquipItemOccur")] [Header("Events")]
        public UnityEvent onEquipItemDone;

        private void Start()
        {
            if (itemManager == null)
            {
                Debug.LogError("[InventoryVisualizer] Start: ItemManager is not set.");
            }
            
            if (hoverPanel == null)
            {
                Debug.LogError("[InventoryVisualizer] Start: HoverPanel is not set.");
            }
        }

        [VisibleEnum(typeof(ItemKind))]
        public void VisualizeInventory(int inputKind)
        {
            ItemKind kind = (ItemKind) inputKind;
            
            // Clear all children
            foreach (Transform child in inventoryItemParent)
            {
                Destroy(child.gameObject);
            }
            
            // Get purchased items
            var purchasedItems = itemManager.GetPurchasedItemsWithKind(kind);
            var equippedItem = itemManager.GetEquippedItemWithKind(kind);
            
            // Instantiate and set items
            foreach (var purchasedItem in purchasedItems)
            {
                var item = Instantiate(inventoryItemPrefab, inventoryItemParent);
                
                InventoryItemInfoHolder tempInventoryItemInfoHolder = item.GetComponent<InventoryItemInfoHolder>();
                tempInventoryItemInfoHolder.empathyMailboxItem = purchasedItem;
                tempInventoryItemInfoHolder.itemImage.sprite = purchasedItem.itemSprite;

                if (equippedItem != null) {
                    // Set equipped indicator
                    if (purchasedItem.itemUniqueId == equippedItem.itemUniqueId)
                    {
                        _isEquipping = true;
                        _currentSelectedInventoryItem = tempInventoryItemInfoHolder;

                        foreach (var equippedIndicator in tempInventoryItemInfoHolder.equippedIndicators)
                        {
                            equippedIndicator.SetActive(true);
                        }
                    }
                }
                
                // Set events
                item.GetComponent<Button>().onClick.AddListener( () =>
                {
                     EquipItem(tempInventoryItemInfoHolder);
                });
                
                // Hover Enter
                item.GetComponent<EventTrigger>().triggers[0].callback.AddListener((_) =>
                {
                    _isHovering = true;
                    if (_hoverPanelCoroutine != null)
                    {
                        StopCoroutine(_hoverPanelCoroutine);
                    }
                    _hoverPanelCoroutine = StartCoroutine(OnMouseHover(tempInventoryItemInfoHolder));
                });
                // Hover Exit
                item.GetComponent<EventTrigger>().triggers[1].callback.AddListener((_) =>
                {
                    _isHovering = false;
                    if (_hoverPanelCoroutine != null)
                        StopCoroutine(_hoverPanelCoroutine);
                    hoverPanel.gameObject.SetActive(false);
                });
            }
            
            if (_currentSelectedInventoryItem == null)
            {
                _isEquipping = false;
            }
            
            inventoryWindow.gameObject.SetActive(true);
        }

        private async void EquipItem(InventoryItemInfoHolder inventoryItemInfoHolder)
        {
            if (_currentSelectedInventoryItem != null)
            {
                if (_currentSelectedInventoryItem.empathyMailboxItem.itemUniqueId == inventoryItemInfoHolder.empathyMailboxItem.itemUniqueId)
                {
                    return;
                }
                
                foreach (var equippedIndicator in _currentSelectedInventoryItem.equippedIndicators)
                {
                    equippedIndicator.SetActive(false);
                }
                
                await itemManager.EquipItem(_currentSelectedInventoryItem.empathyMailboxItem, inventoryItemInfoHolder.empathyMailboxItem);
            }
            else
            {
                await itemManager.EquipItem(inventoryItemInfoHolder.empathyMailboxItem);
            }

            foreach (var equippedIndicator in inventoryItemInfoHolder.equippedIndicators)
            {
                equippedIndicator.SetActive(true);
            }
            
            _currentSelectedInventoryItem = inventoryItemInfoHolder;
            _isEquipping = true;
            
            onEquipItemDone.Invoke();
        }
        
        private IEnumerator OnMouseHover(InventoryItemInfoHolder inventoryItemInfoHolder)
        {
            hoverPanel.SetInventoryEquippedItemHoverUIActive(_isEquipping);
            if (_isEquipping)
            {
                hoverPanel.SetInventoryEquippedItemHoverUI(_currentSelectedInventoryItem.empathyMailboxItem);
            }
            
            hoverPanel.SetInventoryNewItemHoverUI(inventoryItemInfoHolder.empathyMailboxItem);
            hoverPanel.gameObject.SetActive(true);

            while (true)
            {
                // 마우스의 위치가 중앙보다 오른쪽일 경우, 패널을 왼쪽에, 아니면 오른쪽에 띄우기 => 이거 안 될 가능성이 높은 게, Overlay Canvas 를 기준으로 동작해야 되기 때문에.
                // => 오히려 Overlay 라서 될 가능성이 높을 수도 있을 듯? 
                // ReSharper disable once PossibleLossOfFraction
                if (Input.mousePosition.x > Screen.width / 2)
                {
                    hoverPanel.transform.position = new Vector3(Input.mousePosition.x - hoverPanel.GetComponent<RectTransform>().sizeDelta.x, Input.mousePosition.y, 0);
                }
                else
                {
                    hoverPanel.transform.position = new Vector3(Input.mousePosition.x, Input.mousePosition.y, 0);
                }
                
                if (!_isHovering)
                {
                    hoverPanel.gameObject.SetActive(false);
                    yield break;
                }
                
                yield return null;
            }
        }
    }
}
