/**
 * This script generates the GUI for the visualization application. It also
 * implements a front-end for the ThinkGear component.
 */

import System;

var guiHeaderFont : Font;

var enableAdvancedMode : boolean = false;

class OverlayType {
  var texture : Texture2D;
  var color : Color;
}

var overlay : OverlayType;

private var BUILD_NUMBER : String = "v1.1";

private var languageIndex : int;
private var lastLanguageIndex : int = languageIndex;

private var controller : VisualizationController;

private var guiEnabled : boolean = false;

private var headerStyle : GUIStyle;

private var labelWidth : int = 70;

private var textBoxStyle : GUIStyle;

private var enablePortScan : boolean = true;

private var enableFullScreen : boolean = false;

private var showAdvancedMode: boolean = false;

private var axes : VisualizerAxes;

private var tempSkin : GUISkin;

private var sentByte : String = "";

private var langButtonStyle : GUIStyle;

private var meditationSound : MeditationSoundController;

private var framerateLimiter : FramerateLimiter;

private var debug : DebugOutput;

private var displayLanguageChangeWindow : boolean = false;

function Awake(){
  controller = GetComponent(VisualizationController);
  debug = GameObject.Find("ThinkGear").GetComponent(DebugOutput);
  
  // initialize values from the preferences
  axes = GetComponent(VisualizerAxes);

}

function Start(){
  OnLanguageChanged();

  framerateLimiter = GameObject.Find("Framerate Limiter").GetComponent(FramerateLimiter);
  
  guiEnabled = false;
  
  var meditationSoundController : GameObject = GameObject.Find("Meditation Sound Controller");

  if(meditationSoundController)
    meditationSound = meditationSoundController.GetComponent(MeditationSoundController);

  enableFullScreen = PlayerPrefs.GetInt("fullScreen", 0) != 0;

  InvokeRepeating("CheckFullScreen", 1.0, 1.0);
}

function OnLanguageChanged(){
  languageIndex = Localizer.appLanguage;
  lastLanguageIndex = languageIndex;

  tempSkin = Localizer.Skins["thinkgear"];
    
  headerStyle = tempSkin.GetStyle("Header");
  textBoxStyle = tempSkin.GetStyle("TextBox");

  langButtonStyle = Localizer.Skins["main"].GetStyle("langbutton");
}

function CheckFullScreen(){
  if(enableFullScreen != Screen.fullScreen){
    if(enableFullScreen){
      var r : Resolution = Screen.currentResolution;
      Screen.SetResolution(r.width, r.height, true);
    }
    else {
      Screen.SetResolution(1024, 640, false);
    }

    PlayerPrefs.SetInt("fullScreen", enableFullScreen ? 1 : 0);
  }
}

function Update () {
  
  if(Input.GetKeyDown("left"))
    controller.NextVisualization();
  else if(Input.GetKeyDown("right"))
    controller.PreviousVisualization();
  
  if(Input.GetKeyDown("escape")){
    guiEnabled = !guiEnabled;
  }

  if(enableAdvancedMode && Input.GetKeyDown(KeyCode.BackQuote))
    showAdvancedMode = !showAdvancedMode;

  UserStatus.isMenuDisplayed = guiEnabled;
  UserStatus.isAdvancedModeEnabled = showAdvancedMode;
}

function OnGUI(){
  GUI.depth = 1;

  GUI.skin = tempSkin;

  if(guiEnabled){
    if(overlay.texture){
      GUI.color = overlay.color;
      GUI.DrawTexture(Rect(0, 0, Screen.width, Screen.height), overlay.texture);
    }

    GUI.color = Color.white;

    GUILayout.BeginVertical(GUILayout.Height(Screen.height));
    GUILayout.FlexibleSpace();
    GUILayout.BeginHorizontal(GUILayout.Width(Screen.width));
    GUILayout.FlexibleSpace();

    GUILayout.BeginVertical(GUIStyle("Window"));
    MainWindow();
    GUILayout.EndVertical();

    GUILayout.FlexibleSpace();
    GUILayout.EndHorizontal();
    GUILayout.FlexibleSpace();
    GUILayout.EndVertical();

    if(lastLanguageIndex != languageIndex){
      lastLanguageIndex = languageIndex;
  
      var l : Localizer.Language = languageIndex;
      Localizer.SetLanguage(l);

      displayLanguageChangeWindow = true;
    }
  }

}

