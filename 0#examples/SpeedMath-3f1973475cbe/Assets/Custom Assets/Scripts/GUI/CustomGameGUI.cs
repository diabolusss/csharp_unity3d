using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CustomGameGUI : MonoBehaviour {

	public Texture2D background;
	private Rect backgroundRect;
	
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
	
	private bool enableAdd;
	private bool enableSubtract;
	private bool enableMultiply;
	private bool enableDivide;
	
	private int lowerValue;
	private int upperValue;
	
	private float gameTime;
	
	private GUIContent addContent;
	private GUIContent subtractContent;
	private GUIContent multiplyContent;
	private GUIContent divideContent;

  void Awake(){
    skin = Localizer.Skins["GameGUI"];
    l = Localizer.Content["options"];
    labelStyle = skin.GetStyle("StatsLabel");
  }

	// Use this for initialization
	void Start () {
		if(background) {
			backgroundRect = new Rect(0, 0, Screen.width, Screen.height);
		}
		enableAdd = UserStatus.enableAdd;
		enableSubtract = UserStatus.enableSubtract;
		enableMultiply = UserStatus.enableMultiply;
		enableDivide = UserStatus.enableDivide;
		
		gameTime = UserStatus.gameTime;
		
		addContent = new GUIContent(l["add"], add);
		subtractContent = new GUIContent(l["subtract"], subtract);
		multiplyContent = new GUIContent(l["multiply"], multiply);
		divideContent = new GUIContent(l["divide"], divide);
		
		lowerValue = UserStatus.lowerOperandRange;
		upperValue = UserStatus.upperOperandRange;
	}

  void Update(){
    if(UserStatus.enableAdd != enableAdd)
      UserStatus.enableAdd = enableAdd;

    if(UserStatus.enableSubtract != enableSubtract)
      UserStatus.enableSubtract = enableSubtract;

    if(UserStatus.enableMultiply != enableMultiply)
      UserStatus.enableMultiply = enableMultiply;

    if(UserStatus.enableDivide != enableDivide)
      UserStatus.enableDivide = enableDivide;

    if(lowerValue > 12)
      lowerValue = 12;

    if(upperValue <= lowerValue)
      upperValue = lowerValue + 1;

    if(UserStatus.lowerOperandRange != lowerValue)
      UserStatus.lowerOperandRange = lowerValue;

    if(UserStatus.upperOperandRange != upperValue)
      UserStatus.upperOperandRange = upperValue;
	if(UserStatus.gameTime != gameTime)
		UserStatus.gameTime = gameTime;     
  }
	
  void OnGUI(){
  	GUI.depth = 5;
	if(background) {
		GUI.DrawTexture(backgroundRect, background, ScaleMode.ScaleAndCrop);
	}
    GUILayout.BeginHorizontal(GUILayout.Width(Screen.width));
    GUILayout.FlexibleSpace();

    GUILayout.BeginVertical(GUILayout.Height(Screen.height));
    GUILayout.FlexibleSpace();

    GUILayout.Label(l["numheading"], labelStyle);

    GUILayout.BeginHorizontal();
    GUILayout.Label(l["min"] + ": ", GUILayout.Width(LABEL_WIDTH));
    lowerValue = (int)GUILayout.HorizontalSlider(lowerValue, 1.0f, 12.0f, GUILayout.Width(FIELD_WIDTH));
    GUILayout.Space(5);
    GUILayout.Label(lowerValue.ToString(), GUILayout.Width(EXTRA_WIDTH));
    GUILayout.EndHorizontal();

    GUILayout.BeginHorizontal();
    GUILayout.Label(l["max"] + ": ", GUILayout.Width(LABEL_WIDTH));
    upperValue = (int)GUILayout.HorizontalSlider(upperValue, 1.0f, 1000.0f, GUILayout.Width(FIELD_WIDTH));
    GUILayout.Space(5);
    GUILayout.Label(upperValue.ToString(), GUILayout.Width(EXTRA_WIDTH));
    GUILayout.EndHorizontal();

    GUILayout.Space(20);

    GUILayout.Label(l["opsheading"], labelStyle);

    GUILayout.BeginHorizontal();
    enableAdd = GUILayout.Toggle(enableAdd, addContent, GUILayout.Width(WIDE_WIDTH), GUILayout.Height(25));
    enableSubtract = GUILayout.Toggle(enableSubtract, subtractContent, GUILayout.Width(WIDE_WIDTH), GUILayout.Height(25));
    GUILayout.EndHorizontal();

    GUILayout.BeginHorizontal();
    enableMultiply = GUILayout.Toggle(enableMultiply, multiplyContent, GUILayout.Width(WIDE_WIDTH), GUILayout.Height(25));
    enableDivide = GUILayout.Toggle(enableDivide, divideContent, GUILayout.Width(WIDE_WIDTH), GUILayout.Height(25));
    GUILayout.EndHorizontal();
	
	GUILayout.Space(20);
	GUILayout.BeginHorizontal();
	GUILayout.Label(l["gameduration"]);
	gameTime = Mathf.Floor(GUILayout.HorizontalSlider(gameTime, 1.0f, 600.0f, GUILayout.Width(FIELD_WIDTH)));
	GUILayout.Label(gameTime.ToString() + "s", GUILayout.Width(EXTRA_WIDTH));
	//GUILayout.FlexibleSpace();
	GUILayout.EndHorizontal();
	
    GUILayout.Space(50);

    if(GUILayout.Button(Localizer.Content["game"]["start"], skin.GetStyle("button")))
      Application.LoadLevel("Game");
	
	GUILayout.Space(20);
	if(GUILayout.Button(l["back"], skin.GetStyle("button")))
      Application.LoadLevel("LevelSelect");
      
    GUILayout.FlexibleSpace();
    GUILayout.EndVertical();

    GUILayout.FlexibleSpace();
    GUILayout.EndHorizontal();
  }
}

