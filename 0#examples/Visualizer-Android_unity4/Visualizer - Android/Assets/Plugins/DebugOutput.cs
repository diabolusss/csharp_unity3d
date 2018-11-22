using UnityEngine;
using System.Collections;

public class DebugOutput : MonoBehaviour {

  public bool enableDebugOutput = false;

  private bool isConnected = false;
  private DataController d;

  private GUISkin skin;

  void Start(){
    d = (DataController)GameObject.Find("ThinkGear").GetComponent(typeof(DataController));

    OnLanguageChanged();

    skin = Localizer.Skins["thinkgear"];
  }

  void OnLanguageChanged(){
    skin = Localizer.Skins["thinkgear"];
  }

  void OnHeadsetConnected(){
    isConnected = true;
  }

  void OnHeadsetDisconnected(){
    isConnected = false;
  }

  void OnGUI(){
    GUI.skin = skin;

    if(isConnected && enableDebugOutput){
      GUILayout.Space(10);
      GUILayout.BeginHorizontal();
      GUILayout.Space(10);
      GUILayout.Label(d.headsetData.ToString());
      GUILayout.EndHorizontal();
    }
  }
}
