#pragma strict
#pragma implicit
#pragma downcast

var signalTextures : Texture[];
var rescanInterval : float = 5.0;

private var thinkGearData : DataController;

private var signalTextureIndex : int = 5;

private var portName : String = "";

private var enableDemoMode : boolean = false;

private var elementHeight : int = 25;

private var initialPortName : String = "";

private var configByte : String = "02";

private var guiSkin : GUISkin;
private var buttonStyle : GUIStyle;

enum State {
  Disconnected,
  Scanning,
  Connected,
  DisconnectRequested,
  ReconnectCountdown,
  Reconnecting
}

private var state : State = State.Disconnected;

function Awake(){
  DontDestroyOnLoad(this);
  
  thinkGearData = GameObject.Find("ThinkGear").GetComponent(DataController);
  
  InvokeRepeating("UpdateMeter", 0, 1.0);
}

function Start(){
  OnLanguageChanged();

  if(thinkGearData.IsHeadsetInitialized)
    state = State.Connected;

       //GameHelper.SendMessageToAll("OnRequestConnect", enableDemoMode, SendMessageOptions.DontRequireReceiver);
         GameHelper.SendMessageToAll("OnRequestConnect", enableDemoMode, SendMessageOptions.DontRequireReceiver);

  guiSkin = Localizer.Skins["thinkgear"];
  buttonStyle = Localizer.Skins["main"].GetStyle("langbutton");
}

function UpdateMeter(){
  // if headset is uninitialized or off-head, display zero signal quality
  if(!thinkGearData.IsHeadsetInitialized || thinkGearData.IsOffHead)
    signalTextureIndex = 0;
  // otherwise, figure out what the poor signal value is and display
  // the appropriate signal quality icon
  else {
    var signalQuality : float = thinkGearData.headsetData.poorSignalValue;

    // no poor signal flags have been raised
    if(signalQuality < 25)
      signalTextureIndex = 5;
    // one flag has been raised
    else if(signalQuality >= 25 && signalQuality < 51)
      signalTextureIndex = 4;
    // two flags have been raised
    else if(signalQuality >= 51 && signalQuality < 78)
      signalTextureIndex = 3;
    // three flags have been raised
    else if(signalQuality >= 78 && signalQuality < 107)
      signalTextureIndex = 2;
    // four flags have been raised
    else if(signalQuality >= 107)
      signalTextureIndex = 1;
  }
}

function OnGUI(){
  GUI.skin = guiSkin;

  GUI.depth = 5; 

  GUILayout.BeginHorizontal(GUILayout.Width(Screen.width));
  GUILayout.FlexibleSpace();

  switch(state){
    case State.Disconnected:
      if(UserStatus.isAdvancedModeEnabled){
        GUILayout.BeginVertical(GUILayout.Height(elementHeight));
        GUILayout.Space(6);
        GUILayout.BeginHorizontal();

        GUILayout.Space(5);

        GUILayout.EndHorizontal();
        GUILayout.EndVertical();
      }

      GUILayout.BeginVertical(GUILayout.Height(elementHeight));
      GUILayout.FlexibleSpace();
      enableDemoMode = GUILayout.Toggle(enableDemoMode, "  " + Localizer.Content["connectionpanel"]["edemomode"]);
      GUILayout.Space(3);
      GUILayout.EndVertical();

      GUILayout.BeginVertical(GUILayout.Height(elementHeight));
      GUILayout.FlexibleSpace();

      if(GUILayout.Button(Localizer.Content["connectionpanel"]["connect"], GUILayout.Width(120))){
       GameHelper.SendMessageToAll("OnRequestConnect", enableDemoMode, SendMessageOptions.DontRequireReceiver);
      }

      GUILayout.EndVertical();

      break;
    case State.Connected:
      if(!thinkGearData.IsDemo){
        GUILayout.BeginVertical(GUILayout.Height(elementHeight));
        GUILayout.FlexibleSpace();
        GUILayout.Label(Localizer.Content["connectionpanel"]["wavequality"]);
        GUILayout.EndVertical();
        GUILayout.BeginVertical();
        GUILayout.Space(-4);
        GUILayout.Label(signalTextures[signalTextureIndex]);
        GUILayout.EndVertical();
      }

      GUILayout.BeginVertical(GUILayout.Height(elementHeight));
      GUILayout.FlexibleSpace();

      if(GUILayout.Button(Localizer.Content["connectionpanel"]["disconnect"], GUILayout.Width(120))){
        state = State.DisconnectRequested;
        GameHelper.SendMessageToAll("OnRequestDisconnect", null, SendMessageOptions.DontRequireReceiver);
      }

      GUILayout.EndVertical();

      break;
  }

  GUILayout.Space(20);
  GUILayout.EndHorizontal();

function OnLanguageChanged(){
  guiSkin = Localizer.Skins["thinkgear"];
}

function OnHeadsetConnected(){
  state = State.Connected;
}

function OnHeadsetDisconnected(){
      state = State.Disconnected;
}

