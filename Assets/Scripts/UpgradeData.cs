using System;
using UnityEngine;

namespace DefaultNamespace
{
    [CreateAssetMenu(fileName = "UpgradeData", menuName = "DataObjects/UpgradeData", order = 0)]
    public class UpgradeData : ScriptableObject
    {
        public SpeedOffsetPair[] StateData;

        public SpeedOffsetPair GetStateAtLevel(int level)
        {
            if (level < 0 || level >= StateData.Length) return default;
            return StateData[level];
        }
    }

    [Serializable]
    public struct SpeedOffsetPair
    {
        public float Speed;
        public int Offset;
    }
}