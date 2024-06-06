using System;
using System.Collections.Generic;
using UnityEngine;

namespace Client
{
    [CreateAssetMenu(fileName = "SubjectImageHolder", menuName = "WarmMailbox/SubjectImageHolderInStoryMode", order = 1)]
    public class SubjectImageHolder : ScriptableObject
    {
        public Gender gender;
        public Age age;
        public List<SubjectImageCandidate> characterImageInfo;
    }

    [Serializable]
    public enum Gender
    {
        Male,
        Female
    }

    [Serializable]
    public enum Age
    {
        Child,
        Youth,
        Adult
    }

    [Serializable]
    public class SubjectImageCandidate
    {
        public string partName;
        public List<Sprite> candidateSprites;

        SubjectImageCandidate()
        {
            partName = "파트 이름 입력";
            candidateSprites = new List<Sprite>();
        }
    }
}
