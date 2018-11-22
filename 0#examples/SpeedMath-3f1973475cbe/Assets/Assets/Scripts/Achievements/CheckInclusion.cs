using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CheckInclusion : AchievementBase {
  public enum BufferTypes {
    Attention,
    Meditation
  }

  public BufferTypes bufferType;

  override protected void CheckConditions() {
    List<int> buffer = bufferType == BufferTypes.Attention ? attentionData : meditationData;

    for(int i = 0; i < buffer.Capacity; i++){
      if(buffer[i] >= lowerBound && buffer[i] <= upperBound)
        continue;
      else
        return;
    }

    TriggerAchievement();
  }
}
