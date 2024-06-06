using System;
using SimpleJSON;
using UnityEngine.Serialization;

namespace LLMClients
{
    // 특정 주제의 후보
    [Serializable]
    public struct TopicCandidate
    {
        public int possibility;
        public string value;
    }

    [Serializable]
    public struct SpecificTopic
    {
        public string topicName;
        public string topicValue;
    }
    
    [Serializable]
    public struct Worry
    {
        public string worryText;
        public string features;
    }

    [Serializable]
    public struct Respond
    {
        public Worry worry;
        public string adviceText;
        public string respondText;
    }
}
