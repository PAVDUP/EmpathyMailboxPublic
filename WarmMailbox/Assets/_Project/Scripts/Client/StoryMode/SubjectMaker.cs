using System;
using System.Collections.Generic;
using LLMClients;
using SimpleJSON;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

namespace Client
{
    public class SubjectMaker : MonoBehaviour
    {
        public List<SubjectPartCollection> subjectPartCollections;
        public List<SubjectImageHolder> subjectImageHolders;
    
        public void SetSubjectImage(Worry inputWorry)
        {
            JSONNode worryFeatures = JSONNode.Parse(inputWorry.features);
            
            string ageInWorry = worryFeatures["고민자 나이"];
            string genderInWorry = worryFeatures["고민자 성별"];
            
            Debug.Log(worryFeatures.ToString());
            Debug.Log($"고민자 나이: {ageInWorry} / 고민자 성별 : {genderInWorry}");

            var currentAge = ageInWorry switch
            {
                "아동" => Age.Child,
                "청년" => Age.Youth,
                _ => Age.Adult
            };

            var currentGender = genderInWorry == "남자" ? Gender.Male : Gender.Female;
            
            Debug.Log($"현재 나이: {currentAge} / 현재 성별 : {currentGender}");

            List<PartNameAndIndex> partNameAndIndices = new List<PartNameAndIndex>();

            int subjectKind = 0;
            
            foreach (var subjectImage in subjectImageHolders)
            {
                if (subjectImage.age == currentAge && subjectImage.gender == currentGender)
                {
                    foreach (var subjectImageCandidate in subjectImage.characterImageInfo)
                    {
                        int randomIndex = Random.Range(0, subjectImageCandidate.candidateSprites.Count);
                        
                        partNameAndIndices.Add(new PartNameAndIndex
                        {
                            partName = subjectImageCandidate.partName,
                            index = randomIndex
                        });
                        
                        foreach (var subjectPartImages in subjectPartCollections)
                        {
                            foreach (var subjectPart in subjectPartImages.subjectPartImages)
                            {
                                if (subjectPart.name == subjectImageCandidate.partName)
                                {
                                    subjectPart.sprite = subjectImageCandidate.candidateSprites[randomIndex];
                                    
                                    break;
                                }
                            }
                        }
                    }
                    
                    break;
                }
                
                subjectKind++;
            }
            
            EmpathyMailboxDatabaseManager.Instance.currentSubjectKind = subjectKind;
            EmpathyMailboxDatabaseManager.Instance.currentPartNameAndIndices = partNameAndIndices;
        }

        public bool SetImageWithRespondMetaDataAndPartCollection(RespondMetaData respondMetaData, SubjectPartCollection subjectPartCollection)
        {
            if (respondMetaData.subjectKind < 0 || respondMetaData.subjectKind >= subjectImageHolders.Count)
            {
                Debug.LogError("Invalid subject kind");
                return false;
            }

            var subjectImage = subjectImageHolders[respondMetaData.subjectKind];

            foreach (var partNameAndIndex in respondMetaData.subjectInformation)
            {
                foreach (var eachPart in subjectImage.characterImageInfo)
                {
                    if (eachPart.partName == partNameAndIndex.partName)
                    {
                        foreach (var image in subjectPartCollection.subjectPartImages)
                        {
                            if (image.name == partNameAndIndex.partName)
                            {
                                image.sprite = eachPart.candidateSprites[partNameAndIndex.index];
                                Debug.Log($"Set image: {image.name} / {eachPart.candidateSprites[partNameAndIndex.index].name}");
                                break;
                            }
                        }
                        
                        break;
                    }
                }
            }

            return true;
        }
    }

    [Serializable]
    public class PartNameAndIndex
    {
        public string partName;
        public int index;
    }

    [Serializable]
    public class SubjectPartCollection
    {
        public List<Image> subjectPartImages;
    }
}
