using System.Collections.Generic;
using UnityEngine;

namespace LLMClients
{
    [CreateAssetMenu(fileName = "Topic Feature", menuName = "WarmMailbox/WorryTopicFeatureHolder", order = 1)]
    public class TopicFeatureHolder : ScriptableObject
    {
        public string topicName;
        public bool isIndependent;
        public List<TopicCandidate> topicCandidates;
    }
}