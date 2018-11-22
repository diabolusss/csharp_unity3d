using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[RequireComponent(typeof(CSCBridge))]
public class AchievementController : MonoBehaviour {
  public Dictionary<string, AchievementBase> allAchievements;

  private DataController thinkGearClient;

  private Dictionary<string, AchievementBase.Achievement> queuedAchievements;

  private CSCBridge bridge;

  private bool hasSentAchievements = false;

  public enum ConnectorStates {
    None,
    SentIdentification,
    IdentifyOk
  }

  private ConnectorStates state = ConnectorStates.None; 

  void Awake(){
    bridge = GetComponent<CSCBridge>();
    queuedAchievements = new Dictionary<string, AchievementBase.Achievement>();
    allAchievements = new Dictionary<string, AchievementBase>();
  }

	// Use this for initialization
	void Start () {
    thinkGearClient = GameObject.Find("ThinkGear").GetComponent<DataController>();
    bridge.Identify();
    state = ConnectorStates.SentIdentification;
	}

  void OnAchievementStartRequested(){
    Debug.Log("Achievement tracking started.");
    InvokeRepeating("InsertData", 0.0f, 1.0f);	
  }

  void OnAchievementStopRequested(){
    Debug.Log("Achievement tracking stopped.");
    CancelInvoke("InsertData");
  }

  void OnApplicationQuit(){
    OnAchievementStopRequested();
  }

  void OnServerStatusResponse(ServerResponse s){
    switch(state){
      case ConnectorStates.SentIdentification:
        if(s.status == ServerResponse.StatusTypes.NeedInfo)
          bridge.SendAppInfo(new AchievementBase.Achievement[0]);
        else {
          AchievementBase[] achievements = this.GetComponents<AchievementBase>();

          foreach(AchievementBase a in achievements){
            allAchievements.Add(a.achievement.taskName, a);
            bridge.GetTask(a.achievement.taskName);
          }

          state = ConnectorStates.IdentifyOk;
        }

        break;
    }
  }

  void OnServerTaskResponse(Task t){
    if(t.completed){
      allAchievements[t.taskName].TriggerCompleted();
      allAchievements.Remove(t.taskName);
    }
  }

  void OnServerError(ErrorResponse e){
    switch(e.error){
      case ErrorResponse.ErrorTypes.BadRequestFormat:
        Debug.Log("Bad request format.");
        break;

      case ErrorResponse.ErrorTypes.UnknownMethod:
        Debug.Log("Unknown method.");
        break;

      case ErrorResponse.ErrorTypes.InvalidAppKey:
        Debug.Log("Invalid app key.");
        bridge.SendAppInfo(new AchievementBase.Achievement[0]);
        state = ConnectorStates.SentIdentification;
        break;

      case ErrorResponse.ErrorTypes.InvalidTaskName:
        Debug.Log("Invalid task name.");
        bridge.SendAppInfo(new AchievementBase.Achievement[0]);
        state = ConnectorStates.SentIdentification;
        break;
    }
  }
	
  void InsertData(){
    if(thinkGearClient.IsHeadsetInitialized && thinkGearClient.headsetData.poorSignalValue == 0){
      SendMessage("OnDataReceived", new int[2]{thinkGearClient.headsetData.attention, 
                                               thinkGearClient.headsetData.meditation}, 
                  SendMessageOptions.DontRequireReceiver);
    }
  }

  void OnAchievementCompleted(AchievementBase.Achievement a){
    bridge.CompleteTask(a.taskName);
  }
}
