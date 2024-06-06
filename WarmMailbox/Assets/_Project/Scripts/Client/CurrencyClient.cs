using System.Collections.Generic;
using LLMClients;
using TMPro;
using UnityEngine;

namespace Client
{
    public class CurrencyClient : MonoBehaviour
    {
        public List<TextMeshProUGUI> currencyTexts;
        
        public int earnings;
        public List<TextMeshProUGUI> earningsTexts;
        
        public void UpdateCurrency()
        {
            foreach (var currencyText in currencyTexts)
            {
                currencyText.text = EmpathyMailboxDatabaseManager.Instance.GetCurrency().ToString();
            }
        }
        
        public void UpdateCurrency(int inputCurrency)
        {
            foreach (var currencyText in currencyTexts)
            {
                currencyText.text = inputCurrency.ToString();
            }
        }

        public void UpdateEarnings()
        {
            foreach (var earningsText in earningsTexts)
            {
                earningsText.text = "+ " + earnings;
            }
        }
        
        public async void AddEarnings()
        {
            UpdateCurrency(EmpathyMailboxDatabaseManager.Instance.GetCurrency() + earnings);
            await EmpathyMailboxDatabaseManager.Instance.ChangeCurrency(earnings);
        }
    }
}
