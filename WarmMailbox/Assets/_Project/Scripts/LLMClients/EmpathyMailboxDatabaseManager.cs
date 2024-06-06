using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Client;
using Client.StoreAndInventory;
using LLMConnectModule.GPT;
using NUnit.Framework;
using SimpleJSON;
using Unity.Services.CloudSave;
using UnityEngine;
using UnityEngine.Serialization;
using Utils;

namespace LLMClients
{
    [Serializable]
    public class RespondMetaData
    {
        // ReSharper disable once NotAccessedField.Global
        public int storyModeDate; // 분류할 때 필요한 값
        // ReSharper disable once NotAccessedField.Global
        public int subjectKind; // 사람 표시할 때 필요한 값
        public List<PartNameAndIndex> subjectInformation; // 사람 표시할 때 필요한 값
        public string dateTimeId; // 따로 저장한 RespondData의 Key값
    }

    [Serializable]
    public class ConsultMetaData
    {
        public string fairyName; // 요정 초상화 표시할 때 필요한 값
        public string consultTitle; // 상담 제목 표시할 때 필요한 값
        public string dateTimeId; // 따로 저장한 ConsultData의 Key값
    }

    [Serializable]
    public class UserInfo
    {
        public int storyModeDate;
        public int todayConsultedCount;
        public int currency;
    }
    
    [Serializable]
    public class PosyLikeAbility 
    {
        public string posyName;
        public int likability;
        
        public PosyLikeAbility(string posyName, int likability)
        {
            this.posyName = posyName;
            this.likability = likability;
        }
    }
    
    public class EmpathyMailboxDatabaseManager : Singleton<EmpathyMailboxDatabaseManager>
    {
        public ConsultChatTitleCreator consultChatTitleCreator;
        
        public Worry currentWorry;
        public int currentSubjectKind;
        public List<PartNameAndIndex> currentPartNameAndIndices;
        private bool _isInitialized;
        public bool IsInitialized => _isInitialized;
        
        private UserInfo _userInfo = new UserInfo();
        private List<RespondMetaData> _respondMetaData = new();
        private List<ConsultMetaData> _consultMetaData = new();
        private List<PosyLikeAbility> _posyLikeAbilities = new();
        private List<string> _purchasedItemsIds = new();
        private List<string> _equippedItemsIds = new();

        #region UGS
        
        public void Initialize()
        {
            if (_isInitialized)
            {
                return;
            }
            
            // ReSharper disable once UnusedVariable
            GPTInterface gptInterface = GPTInterface.Instance;
            
            gptInterface.onGPTInitialized.AddListener(Call);
        }

        private async void Call()
        {
            await InitializeMetaData();
        }

        #endregion

        // Data Management 정리할 때, 각 항목 별로 나누고, 각 항목 내에서 CRUD(Get,Update)로 나누면 되겠다!!
        #region Data Management

