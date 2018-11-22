using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class AchievementBase : MonoBehaviour {

  [System.Serializable]
  public class Achievement {
    public string taskName;
    public string description;
    public int pointValue;
  }

  public Achievement achievement;

  public int lowerBound;
  public int upperBound;
  public int numElements;
  public bool completed = false;
  
  protected List<int> attentionData;
  protected List<int> meditationData;

  void Start(){
    attentionData = new List<int>(numElements);
    meditationData = new List<int>(numElements);

    OnReset();
  }

  virtual public void OnDataReceived(int [] data){
    while(attentionData.Count >= numElements)
      attentionData.RemoveAt(0);

    while(meditationData.Count >= numElements)
      meditationData.RemoveAt(0);

    attentionData.Add(data[0]);
    meditationData.Add(data[1]);

    CheckConditions();
  }

  public void OnReset(){
    attentionData.Clear();
    meditationData.Clear();

    for(int i = 0; i < numElements; i++){
      attentionData.Add(-1);
      meditationData.Add(-1);
    }
  }

  public void TriggerCompleted(){
    this.enabled = false;
    completed = true;
  }

  virtual protected void CheckConditions(){}

  void OnLoadDeactivate(string [] deactivatedAchievements){
    if(((IList<string>)deactivatedAchievements).Contains(name)){
      TriggerCompleted();
    }
  }

  protected void TriggerAchievement(){
    SendMessage("OnAchievementCompleted", achievement, SendMessageOptions.DontRequireReceiver); 
    TriggerCompleted();
  }
}
