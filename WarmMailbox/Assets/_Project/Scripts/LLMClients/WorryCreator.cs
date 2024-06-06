using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using _Project.Scripts.LLMClients;
using Client.StoreAndInventory;
using SimpleJSON;
using UnityEngine;
using UnityEngine.Events;
using Random = UnityEngine.Random;

namespace LLMClients
{
    /// <summary>
    /// WorryCreator 는 주어진 topicFeatureContainers 에 따라서 Worry 를 생성하고, 이를 기반으로 동작을 실행하는 클래스.
    /// - CreateAndIndicateWorryInternal : 생성된 prompt contents 로 worry 생성 api를 호출, await 후, Worry 를 반환.
    /// - CreateAndIndicateWorryInternalWithoutAwait : 생성된 prompt contents 로 worry 생성 api를 호출, 해당 시점이 아닌 다른 시점에서 실행할 수 있도록 함.
    ///     - 이 경우, 빈 Worry 를 반환. / onCompleteCreateWorry 를 Invoke 해 Worry 를 반환하는 함수를 따로 제작하여야 함.
    /// </summary>
    public abstract class WorryCreator : MonoBehaviour
    {
        private bool _isInitialized;
        public List<TopicFeatureHolder> topicFeatureHolders = new List<TopicFeatureHolder>();
        public UnityEvent<Worry> onCompleteCreateWorry;
        
        private bool _isCreatingWorry;

        public virtual void Initialize()
        {
            _isInitialized = true;
        }
        
        public JSONNode CreateWorryTopicPrompt()
        {
            JSONNode worryTopicPrompt = new JSONObject();

            // 1. 각 topic 주제별 후보군 별로 반복. => 결과 : worry Prompt 를 확률에 기반하여 설정 완료.
            foreach (var topicFeatureContainer in topicFeatureHolders)
            {
                int sumOfPossibility = 0;

                // 2. 독립사건, 종속사건 유무에 따른 분기.
                if (topicFeatureContainer.isIndependent)
                {
                    // 2. 독립사건이므로 전체 확률 100으로 고정.
                    sumOfPossibility = 100;

                    foreach (var topicCandidate in topicFeatureContainer.topicCandidates)
                    {
                        var randomValue = Random.Range(0, sumOfPossibility);
                        if (topicCandidate.possibility > randomValue)
                        {
                            if (worryTopicPrompt[topicFeatureContainer.topicName] == null)
                                worryTopicPrompt[topicFeatureContainer.topicName] = new JSONArray();

                            worryTopicPrompt[topicFeatureContainer.topicName].Add(topicCandidate.value);
                        }
                    }
                }
                else
                {
                    // 2. 각 topic 주제 내 후보군을 뽑아내기.
                    List<TopicCandidate> topicCandidates = topicFeatureContainer.topicCandidates;

                    // 3. 단일 topic 내 후보군의 확률 총합이 100 이상일 경우, 그것을 확률로서 삼음. 아니라면, 100을 확률로서 삼음.
                    foreach (var topicCandidate in topicCandidates)
                    {
                        sumOfPossibility += topicCandidate.possibility;
                    }

                    if (sumOfPossibility <= 100) sumOfPossibility = 100;

                    // 4. 확률 기반 해당 주제의 topic 을 설정.  
                    var randomValue = Random.Range(0, sumOfPossibility);
                    foreach (var topicCandidate in topicCandidates)
                    {
                        randomValue -= topicCandidate.possibility;
                        Debug.Log($"[WorryCreator] random value : {randomValue}");
                        
                        if (randomValue <= 0)
                        {
                            Debug.Log($"[WorryCreator] added topic : {topicFeatureContainer.topicName} : {topicCandidate.value}");
                            worryTopicPrompt[topicFeatureContainer.topicName] = topicCandidate.value;
                            break;
                        }
                    }
                }
            }
            
            // GetEquippedItems() 을 통해 현재 장착된 아이템들을 가져온 후, 해당 아이템들의 Prompt 를 추가.
            List<EmpathyMailboxItem> items = FindObjectOfType<ItemManager>().GetEquippedItems();
            foreach (var item in items)
            {
                if (Random.Range(0, 100) > 90)
                {
                    if (worryTopicPrompt["특수 키워드"] == null)
                        worryTopicPrompt["특수 키워드"] = new JSONArray();

                    worryTopicPrompt["특수 키워드"].Add(item.additionalWorryPrompt);
                }
            }
            
            // ReSharper disable once RedundantToStringCall
            Debug.Log("[WorryCreator] worryTopicPrompt : " + worryTopicPrompt.ToString());
            return worryTopicPrompt;
        }

        
        public async Task<Worry> CreateAndIndicateWorry(List<SpecificTopic> addedTopics = null)
        {
            // 1. Worry 가 이미 생성 중인 경우, 에러를 출력하고, 빈 Worry 를 반환.
            if (_isCreatingWorry)
            {   
                Debug.LogError("[WorryCreator] CreateAndIndicateWorry : Worry is already being created.");
                return new Worry();
            }
            else
            {
                _isCreatingWorry = true;
            }
            
            if (_isInitialized == false)
            {
                Initialize();
            }

            // 2. Worry 생성 및 반환.
            Worry tempWorry;
            try
            {
                var tempWorryTopicPrompt = CreateWorryTopicPrompt();
                Debug.Log(tempWorryTopicPrompt);

                if (addedTopics != null)
                    foreach (var specificTopic in addedTopics)
                    {
                        tempWorryTopicPrompt[specificTopic.topicName] = specificTopic.topicValue;
                    }

                // ReSharper disable once RedundantToStringCall
                Debug.Log("[WorryCreator] worryTopicPrompt : " + tempWorryTopicPrompt.ToString());

                try
                {
                    tempWorry = await CreateAndIndicateWorryInternal(tempWorryTopicPrompt, addedTopics);
                }
                catch (Exception e)
                {
                    _isCreatingWorry = false;
                    Console.WriteLine(e);
                    throw;
                }

                onCompleteCreateWorry.Invoke(tempWorry);
                EmpathyMailboxDatabaseManager.Instance.currentWorry = tempWorry;
            } 
            catch (Exception e)
            {
                _isCreatingWorry = false;
                Console.WriteLine(e);
                throw;
            }
            
            _isCreatingWorry = false;
            return tempWorry;
        }

        protected abstract Task<Worry> CreateAndIndicateWorryInternal(JSONNode worryTopicPrompt,
            List<SpecificTopic> addedTopics = null);
    }
}