        private async Task InitializeMetaData()
        {
            Debug.Log("[EmpathyMailboxDatabaseManager] InitializeMetaData Start");
            var savedData = await CloudSaveService.Instance.Data.Player.LoadAsync
                (new HashSet<string>{"consultMetaData", "respondMetaData", "userInfo", "posyLikeAbilities", "purchasedItems", "equippedItems"});
            
            if (savedData.TryGetValue("consultMetaData", out var consultMetaData))
            {
                Debug.Log("[EmpathyMailboxDatabaseManager] Done: " + consultMetaData.Value.GetAsString());
                var tempJsonArray = JSONNode.Parse(consultMetaData.Value.GetAsString()); // JSONArray parse.

                foreach (var variable in tempJsonArray.AsArray) // JSONArray to each JSONObject have key value.
                {
                    var tempConsultMetaData = new ConsultMetaData();
                    tempConsultMetaData.fairyName = variable.Value["fairyName"]; // Find JSONObject's element by key. 
                    tempConsultMetaData.consultTitle = variable.Value["consultTitle"];
                    tempConsultMetaData.dateTimeId = variable.Value["dateTimeId"];
                    
                    _consultMetaData.Add(tempConsultMetaData);
                }
            }
            else
            {
                _consultMetaData = new List<ConsultMetaData>();
                // Initialize with empty list
                await CloudSaveService.Instance.Data.Player.SaveAsync(new Dictionary<string, object>
                {
                    {"consultMetaData", _consultMetaData}
                });
            }
            
            if (savedData.TryGetValue("respondMetaData", out var respondMetaData))
            {
                var tempJsonArray = JSONNode.Parse(respondMetaData.Value.GetAsString());
                Debug.Log("[EmpathyMailboxDatabaseManager] tempJsonArray: " + tempJsonArray.ToString());
                
                foreach (var variable in tempJsonArray.AsArray)
                {
                    var tempRespondMetaData = new RespondMetaData();
                    tempRespondMetaData.storyModeDate = variable.Value["storyModeDate"];
                    tempRespondMetaData.subjectKind = variable.Value["subjectKind"];
                    
                    List<PartNameAndIndex> tempPartNameAndIndex = new List<PartNameAndIndex>();
                    JSONNode partNameAndIndices = JSONNode.Parse(variable.Value["subjectInformation"].ToString());
                    foreach (var partNameAndIndex in partNameAndIndices.AsArray)
                    {
                        tempPartNameAndIndex.Add(new PartNameAndIndex
                        {
                            partName = partNameAndIndex.Value["partName"],
                            index = partNameAndIndex.Value["index"]
                        });
                    }
                    tempRespondMetaData.subjectInformation = tempPartNameAndIndex;
                    
                    tempRespondMetaData.dateTimeId = variable.Value["dateTimeId"];
                    
                    _respondMetaData.Add(tempRespondMetaData);
                }
            }
            else
            {
                _respondMetaData = new List<RespondMetaData>();
                // Initialize with empty list
                await CloudSaveService.Instance.Data.Player.SaveAsync(new Dictionary<string, object>
                {
                    {"respondMetaData", _respondMetaData}
                });
            }
            
            if (savedData.TryGetValue("userInfo", out var userInfo))
            {
                Debug.Log("[EmpathyMailboxDatabaseManager] Done: " + userInfo.Value.GetAsString());
                var tempJsonArray = JSONNode.Parse(userInfo.Value.GetAsString());
                
                _userInfo.storyModeDate = tempJsonArray["storyModeDate"];
                _userInfo.todayConsultedCount = tempJsonArray["todayConsultedCount"];
                _userInfo.currency = tempJsonArray["currency"];
            }
            else
            {
                _userInfo = new UserInfo();
                await CloudSaveService.Instance.Data.Player.SaveAsync(new Dictionary<string, object>
                {
                    {"userInfo", _userInfo}
                });
            }
            
            if (savedData.TryGetValue("posyLikeAbilities", out var posyLikeAbilities))
            {
                Debug.Log("[EmpathyMailboxDatabaseManager] Done: " + posyLikeAbilities.Value.GetAsString());
                var tempJsonArray = JSONNode.Parse(posyLikeAbilities.Value.GetAsString());

                foreach (var variable in tempJsonArray.AsArray)
                {
                    var tempPosyLikeAbility = new PosyLikeAbility(variable.Value["posyName"], variable.Value["likability"]);
                    _posyLikeAbilities.Add(tempPosyLikeAbility);
                }
            }
            else
            {
                _posyLikeAbilities = new List<PosyLikeAbility>()
                {
                    new PosyLikeAbility("목화솜", 0), new PosyLikeAbility("무궁화", 0), 
                    new PosyLikeAbility("초롱꽃", 0), new PosyLikeAbility("진달래", 0)
                };
                // Initialize with empty list
                await CloudSaveService.Instance.Data.Player.SaveAsync(new Dictionary<string, object>
                {
                    {"posyLikeAbilities", _posyLikeAbilities}
                });
            }
            
            if (savedData.TryGetValue("purchasedItems", out var purchasedItems))
            {
                Debug.Log("[EmpathyMailboxDatabaseManager] purchasedItems: " + purchasedItems.Value.GetAsString());
                var tempJsonArray = JSONNode.Parse(purchasedItems.Value.GetAsString());

                foreach (var variable in tempJsonArray.AsArray)
                {
                    _purchasedItemsIds.Add(variable.Value);
                }
            }
            else
            {
                _purchasedItemsIds = new List<string>();
                // Initialize with empty list
                await CloudSaveService.Instance.Data.Player.SaveAsync(new Dictionary<string, object>
                {
                    {"purchasedItems", _purchasedItemsIds}
                });
            }
            
            if (savedData.TryGetValue("equippedItems", out var equippedItems))
            {
                Debug.Log("[EmpathyMailboxDatabaseManager] equippedItems: " + equippedItems.Value.GetAsString());
                var tempJsonArray = JSONNode.Parse(equippedItems.Value.GetAsString());

                foreach (var variable in tempJsonArray.AsArray)
                {
                    _equippedItemsIds.Add(variable.Value);
                }
            }
            else
            {
                _equippedItemsIds = new List<string>();
                // Initialize with empty list
                await CloudSaveService.Instance.Data.Player.SaveAsync(new Dictionary<string, object>
                {
                    {"equippedItems", _equippedItemsIds}
                });
            }
            
            Debug.Log("[EmpathyMailboxDatabaseManager] InitializeMetaData Done");
            _isInitialized = true;
        }

