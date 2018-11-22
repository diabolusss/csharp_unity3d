using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class OptionsInterface : MonoBehaviour {

	public Texture2D add;
	public Texture2D subtract;
	public Texture2D multiply;
	public Texture2D divide;
	
	private GUISkin skin;
	private Dictionary<string, string> l;
	private GUIStyle labelStyle;
	
	private const int LABEL_WIDTH = 70;
	private const int FIELD_WIDTH = 200;
	private const int EXTRA_WIDTH = 30;
	private const int WIDE_WIDTH = 150;
	
	private bool enableSounds;
	
	private int lowerValue;
	private int upperValue;
	
	private GUIContent addContent;
	private GUIContent subtractContent;
	private GUIContent multiplyContent;
	private GUIContent divideContent;

	void Awake(){
		skin = Localizer.Skins["GameGUI"];
		l = Localizer.Content["options"];
		labelStyle = skin.GetStyle("StatsLabel");
		
		enableSounds = UserStatus.isSoundEnabled;
	}

	// Use this for initialization
	void Start () {		
		addContent = new GUIContent(l["add"], add);
		subtractContent = new GUIContent(l["subtract"], subtract);
		multiplyContent = new GUIContent(l["multiply"], multiply);
		divideContent = new GUIContent(l["divide"], divide);
		
		lowerValue = UserStatus.lowerOperandRange;
		upperValue = UserStatus.upperOperandRange;
	}

	void Update(){
		if(UserStatus.isSoundEnabled != enableSounds) {
			UserStatus.isSoundEnabled = enableSounds;
		}
	}
	
  void OnGUI(){
    GUILayout.BeginHorizontal(GUILayout.Width(Screen.width));
    GUILayout.FlexibleSpace();

    GUILayout.BeginVertical(GUILayout.Height(Screen.height));
    GUILayout.FlexibleSpace();
	
	GUILayout.Label(l["gameoptions"], labelStyle);
    
    GUILayout.Space(5);

    enableSounds = GUILayout.Toggle(enableSounds, l["enablesounds"]);

    GUILayout.Space(100);

    if(GUILayout.Button(l["mainmenu"], skin.GetStyle("button")))
      Application.LoadLevel("MainMenu");

    GUILayout.FlexibleSpace();
    GUILayout.EndVertical();

    GUILayout.FlexibleSpace();
    GUILayout.EndHorizontal();
  }
}
