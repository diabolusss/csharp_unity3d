using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CheckDelta : AchievementBase {
  public enum BufferTypes {
    Attention,
    Meditation
  }

  public BufferTypes bufferType;

  private enum ConditionStates {
    WaitingForLowest,
    WaitingForHighest
  }

  private ConditionStates state;

  override protected void CheckConditions(){
    state = ConditionStates.WaitingForLowest;

    List<int> buffer = bufferType == BufferTypes.Attention ? attentionData : meditationData;

    for(int i = 0; i < buffer.Count; i++){
      switch(state){
        case ConditionStates.WaitingForLowest:
          if(buffer[i] <= lowerBound)
            state = ConditionStates.WaitingForHighest;

          break;
        case ConditionStates.WaitingForHighest:
          if(buffer[i] >= upperBound){
            TriggerAchievement();
            return;
          }

          break;
      }
    }
  }
}