        #region Record respond and consult history
        
        public async Task RecordRespond(Respond respond)
        {
            if (!_isInitialized)
            {
                Debug.Log("[EmpathyMailboxDatabaseManager] RecordRespond Not Initialized");
                return;
            }

            RespondMetaData tempRespondMetaData = new RespondMetaData();
            tempRespondMetaData.storyModeDate = _userInfo.storyModeDate; 
            tempRespondMetaData.subjectKind = currentSubjectKind;
            tempRespondMetaData.subjectInformation = currentPartNameAndIndices; 
            tempRespondMetaData.dateTimeId = "Respond" + DateTime.Now.ToString("yyyy-MM-dd-mm-ss");
            
            _respondMetaData.Add(tempRespondMetaData);
            
            await CloudSaveService.Instance.Data.Player.SaveAsync(new Dictionary<string, object>
            {
                {"respondMetaData", _respondMetaData}
            });
            
            await CloudSaveService.Instance.Data.Player.SaveAsync(new Dictionary<string, object>
            {
                {tempRespondMetaData.dateTimeId, respond}
            });
        }

        public async Task RecordConsultHistory(List<ChatMessage> inputHistory)
        {
            if (inputHistory == null)
            {
                return;
            } 
            
            if (inputHistory.Count == 0 || string.IsNullOrEmpty(inputHistory[0].message))
            {
                return;
            }
            
            if (!_isInitialized)
            {
                Debug.Log("[EmpathyMailboxDatabaseManager] RecordRespond Not Initialized");
                return;
            }
            
            ConsultMetaData tempConsultMetaData = new ConsultMetaData();
            tempConsultMetaData.fairyName = FindObjectOfType<EmpathyMailboxLLMManager>().currentFairyName; // ! 요정 이름 !
            tempConsultMetaData.consultTitle = await consultChatTitleCreator.CreateAndReturnTitle(inputHistory); // ! 상담 제목 !
            tempConsultMetaData.dateTimeId = "Consult" + DateTime.Now.ToString("yyyy-MM-dd-mm-ss");
            
            _consultMetaData.Add(tempConsultMetaData);
            
            await CloudSaveService.Instance.Data.Player.SaveAsync(new Dictionary<string, object>
            {
                {"consultMetaData", _consultMetaData}
            });
            
            Debug.Log("[EmpathyMailboxDatabaseManager] inputHistory Check before save : " + inputHistory.Count + " / " + inputHistory[0]);
            
            await CloudSaveService.Instance.Data.Player.SaveAsync(new Dictionary<string, object>
            {
                {tempConsultMetaData.dateTimeId, inputHistory}
            });
        }
        
        #endregion
        
        #region Fetch Respond and Consult History

