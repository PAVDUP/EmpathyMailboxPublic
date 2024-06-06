using System;
using System.Collections.Generic;
using UnityEngine;

namespace Client
{
    
    public class PosyLikeAbilityScale : ScriptableObject
    {
        public List<PosyLikeAbilityLevel> posyLikeAbilityLevels; 
    }
    
    [Serializable]
    public class PosyLikeAbilityLevel
    {
        public int level;
        public int maxExp;
        public Sprite levelSprite;
    }
}
