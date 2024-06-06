using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Client.StoreAndInventory
{
    public class StoreItemInfoHolder : MonoBehaviour
    {
        public EmpathyMailboxItem empathyMailboxItem;
        public Image itemImage;
        public TextMeshProUGUI itemPriceText;
        public GameObject[] purchasedIndicators;
    }
}
