using UnityEngine;
using System.Collections;

public class CheckZone : AchievementBase {
  override protected void CheckConditions(){
    for(int i = 0; i < attentionData.Capacity; i++){
      if(InBounds(attentionData[i], lowerBound, upperBound) &&
         InBounds(meditationData[i], lowerBound, upperBound))
        continue;
      else
        return;
    }

    TriggerAchievement();
  }

  private bool InBounds(int value, int lowerBound, int upperBound){
    return value >= lowerBound && value <= upperBound;
  }
}
