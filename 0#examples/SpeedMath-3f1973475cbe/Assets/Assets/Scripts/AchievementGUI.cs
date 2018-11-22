using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[RequireComponent(typeof(AchievementController))]
public class AchievementGUI : MonoBehaviour {

  public GUISkin skin;
  public float timeoutInterval = 5.0f;
  public Texture2D achievementIcon;

  public bool enableAchievementPopups = false;

  private GUIStyle boxStyle;
  private GUIStyle headerStyle;
  private GUIStyle labelStyle;
  private GUIStyle smallHeaderStyle;
 
  private bool displayAchievementList = false;
  private bool displayAchievement = false;
  private Vector2 scrollPosition;
  private Rect scrollViewBox;
  private Rect viewPanel;

  private AchievementController controller;

  private AchievementBase.Achievement currentAchievement;

  private bool displayServerNotFound = false;

  void Awake(){
    controller = GetComponent<AchievementController>();
  }

	// Use this for initialization
	void Start () {
    scrollViewBox = new Rect(Screen.width / 3, 0, Screen.width / 3, Screen.height);
    viewPanel = new Rect(0, 0, Screen.width / 3, 1000);

    boxStyle = skin.GetStyle("MediaBar");
    headerStyle = skin.GetStyle("SongTitle");
    labelStyle = skin.GetStyle("SongName");
    smallHeaderStyle = skin.GetStyle("StatusBar Header");
	}

  void OnAchievementCompleted(AchievementBase.Achievement a){
    CancelInvoke("HideAchievementPrompt");
    currentAchievement = a;
    displayAchievement = true;

    Invoke("HideAchievementPrompt", timeoutInterval);
  }

  void OnServerNotFound(){
    displayServerNotFound = true;
  }

  void OnGUI(){
  	// Disable annoying CSC prompt
/*  	
    if(displayServerNotFound){
      GUI.depth = 0;

      GUILayout.BeginVertical("Box", GUILayout.Height(Screen.height));
      GUILayout.FlexibleSpace();

      GUILayout.BeginHorizontal(GUILayout.Width(Screen.width));
      GUILayout.FlexibleSpace();

      GUILayout.BeginVertical("Box", GUILayout.Width(250));

      GUILayout.Label(Localizer.Content["cscpanel"]["notfound"]);
      GUILayout.Space(20);

      if(GUILayout.Button(Localizer.Content["cscpanel"]["close"])){
        displayServerNotFound = false;
      }

      GUILayout.EndVertical();

      GUILayout.FlexibleSpace();
      GUILayout.EndHorizontal();

      GUILayout.FlexibleSpace();
      GUILayout.EndVertical();

      if(Event.current.type == EventType.MouseDown)
        Event.current.Use();
    }
*/
    if(enableAchievementPopups && displayAchievement){
      GUILayout.BeginVertical(GUILayout.Height(Screen.height));
      GUILayout.Space(Screen.height / 2);

      GUILayout.BeginHorizontal(GUILayout.Width(Screen.width));
    
      GUILayout.FlexibleSpace();
   
      GUILayout.BeginHorizontal(boxStyle);

      GUILayout.BeginVertical();
      GUILayout.Label(achievementIcon);
      GUILayout.Space(10);
      GUILayout.EndVertical();

      GUILayout.Space(10);

      GUILayout.BeginVertical();

      GUILayout.Label(currentAchievement.taskName + " - " + currentAchievement.pointValue + " points", headerStyle);
      GUILayout.Label(currentAchievement.description, labelStyle);

      GUILayout.EndVertical();

      GUILayout.EndHorizontal();

      GUILayout.FlexibleSpace();

      GUILayout.EndHorizontal();

      GUILayout.FlexibleSpace();

      GUILayout.EndVertical();
    }

    if(enableAchievementPopups && displayAchievementList){
      GUILayout.BeginVertical(GUILayout.Height(Screen.height));
      GUILayout.BeginHorizontal(GUILayout.Width(Screen.width));

      GUILayout.FlexibleSpace();
      GUILayout.BeginVertical();
      GUILayout.FlexibleSpace(); 
      GUILayout.BeginHorizontal("Box");
      GUILayout.BeginVertical();
      GUILayout.Label("Completed Achievements", headerStyle);
      GUILayout.Space(15);

      foreach(KeyValuePair<string, AchievementBase> k in controller.allAchievements){
        AchievementBase.Achievement a = k.Value.achievement;

        GUILayout.Label(a.taskName + " - " + a.pointValue + " points", smallHeaderStyle);
        GUILayout.Label(a.description, labelStyle);
      }

      GUILayout.EndVertical();
      GUILayout.EndHorizontal();
      GUILayout.FlexibleSpace();
      GUILayout.EndVertical();
      GUILayout.FlexibleSpace();

      GUILayout.EndHorizontal();

      GUILayout.EndVertical();
    }
  }

  void HideAchievementPrompt(){
    displayAchievement = false;
  }
}