        public async Task<Respond> GetRespond(string dateTimeId)
        {
            if (!_isInitialized)
            {
                Debug.Log("[EmpathyMailboxDatabaseManager] RecordRespond Not Initialized");
                return new Respond();
            }

            var savedData = await CloudSaveService.Instance.Data.Player.LoadAsync
                (new HashSet<string>{dateTimeId});
            
            if (savedData.TryGetValue(dateTimeId, out var respondData))
            {
                var tempJsonRespond = JSONNode.Parse(respondData.Value.GetAsString());
                Respond tempRespond = new Respond();
                
                tempRespond.respondText = tempJsonRespond["respondText"];
                tempRespond.adviceText = tempJsonRespond["adviceText"];
                tempRespond.worry = new Worry
                {
                    worryText = tempJsonRespond["worry"]["worryText"],
                    features = tempJsonRespond["worry"]["features"]
                };

                return tempRespond;
            }
            else
            {
                Debug.LogError("[EmpathyMailboxDatabaseManager] GetRespond Failed : " + dateTimeId);
                return new Respond();
            }
        }
        
        public async Task<List<ChatMessage>> GetConsultHistory(string dateTimeId)
        {
            if (!_isInitialized)
            {
                Debug.Log("[EmpathyMailboxDatabaseManager] RecordRespond Not Initialized");
                return new List<ChatMessage>();
            }

            var savedData = await CloudSaveService.Instance.Data.Player.LoadAsync
                (new HashSet<string>{dateTimeId});
            
            if (savedData.TryGetValue(dateTimeId, out var consultData))
            {
                var tempJsonArray = JSONNode.Parse(consultData.Value.GetAsString());
                List<ChatMessage> tempChatHistory = new List<ChatMessage>();
                
                foreach (var variable in tempJsonArray.AsArray)
                {
                    ChatMessage tempChatMessage = new ChatMessage();
                    tempChatMessage.chatTargetType = (ChatTargetType)int.Parse(variable.Value["chatTargetType"]);
                    tempChatMessage.message = variable.Value["message"];
                    
                    tempChatHistory.Add(tempChatMessage);
                }

                return tempChatHistory;
            }
            else
            {
                Debug.LogError("[EmpathyMailboxDatabaseManager] GetConsultHistory Failed : " + dateTimeId);
                return new List<ChatMessage>();
            }
        }

        #endregion

        #region MetaData

        public List<RespondMetaData> GetRespondMetaData()
        {
            if (!_isInitialized)
            {
                Debug.Log("[EmpathyMailboxDatabaseManager] RecordRespond Not Initialized");
                return null;
            }

            return _respondMetaData;
        }
        
        public List<ConsultMetaData> GetConsultMetaData()
        {
            if (!_isInitialized)
            {
                Debug.Log("[EmpathyMailboxDatabaseManager] RecordRespond Not Initialized");
                return null;
            }

            return _consultMetaData;
        }

        #endregion
        
        #region UserInfo
        
        public int GetStoryModeDate()
        {
            if (!_isInitialized)
            {
                Debug.Log("[EmpathyMailboxDatabaseManager] RecordRespond Not Initialized");
                return -1;
            }

            return _userInfo.storyModeDate;
        }
        
        public int GetConsultedCount()
        {
            if (!_isInitialized)
            {
                Debug.Log("[EmpathyMailboxDatabaseManager] RecordRespond Not Initialized");
                return -1;
            }

            return _userInfo.todayConsultedCount;
        }
        
        public int GetCurrency()
        {
            if (!_isInitialized)
            {
                Debug.Log("[EmpathyMailboxDatabaseManager] RecordRespond Not Initialized");
                return -1;
            }

            return _userInfo.currency;
        }
        
        public async Task AddStoryModeDate()
        {
            if (!_isInitialized)
            {
                Debug.Log("[EmpathyMailboxDatabaseManager] RecordRespond Not Initialized");
                return;
            }
            
            _userInfo.storyModeDate++;
            
            await RecordUserInfo();
        }
        
        public async Task AddConsultedCount()
        {
            if (!_isInitialized)
            {
                Debug.Log("[EmpathyMailboxDatabaseManager] RecordRespond Not Initialized");
                return;
            }
            
            _userInfo.todayConsultedCount++;
            
            await RecordUserInfo();
        }
        