function MainWindow(){
  GUILayout.Label(Localizer.Content["thinkgear"]["optionshdr"], headerStyle);

  GUILayout.BeginHorizontal();
  GUILayout.BeginVertical(GUILayout.Width(labelWidth));
  GUILayout.Label(Localizer.Content["thinkgear"]["abouthdr"]);
  GUILayout.EndVertical();
  GUILayout.BeginVertical();
  GUILayout.Label(Localizer.Content["thinkgear"]["programname"] + " (" + BUILD_NUMBER + ")");
  GUILayout.EndVertical();
  GUILayout.EndHorizontal();

  if(showAdvancedMode){
    GUILayout.BeginHorizontal();
    GUILayout.BeginVertical(GUILayout.Width(labelWidth));
    GUILayout.Label(Localizer.Content["thinkgear"]["language"]);
    GUILayout.EndVertical();

    GUILayout.BeginVertical();
    languageIndex = GUILayout.SelectionGrid(languageIndex, 
                                            [Localizer.Content["thinkgear"]["langen"], 
                                             Localizer.Content["thinkgear"]["langjp"],
                                             Localizer.Content["thinkgear"]["langzh-cn"],
                                             Localizer.Content["thinkgear"]["langzh-tw"],
                                             Localizer.Content["thinkgear"]["langko"]], 
                                             3, langButtonStyle); 

    if(showAdvancedMode){
      if(GUILayout.Button(Localizer.Content["thinkgear"]["resetlang"]))
        Localizer.ResetLanguage();
    }

    GUILayout.EndVertical();

    GUILayout.EndHorizontal();
  }
  
  GUILayout.BeginHorizontal();
  GUILayout.BeginVertical(GUILayout.Width(labelWidth));
  GUILayout.Label(Localizer.Content["thinkgear"]["options"]);
  GUILayout.EndVertical();
  GUILayout.BeginVertical();
  
  debug.enableDebugOutput = GUILayout.Toggle(debug.enableDebugOutput, "  " + Localizer.Content["thinkgear"]["showdata"]);
    
  if(axes)
    axes.enabled = GUILayout.Toggle(axes.enabled, "  " + Localizer.Content["thinkgear"]["showaxes"]);

  if(showAdvancedMode && meditationSound)
    meditationSound.soundEnabled = GUILayout.Toggle(meditationSound.soundEnabled, "  " + Localizer.Content["thinkgear"]["emedsound"]);

  if(showAdvancedMode){
    GUILayout.BeginHorizontal();
    GUILayout.Label("Target ");

    framerateLimiter.frameRate = GUILayout.HorizontalSlider(framerateLimiter.frameRate, 5, 100, GUILayout.Width(55));
    GUILayout.Label(framerateLimiter.frameRate.ToString("##.00") + " fps");
    GUILayout.EndHorizontal();
    framerateLimiter.enableFrameRateDisplay = GUILayout.Toggle(framerateLimiter.enableFrameRateDisplay, "  " + Localizer.Content["thinkgear"]["enablefps"]);
  }

  // TODO: report rendertexture bug to Unity. this hack disables the full-screen mode toggle in game mode
  //if(axes)
    //enableFullScreen = GUILayout.Toggle(enableFullScreen, "  " + Localizer.Content["thinkgear"]["fullscreen"]);

  if(showAdvancedMode){
    if(GUILayout.Button(Localizer.Content["thinkgear"]["resetprefs"]))
      PlayerPrefs.DeleteAll();
  }

  GUILayout.EndVertical();
  GUILayout.EndHorizontal();
  
  GUILayout.BeginHorizontal(GUILayout.Height(50));
  GUILayout.EndHorizontal();
  
  GUILayout.Space(20);
  
  if(GUILayout.Button(Localizer.Content["thinkgear"]["quit"], GUILayout.Height(30))){
    Application.Quit();
  }
}

@script RequireComponent(VisualizationController)
