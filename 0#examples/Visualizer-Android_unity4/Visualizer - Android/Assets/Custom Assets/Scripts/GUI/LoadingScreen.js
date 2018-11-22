#pragma strict
#pragma implicit
#pragma downcast

enum DisplayType {
  Toshiba,
  Other
}

class LoadingDisplay {
  public var type : DisplayType;
  public var banner : Texture2D;
  public var tagline : String;
  public var copyright : String;
}

public var displayType : DisplayType;
public var displays : LoadingDisplay[];

private var tempSkin : GUISkin;

private var taglineStyle : GUIStyle;

private var subtitleStyle : GUIStyle;

private var currentDisplay : LoadingDisplay;

function Awake(){
  for(var display : LoadingDisplay in displays){
    if(display.type == displayType){
      currentDisplay = display;
      break;
    }
  }
}

function Start(){
  OnLanguageChanged();
}

function OnLanguageChanged(){
  tempSkin = Localizer.Skins["main"]; 

  taglineStyle = tempSkin.GetStyle("LoadingTagline");

  if(Localizer.appLanguage != Localizer.Language.en)
    subtitleStyle = tempSkin.GetStyle("loadingsubtitle");
}

function OnGUI(){
  GUI.color = Color.white;
  GUILayout.BeginVertical(GUILayout.Height(Screen.height));
  GUILayout.FlexibleSpace();

  GUILayout.BeginHorizontal(GUILayout.Width(Screen.width));
  GUILayout.FlexibleSpace();

  GUILayout.BeginHorizontal(GUILayout.Width(currentDisplay.banner.width));
  GUILayout.FlexibleSpace();
  GUILayout.BeginVertical();
  GUILayout.Label(currentDisplay.banner);

  GUILayout.EndVertical();
  GUILayout.FlexibleSpace();
  GUILayout.EndHorizontal();

  GUILayout.FlexibleSpace();
  GUILayout.EndHorizontal();

  GUILayout.Space(55);

  GUILayout.BeginHorizontal(GUILayout.Width(Screen.width));
  GUILayout.FlexibleSpace();
  GUILayout.Label(Localizer.Content["loading"]["tagline"], taglineStyle);
  GUILayout.FlexibleSpace();
  GUILayout.EndHorizontal();

  GUILayout.FlexibleSpace();
  
  GUILayout.BeginHorizontal();
  GUILayout.FlexibleSpace();
  GUILayout.Label(currentDisplay.copyright, taglineStyle);
  GUILayout.Space(30);
  GUILayout.EndHorizontal();
  GUILayout.Space(10);

  GUILayout.EndVertical();
}