        public async Task ResetConsultedCount()
        {
            if (!_isInitialized)
            {
                Debug.Log("[EmpathyMailboxDatabaseManager] RecordRespond Not Initialized");
                return;
            }
            
            _userInfo.todayConsultedCount = 0;
            
            await RecordUserInfo();
        }
        
        public async Task ChangeCurrency(int inputDifference)
        {
            if (!_isInitialized)
            {
                Debug.Log("[EmpathyMailboxDatabaseManager] RecordRespond Not Initialized");
                return;
            }
            
            _userInfo.currency += inputDifference;
            
            await RecordUserInfo();
        }
        

        private async Task RecordUserInfo()
        {
            if (!_isInitialized)
            {
                Debug.Log("[EmpathyMailboxDatabaseManager] RecordRespond Not Initialized");
                return;
            }
            
            await CloudSaveService.Instance.Data.Player.SaveAsync(new Dictionary<string, object>
            {
                {"userInfo", _userInfo}
            });
        }
        
        #endregion

        #region PosyLikeAbility

        public int GetPosyLikeAbility(string posyName)
        {
            if (!_isInitialized)
            {
                Debug.Log("[EmpathyMailboxDatabaseManager] RecordRespond Not Initialized");
                return -1;
            }

            foreach (var posyLikeAbility in _posyLikeAbilities)
            {
                if (posyLikeAbility.posyName == posyName)
                {
                    return posyLikeAbility.likability;
                }
            }

            return -1;
        }
        
        public async Task ChangePosyLikeAbility(string posyName, int inputDifference)
        {
            if (!_isInitialized)
            {
                Debug.Log("[EmpathyMailboxDatabaseManager] RecordRespond Not Initialized");
                return;
            }

            foreach (var posyLikeAbility in _posyLikeAbilities)
            {
                if (posyLikeAbility.posyName == posyName)
                {
                    posyLikeAbility.likability += inputDifference;
                    break;
                }
            }
            
            await CloudSaveService.Instance.Data.Player.SaveAsync(new Dictionary<string, object>
            {
                {"posyLikeAbilities", _posyLikeAbilities}
            });
        }

        #endregion

        #region Item
        
        public List<string> GetPurchasedItemsIds()
        {
            if (!_isInitialized)
            {
                Debug.Log("[EmpathyMailboxDatabaseManager] RecordRespond Not Initialized");
                return null;
            }

            return _purchasedItemsIds;
        }
        
        public List<string> GetEquippedItems()
        {
            if (!_isInitialized)
            {
                Debug.Log("[EmpathyMailboxDatabaseManager] RecordRespond Not Initialized");
                return null;
            }

            return _equippedItemsIds;
        }
        
        public async Task PurchaseItem(string itemId)
        {
            if (!_isInitialized)
            {
                Debug.Log("[EmpathyMailboxDatabaseManager] RecordRespond Not Initialized");
                return;
            }

            _purchasedItemsIds.Add(itemId);
            
            await CloudSaveService.Instance.Data.Player.SaveAsync(new Dictionary<string, object>
            {
                {"purchasedItems", _purchasedItemsIds}
            });
        }
        
        public async Task EquipItem(string itemId)
        {
            if (!_isInitialized)
            {
                Debug.Log("[EmpathyMailboxDatabaseManager] RecordRespond Not Initialized");
                return;
            }

            _equippedItemsIds.Add(itemId);
            
            await CloudSaveService.Instance.Data.Player.SaveAsync(new Dictionary<string, object>
            {
                {"equippedItems", _equippedItemsIds}
            });
        }
        
        public async Task EquipItem(string removedItemId, string newItemId)
        {
            if (!_isInitialized)
            {
                Debug.Log("[EmpathyMailboxDatabaseManager] RecordRespond Not Initialized");
                return;
            }

            _equippedItemsIds.Remove(removedItemId);
            _equippedItemsIds.Add(newItemId);
            
            await CloudSaveService.Instance.Data.Player.SaveAsync(new Dictionary<string, object>
            {
                {"equippedItems", _equippedItemsIds}
            });
        }

        #endregion
        
        #endregion

        #region Event Function

        protected override void Awake()
        {
            base.Awake();
            Initialize();
        }

        #endregion
    }
}
