using System;
using System.Collections.Generic;
using UnityEngine;

namespace LLMClients
{
    [Serializable]
    public class FairyCollection
    {
        public LLMType llmType;
        public List<FairyType> fairyTypes;
    }
    
    public class FairyCollectionManager : MonoBehaviour
    {
        public LLMType currentLLMType;
        public List<FairyCollection> fairyCollections;

        public FairyType ReturnFairy(string fairyName)
        {
            foreach (var fairyCollection in fairyCollections)
            {
                if (fairyCollection.llmType == currentLLMType)
                {
                    foreach (var fairyType in fairyCollection.fairyTypes)
                    {
                        if (fairyType.fairyTypeName == fairyName)
                        {
                            return fairyType;
                        }
                    }
                }
            }

            Debug.LogError("[FairyCollectionManager] There is no fairy with the name " + fairyName + " in the current LLMType.");
            return null;
        }
    }
}
