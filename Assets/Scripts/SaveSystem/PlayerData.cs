using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class PlayerData
{
   public int level;
   public int gold;
   public int currentExp;
   public int maxExp;
   public int remainingStats;
   public Dictionary<PlayerData.Classes,int[]> classAndStats;

   public PlayerData(PlayerAttributes data)
   {
      level = data.playerLevel;
      gold = data.playerGold;
      currentExp = data.playerCurrentExperience;
      maxExp = data.playerMaxExperience;
      remainingStats = data.statPoints;
      classAndStats = data.classAndStats;
   }
   public enum Classes
   {
      warrior,
      mage,
      archer
   }
}
