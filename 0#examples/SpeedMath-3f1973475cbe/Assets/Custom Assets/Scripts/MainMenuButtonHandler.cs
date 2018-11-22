using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MainMenuButtonHandler : MonoBehaviour {

  private GUISkin skin;
  private Dictionary<string, string> l;
  private bool showIntro;
  private int padding;
  
  void Start(){
    skin = Localizer.Skins["GameGUI"]; 
    l = Localizer.Content["game"];
    
    switch(Localizer.appLanguage) {
    	case Localizer.Language.en:
    		padding = Screen.height / 7;
    		break;
    	case Localizer.Language.zh_cn:
    	case Localizer.Language.zh_tw:
    		padding = Screen.height / 10;
    		break;
    	default:
    		padding = Screen.height / 7;
    		break;    	
    }
    showIntro = false;
  }

  void OnGUI(){	
	if(showIntro) {
		GUILayout.BeginVertical("Box", GUILayout.Height(Screen.height));
		GUILayout.FlexibleSpace();		
		GUILayout.BeginHorizontal(GUILayout.Width(Screen.width));
		GUILayout.FlexibleSpace();
		
		GUILayout.BeginVertical("Box", GUILayout.Width(250));
		
		GUILayout.Label(l["intro"]);			
		GUILayout.Space(20);			
		if(GUILayout.Button(Localizer.Content["cscpanel"]["close"])){
			showIntro = false;
		}
		
		GUILayout.EndVertical();
				
		GUILayout.FlexibleSpace();
		GUILayout.EndHorizontal();
		GUILayout.Space(Screen.height / 5);
		GUILayout.EndVertical();	
	}
	else {	
		GUI.skin = skin;
	    GUILayout.BeginVertical(GUILayout.Height(Screen.height));
	    GUILayout.FlexibleSpace();
	    GUILayout.BeginHorizontal(GUILayout.Width(Screen.width));
	
	    GUILayout.FlexibleSpace();
	    
	    GUILayout.BeginVertical();
	
	    if(GUILayout.Button(l["newgame"])){
	      Application.LoadLevel("LevelSelect");
	    }
	
	    if(GUILayout.Button(l["viewscores"])){
	      Application.LoadLevel("Statistics");
	    }
	
	    if(GUILayout.Button(l["options"])){
	      Application.LoadLevel("Options");
	    }
		
		if(GUILayout.Button(l["about"])){
			showIntro = true;
		}
	    if(GUILayout.Button(l["quit"])){
	      Application.Quit();
	    }
	
	    GUILayout.EndVertical();
	
	    GUILayout.FlexibleSpace();
	
	    GUILayout.EndHorizontal();
	    GUILayout.Space(padding);
	    GUILayout.EndVertical();
	}
  }
}
