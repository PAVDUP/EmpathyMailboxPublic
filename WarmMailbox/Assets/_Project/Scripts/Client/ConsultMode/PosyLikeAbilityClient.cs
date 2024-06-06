using System;
using System.Collections.Generic;
using LLMClients;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Client
{
    public class PosyLikeAbilityClient : MonoBehaviour
    {
        public PosyLikeAbilityScale posyLikeAbilityScale;
        public List<PosyLikeAbilityUICollection> posyLikeAbilityUICollections;
        public PosyLikeAbilityUICollection currentPosyLikeAbilityUICollection;
        public int earningsLikeAbilityExp = 1;
        
        private string _currentFairyName;
    
        public void SetAllPosyLikeAbilityLevelImages()
        {
            foreach (var posyLikeAbilityUICollection in posyLikeAbilityUICollections)
            {
                int exp = EmpathyMailboxDatabaseManager.Instance.GetPosyLikeAbility(posyLikeAbilityUICollection.fairyName);

                int i = 0;
                foreach (var posyLikeAbilityLevel in posyLikeAbilityScale.posyLikeAbilityLevels)
                {
                    if (exp < posyLikeAbilityLevel.maxExp || exp > 1000)
                    {
                        foreach (var vLevelText in posyLikeAbilityUICollection.posyLikeAbilityLevelTexts)
                        {
                            vLevelText.text = (i + 1).ToString();
                        }
                        
                        Sprite tempSprite = posyLikeAbilityScale.posyLikeAbilityLevels[i].levelSprite;
                    
                        foreach (var vLevelImage in posyLikeAbilityUICollection.posyLikeAbilityLevelImages)
                        {
                            vLevelImage.sprite = tempSprite;
                        }
                        
                        float tempFillAmount;
                        if (i > 0)
                        { 
                            tempFillAmount = 0.2f +
                                             ((float)(exp - posyLikeAbilityScale.posyLikeAbilityLevels[i - 1].maxExp)
                                              / (posyLikeAbilityLevel.maxExp - posyLikeAbilityScale
                                                  .posyLikeAbilityLevels[i - 1].maxExp) * 0.6f) ;
                        }
                        else
                        {
                            tempFillAmount = 0.2f + ((float)(exp)
                                              / (posyLikeAbilityLevel.maxExp) * 0.6f);
                        }
                        
                    
                        foreach (var vPercentImage in posyLikeAbilityUICollection.posyLikeAbilityPercentImages)
                        {
                            if (i <= posyLikeAbilityScale.posyLikeAbilityLevels.Count - 2)
                            {
                                vPercentImage.sprite = posyLikeAbilityScale.posyLikeAbilityLevels[i+1].levelSprite;
                            }
                            else
                            {
                                vPercentImage.sprite = posyLikeAbilityScale.posyLikeAbilityLevels[^1].levelSprite;
                                vPercentImage.fillAmount = 1;
                                continue;
                            }
                            
                            vPercentImage.fillAmount = tempFillAmount;
                        }
                        
                        break;
                    }

                    i++;
                }
            }
        }
    
        public void SetCurrentPosyLikeAbilityUICollection(string fairyName)
        {
            _currentFairyName = fairyName;
            int exp = EmpathyMailboxDatabaseManager.Instance.GetPosyLikeAbility(fairyName);
            
            int i = 0;
            foreach (var posyLikeAbilityLevel in posyLikeAbilityScale.posyLikeAbilityLevels)
            {
                if (exp < posyLikeAbilityLevel.maxExp)
                {
                    foreach (var vLevelText in currentPosyLikeAbilityUICollection.posyLikeAbilityLevelTexts)
                    {
                        vLevelText.text = (i + 1).ToString();
                    }
                    
                    Sprite tempSprite = posyLikeAbilityScale.posyLikeAbilityLevels[i].levelSprite;
                    
                    foreach (var vLevelImage in currentPosyLikeAbilityUICollection.posyLikeAbilityLevelImages)
                    {
                        vLevelImage.sprite = tempSprite;
                    }

                    float tempFillAmount;
                    if (i > 0)
                    { 
                        tempFillAmount = 0.2f + ((float)(exp - posyLikeAbilityScale.posyLikeAbilityLevels[i - 1].maxExp)
                                          / (posyLikeAbilityLevel.maxExp - posyLikeAbilityScale
                                              .posyLikeAbilityLevels[i - 1].maxExp)) * 0.6f;
                    }
                    else
                    {
                        tempFillAmount = 0.2f + ((float)(exp)
                                                 / (posyLikeAbilityLevel.maxExp)) * 0.6f;
                    }
                    
                    foreach (var vPercentImage in currentPosyLikeAbilityUICollection.posyLikeAbilityPercentImages)
                    {
                        if (i <= posyLikeAbilityScale.posyLikeAbilityLevels.Count - 2)
                        {
                            vPercentImage.sprite = posyLikeAbilityScale.posyLikeAbilityLevels[i+1].levelSprite;
                        }
                        else
                        {
                            vPercentImage.sprite = posyLikeAbilityScale.posyLikeAbilityLevels[^1].levelSprite;
                            vPercentImage.fillAmount = 1;
                            continue;
                        }
                        
                        vPercentImage.fillAmount = tempFillAmount;
                    }
                    
                    break;
                }

                i++;
            }
        }
        
        public void AddPosyLikeAbilityExp()
        {
#pragma warning disable CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
            EmpathyMailboxDatabaseManager.Instance.ChangePosyLikeAbility(_currentFairyName, earningsLikeAbilityExp);
#pragma warning restore CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
            SetAllPosyLikeAbilityLevelImages();
        }
    }

    [Serializable]
    public class PosyLikeAbilityUICollection
    {
        public string fairyName;
        public List<TextMeshProUGUI> posyLikeAbilityLevelTexts;
        public List<Image> posyLikeAbilityLevelImages;
        public List<Image> posyLikeAbilityPercentImages;
    }
}