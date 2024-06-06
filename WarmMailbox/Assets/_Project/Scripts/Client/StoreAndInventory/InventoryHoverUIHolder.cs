using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Client.StoreAndInventory
{
    [Serializable]
    public class InventoryItemHoverUI
    {
        public Transform itemHoverUI;
        public TextMeshProUGUI itemNameText;
        public Image itemImage;
        public TextMeshProUGUI itemKindText;
        public TextMeshProUGUI itemDescriptionText;
        public TextMeshProUGUI itemPriceText;
        public TextMeshProUGUI additionalPointsText;
    }
    
    public class InventoryHoverUIHolder : MonoBehaviour
    {
        public InventoryItemHoverUI newInventoryItemHoverUI;
        public InventoryItemHoverUI equippedInventoryItemHoverUI;
        
        public void SetInventoryNewItemHoverUI(EmpathyMailboxItem empathyMailboxItem)
        {
            newInventoryItemHoverUI.itemNameText.text = empathyMailboxItem.itemName;
            newInventoryItemHoverUI.itemImage.sprite = empathyMailboxItem.itemSprite;
            switch (empathyMailboxItem.itemKind)
            {
                case ItemKind.LeftTop:
                    newInventoryItemHoverUI.itemKindText.text = "걸이용";
                    break;
                case ItemKind.RightTop:
                    newInventoryItemHoverUI.itemKindText.text = "액자용";
                    break;
                case ItemKind.Desk:
                    newInventoryItemHoverUI.itemKindText.text = "책상용";
                    break;
            }
            newInventoryItemHoverUI.itemDescriptionText.text = empathyMailboxItem.itemDescription;
            newInventoryItemHoverUI.itemPriceText.text = empathyMailboxItem.itemPrice.ToString();
            newInventoryItemHoverUI.additionalPointsText.text = "+" + empathyMailboxItem.bonusCurrency;
        }
        
        public void SetInventoryEquippedItemHoverUI(EmpathyMailboxItem empathyMailboxItem)
        {
            equippedInventoryItemHoverUI.itemNameText.text = empathyMailboxItem.itemName;
            equippedInventoryItemHoverUI.itemImage.sprite = empathyMailboxItem.itemSprite;
            switch (empathyMailboxItem.itemKind)
            {
                case ItemKind.LeftTop:
                    equippedInventoryItemHoverUI.itemKindText.text = "걸이용";
                    break;
                case ItemKind.RightTop:
                    equippedInventoryItemHoverUI.itemKindText.text = "액자용";
                    break;
                case ItemKind.Desk:
                    equippedInventoryItemHoverUI.itemKindText.text = "책상용";
                    break;
            }
            equippedInventoryItemHoverUI.itemKindText.text += " (장착중)";
            equippedInventoryItemHoverUI.itemDescriptionText.text = empathyMailboxItem.itemDescription;
            equippedInventoryItemHoverUI.itemPriceText.text = empathyMailboxItem.itemPrice.ToString();
            equippedInventoryItemHoverUI.additionalPointsText.text = "+" + empathyMailboxItem.bonusCurrency;
        }
        
        public void SetInventoryEquippedItemHoverUIActive(bool active)
        {
            equippedInventoryItemHoverUI.itemHoverUI.gameObject.SetActive(active);
        }
    }
}
