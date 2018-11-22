using UnityEngine;
using System.Collections;

public class GameButtonHandler : MonoBehaviour {
  private GameRunner runner;

  public bool showRestart = true;

  public GUISkin skin;

  private bool showMenu = false;

  void Awake(){
    runner = GameObject.Find("Game Controller").GetComponent<GameRunner>();
  }

  void Start(){
    skin = Localizer.Skins["GameGUI"];
  }

  void OnGUI(){
    if(Event.current.type == EventType.KeyDown && Event.current.keyCode == KeyCode.Escape){
      showMenu = !showMenu;
      runner.TogglePause();
    }
    else {
      GUI.depth = 0;
      GUI.skin = skin;

      if(showMenu){
        GUILayout.BeginVertical(GUILayout.Height(Screen.height));
        GUILayout.FlexibleSpace();

        GUILayout.BeginHorizontal(GUILayout.Width(Screen.width));

        GUILayout.FlexibleSpace();

        GUILayout.BeginVertical("Box");

        if(showRestart){
          if(GUILayout.Button(Localizer.Content["game"]["restart"]))
            Application.LoadLevel("Game");

          GUILayout.Space(30);
        }

        if(GUILayout.Button(Localizer.Content["game"]["backmenu"]))
          Application.LoadLevel("MainMenu");

        GUILayout.EndVertical();

        GUILayout.FlexibleSpace();

        GUILayout.EndHorizontal();

        GUILayout.FlexibleSpace();
        GUILayout.EndVertical();
      }
    }
  }
}
