using UnityEngine;

namespace LLMClients
{
    public class PosyLikabilityCheckAndActivator : MonoBehaviour
    {
        public string posyName;
        
        public void CheckAndActivatePosy()
        {
            if (EmpathyMailboxDatabaseManager.Instance.GetPosyLikeAbility(posyName) > 40)
            {
                gameObject.SetActive(true);
            }
        }
    }
}
