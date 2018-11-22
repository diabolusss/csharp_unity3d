using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Pinwheel))]
public class DelayedQuitter: MonoBehaviour {

  public float minTimeBeforeQuit = 10.0f;
  public float minUnloadingDisplayPeriod = 2.0f; 
  public float doneDisplayPeriod = 0.5f;

  private float doneTime;
  private float exitTime;

  private int labelHeight = 40;

  private GUISkin skin;

  private string progressMessage;
  private string doneMessage;
  private string exitingMessage;

  private Pinwheel pinwheel;
  
  private enum UnloadStates {
    Unloading,
    Done,
    Exiting
  }

  private UnloadStates state = UnloadStates.Unloading;

  void Awake(){
    OnLanguageChanged();
  }

	// Use this for initialization
	void Start () {
    pinwheel = (Pinwheel)GetComponent("Pinwheel");

    // determine what time to show the "done" message
    // if the app hasn't run for a minimum time, then run it for that minimum time
    // otherwise, just add a standard delta to the current time
    doneTime = Time.time < minTimeBeforeQuit ? minTimeBeforeQuit : Time.time + minUnloadingDisplayPeriod;

    // determine what time to show the "exit" message and quit the ap
    exitTime = doneTime + doneDisplayPeriod;
	}

  void OnLanguageChanged(){
    skin = Localizer.Skins["main"];
    progressMessage = Localizer.Content["exitsplash"]["cleanup"];
    doneMessage = Localizer.Content["exitsplash"]["done"];
    exitingMessage = Localizer.Content["exitsplash"]["exiting"]; 
  }
	
	// Update is called once per frame
	void Update () {
    switch(state){
      case UnloadStates.Unloading:
        if(Time.time >= doneTime)
          state = UnloadStates.Done;

        break;
      case UnloadStates.Done:
        if(Time.time >= exitTime)
          state = UnloadStates.Exiting;

        break;
      case UnloadStates.Exiting:
        Application.Quit();

        break;
    }
	}
  
  void OnGUI(){
    GUI.skin = skin;

    GUILayout.BeginVertical(GUILayout.Height(Screen.height));
    GUILayout.FlexibleSpace();

    GUILayout.BeginHorizontal(GUILayout.Width(Screen.width));
    GUILayout.FlexibleSpace();

    GUILayout.BeginVertical(skin.box, GUILayout.Width(500));

    switch(state){
      case UnloadStates.Unloading:
        GUILayout.BeginHorizontal();
        GUILayout.BeginVertical(GUILayout.Height(labelHeight));
        GUILayout.FlexibleSpace();
        GUILayout.Label(progressMessage);
        GUILayout.FlexibleSpace();
        GUILayout.EndVertical();
        GUILayout.Space(20);
        GUILayout.BeginVertical(GUILayout.Height(labelHeight));
        GUILayout.FlexibleSpace();
        GUILayout.Label(pinwheel.currentFrame);
        GUILayout.FlexibleSpace();
        GUILayout.EndVertical();
        GUILayout.FlexibleSpace();
        GUILayout.EndHorizontal();
        break;
      
      case UnloadStates.Done:
        GUILayout.BeginHorizontal();
        GUILayout.BeginVertical(GUILayout.Height(labelHeight));
        GUILayout.FlexibleSpace();
        GUILayout.Label(doneMessage);
        GUILayout.FlexibleSpace();
        GUILayout.EndVertical();
        GUILayout.EndHorizontal();
        break;

      case UnloadStates.Exiting:
        GUILayout.BeginHorizontal();
        GUILayout.BeginVertical(GUILayout.Height(labelHeight));
        GUILayout.FlexibleSpace();
        GUILayout.Label(exitingMessage);
        GUILayout.FlexibleSpace();
        GUILayout.EndVertical();
        GUILayout.EndHorizontal();
        break;
    }

    GUILayout.EndVertical();

    GUILayout.FlexibleSpace();
    GUILayout.EndHorizontal();

    GUILayout.FlexibleSpace();
    GUILayout.EndVertical();
  }
}
