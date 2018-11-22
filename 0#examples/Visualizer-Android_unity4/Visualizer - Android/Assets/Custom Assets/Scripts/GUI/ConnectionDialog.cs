using UnityEngine;
using System.Collections;

public class ConnectionDialog : MonoBehaviour {
  private enum State {
    Disconnected,
    Scanning,
    Connected
  }

  private State state = State.Disconnected;
  private bool enableDemoMode = false;


  void OnHeadsetConnected(){
    state = State.Connected;
  }

  void OnHeadsetDisconnected(){
    state = State.Disconnected;
  }

  void OnGUI(){
    GUILayout.Label("Time: " + Time.time);

    switch(state){
      case State.Disconnected:
        GUILayout.BeginHorizontal();

        if(GUILayout.Button("Connect")){
            GameHelper.SendMessageToAll("OnRequestConnect", enableDemoMode, SendMessageOptions.DontRequireReceiver);
        }
      
        enableDemoMode = GUILayout.Toggle(enableDemoMode, " Enable demo mode");

        GUILayout.EndHorizontal();
        
        break;
      case State.Connected:
        GUILayout.BeginHorizontal();
        GUILayout.Label("Connected");

        GUILayout.Space(20);

        if(GUILayout.Button("Disconnect")){
            GameHelper.SendMessageToAll("OnRequestDisconnect", null, SendMessageOptions.DontRequireReceiver);
        }

        GUILayout.EndHorizontal();

        break;
    }
  }
}
