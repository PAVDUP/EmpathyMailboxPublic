using System;
using System.Collections.Generic;
using UnityEngine;

namespace Client
{
    [CreateAssetMenu(fileName = "DairySubjectsInfo", menuName = "WarmMailbox/DairySubjectsInfo", order = 1)]
    public class DairySubjectsInfo : ScriptableObject
    {
        public List<DairySubjects> dairySubjects;
    }
    
    [Serializable]
    public class DairySubjects
    {
        public int todayDate;
        public int todaySubjectsCount;
    }
}
