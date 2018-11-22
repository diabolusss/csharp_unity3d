using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class LevelSelectController : MonoBehaviour {
	
	public enum MenuStates {
		Grade,
		Tests
	}
	
	public Texture2D background;
	private Rect backgroundRect;
	private GUISkin skin;
	private Dictionary<string, string> l;
	
	private MenuStates menuState = MenuStates.Grade;
	
	// Use this for initialization
	void Start () {
		if(background) {
			backgroundRect = new Rect(0, 0, Screen.width, Screen.height);
		}
		
		skin = Localizer.Skins["GameGUI"];
		l = Localizer.Content["levels"];
	}
	
	// Update is called once per frame
	void Update () {
	
	}
	
	void OnGUI() {
		GUI.depth = 5;
		if(background) {
			GUI.DrawTexture(backgroundRect, background, ScaleMode.ScaleAndCrop);
		}
		GUI.skin = skin;
		
		GUILayout.BeginHorizontal(GUILayout.Width(Screen.width));
    	GUILayout.FlexibleSpace();
        GUILayout.BeginVertical(GUILayout.Height(Screen.height));
    	GUILayout.FlexibleSpace();		
		
		
		switch(menuState) {
			case MenuStates.Grade:

		    	GUILayout.BeginVertical();
		    	
		    	GUILayout.Label(l["selectgrade"]);
		    	
		    	if(GUILayout.Button(l["grade1"])) {
		    		/*UserStatus.enableAdd = true;
		    		UserStatus.enableSubtract = true;
		    		UserStatus.enableMultiply = false;
		    		UserStatus.enableDivide = false;*/
		    		/* Hack: should be using above setters but they dont work wtf */
		    		UserStatus.enableFlags = 0x3;
		    		UserStatus.lowerOperandRange = 1;
		    		UserStatus.upperOperandRange = 10;
		    		
		    		menuState = MenuStates.Tests;
		    	}		    	
		    	if(GUILayout.Button(l["grade2"])) {
		    		/*UserStatus.enableAdd = true;
		    		UserStatus.enableSubtract = true;
		    		UserStatus.enableMultiply = false;
		    		UserStatus.enableDivide = false;*/
		    		/* Hack: should be using above setters but they dont work wtf */
		    		UserStatus.enableFlags = 0x3;
		    		UserStatus.lowerOperandRange = 1;
		    		UserStatus.upperOperandRange = 100;
		    		
		    		menuState = MenuStates.Tests;
		    	}
		    	if(GUILayout.Button(l["grade3"])) {
		    		UserStatus.enableAdd = true;
		    		UserStatus.enableSubtract = true;
		    		UserStatus.enableMultiply = true;
		    		UserStatus.enableDivide = true;    		
		    		UserStatus.lowerOperandRange = 1;
		    		UserStatus.upperOperandRange = 100;
		    		UserStatus.multiplicationUpperOperandRange = 10;
		    		UserStatus.divisionUpperOperandRange = 10;
		    		
		    		menuState = MenuStates.Tests;
		    	}		    	
		    	if(GUILayout.Button(l["grade4"])) {
		    		UserStatus.enableAdd = true;
		    		UserStatus.enableSubtract = true;
		    		UserStatus.enableMultiply = true;
		    		UserStatus.enableDivide = true;
		    		UserStatus.lowerOperandRange = 1;
		    		UserStatus.upperOperandRange = 100;
		    		UserStatus.multiplicationUpperOperandRange = 20;
		    		UserStatus.divisionUpperOperandRange = 20;
		    		
		    		menuState = MenuStates.Tests;
		    	}
		    	if(GUILayout.Button(l["grade5"])) {
		    		UserStatus.enableAdd = true;
		    		UserStatus.enableSubtract = true;
		    		UserStatus.enableMultiply = true;
		    		UserStatus.enableDivide = true;
		    		UserStatus.lowerOperandRange = 1;
		    		UserStatus.upperOperandRange = 1000;
		    		UserStatus.multiplicationUpperOperandRange = 100;
		    		UserStatus.divisionUpperOperandRange = 100;
		    		
		    		menuState = MenuStates.Tests;
		    	}
		    	if(GUILayout.Button(l["custom"])) {
    				Application.LoadLevel("CustomGameMenu");
    			}	    	
		    	GUILayout.EndVertical();
		    	break;
	    	
	    	case MenuStates.Tests:
		    	
		    	GUILayout.Label(l["pleaseselect"]);
		    	GUILayout.Space(20);
		    	
		    	GUILayout.BeginHorizontal();
		    	GUILayout.BeginVertical();
		    	
		    	GUILayout.Label(l["timed"]);
		    	
		    	if(GUILayout.Button(l["30seconds"])){		
		    		UserStatus.gameTime = 30;
		    		UserStatus.gameType = UserStatus.GameTypes.Timed;
		    				    				
		    		Application.LoadLevel("Game");
		    	}
		    	
		    	if(GUILayout.Button(l["1minute"])){		    		
		    		UserStatus.gameTime = 60;
		    		UserStatus.gameType = UserStatus.GameTypes.Timed;
		    		
		    		Application.LoadLevel("Game");
		    	}
		    	
		    	if(GUILayout.Button(l["2minute"])){
		    		UserStatus.gameTime = 120;
					UserStatus.gameType = UserStatus.GameTypes.Timed;
					
		    		Application.LoadLevel("Game");
		    	}
		    	
		    	if(GUILayout.Button(l["5minute"])){
		    		UserStatus.gameTime = 300;
					UserStatus.gameType = UserStatus.GameTypes.Timed;
					
		    		Application.LoadLevel("Game");    		
		    	}
		    	
		    	if(GUILayout.Button(l["10minute"])){
		    		UserStatus.gameTime = 600;
					UserStatus.gameType = UserStatus.GameTypes.Timed;
					
		    		Application.LoadLevel("Game");    		
		    	}    	
		    	GUILayout.EndVertical();
		    	
		    	GUILayout.Space(30);
		    	
		    	GUILayout.BeginVertical();		    	
		    	GUILayout.Label(l["numbered"]);
		    	
		    	if(GUILayout.Button(l["10Questions"])) {
		    		UserStatus.gameTime = 30;
		    		UserStatus.problems = 10;
		    		UserStatus.gameType = UserStatus.GameTypes.Numbered;
		    		
		    		Application.LoadLevel("Game");
		    	}
		    	
		    	if(GUILayout.Button(l["25Questions"])) {
		    		UserStatus.gameTime = 180;
		    		UserStatus.problems = 25;
		    		UserStatus.gameType = UserStatus.GameTypes.Numbered;
		    		
		    		Application.LoadLevel("Game");
		    	}
		    	
		    	if(GUILayout.Button(l["50Questions"])) {
		    		UserStatus.gameTime = 400;
		    		UserStatus.problems = 50;
		    		UserStatus.gameType = UserStatus.GameTypes.Numbered;
		    		
		    		Application.LoadLevel("Game");
		    	}
		    	GUILayout.EndVertical();
		    	GUILayout.EndHorizontal();
		    	break;
		}
	    	
    	
    	GUILayout.Space(40);
		
    	if(GUILayout.Button(Localizer.Content["game"]["backmenu"]))
      		Application.LoadLevel("MainMenu");
    	
    	GUILayout.Space(Screen.height / 10);
    	GUILayout.EndVertical();
    	GUILayout.FlexibleSpace();
    	GUILayout.EndHorizontal();
	}
}
