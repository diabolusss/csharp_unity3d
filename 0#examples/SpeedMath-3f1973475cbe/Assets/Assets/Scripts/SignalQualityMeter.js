#pragma strict
#pragma implicit

var signalTextures : Texture[];

var reconnectInterval : float = 10.0;

private var thinkGearData : DataController;

private var signalTextureIndex : int = 5;

private var portName : String = "";

private var enableDemoMode : boolean = false;

private var elementHeight : int = 25;

private var initialPortName : String = "";

private var configByte : String = "02";

private var guiSkin : GUISkin;
private var buttonStyle : GUIStyle;

private var boxStyle : GUIStyle;
private var disconnectedStyle : GUIStyle;

enum States {
  Disconnected,
  Scanning,
  Connected,
  Reconnecting
}

enum ReconnectStates {
  Idle,
  Waiting,
  Scanning
}

private var state : States = States.Disconnected;
private var reconnectState : ReconnectStates = ReconnectStates.Waiting;

function Awake(){
  DontDestroyOnLoad(this);
  
  thinkGearData = GameObject.Find("ThinkGear").GetComponent.<DataController>();
  
  InvokeRepeating("UpdateMeter", 0, 1.0);
}

function Start(){
  OnLanguageChanged();

  if(thinkGearData.IsHeadsetInitialized)
    state = States.Connected;

  GameHelper.SendMessageToAll("OnRequestPortScan", "", SendMessageOptions.DontRequireReceiver);

  guiSkin = Localizer.Skins["nsskin"];

  boxStyle = guiSkin.FindStyle("SignalBox");
  disconnectedStyle = guiSkin.FindStyle("SignalError");

  buttonStyle = guiSkin.FindStyle("Language");
}

function Update(){
  if(reconnectState == ReconnectStates.Idle){
    reconnectState = ReconnectStates.Waiting;
    Invoke("ReconnectScan", reconnectInterval);
  }
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

  GUI.depth = 1; 

  GUILayout.BeginHorizontal(GUILayout.Width(Screen.width));
  GUILayout.FlexibleSpace();

  GUILayout.BeginHorizontal(state == States.Disconnected ? disconnectedStyle : boxStyle);

  switch(state){
    case States.Disconnected:
      if(UserStatus.isAdvancedModeEnabled){
        GUILayout.BeginVertical(GUILayout.Height(elementHeight));
        GUILayout.Space(6);
        GUILayout.BeginHorizontal();
        GUILayout.Label("Initial port");
        GUILayout.Space(5);
        
        if(Application.platform == RuntimePlatform.WindowsPlayer){ 
          GUILayout.Label("COM");
          initialPortName = GUILayout.TextField(initialPortName, 3, GUILayout.Width(25));
        }
        else {
          initialPortName = GUILayout.TextField(initialPortName, GUILayout.Width(180));
        }

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
        // add the "\\.\COM" prefix to the user-specified starting COM port number
        var formattedPortName = Application.platform == RuntimePlatform.WindowsPlayer ? "\\\\.\\COM" + initialPortName : initialPortName;

        GameHelper.SendMessageToAll("OnRequestPortScan", enableDemoMode ? "DemoMode" : formattedPortName, SendMessageOptions.DontRequireReceiver);
      }

      GUILayout.EndVertical();

      break;
    case States.Scanning:
      GUILayout.BeginVertical(GUILayout.Height(elementHeight));
      GUILayout.FlexibleSpace();
      GUILayout.Label(Localizer.Content["connectionpanel"]["scanning"] + " " + portName + "...");
      GUILayout.EndVertical();

      GUILayout.BeginVertical(GUILayout.Height(elementHeight));
      GUILayout.FlexibleSpace();

      if(GUILayout.Button(Localizer.Content["connectionpanel"]["skip"], GUILayout.Width(120))){
        GameHelper.SendMessageToAll("OnRequestPortQueryCancel", null, SendMessageOptions.DontRequireReceiver);
      }

      GUILayout.EndVertical();

      break;
    case States.Connected:
      if(!thinkGearData.IsDemo){
        if(UserStatus.isAdvancedModeEnabled){
          GUILayout.BeginVertical(GUILayout.Height(elementHeight));
          GUILayout.Space(6);
          GUILayout.BeginHorizontal();
          GUILayout.Label("Config Byte");
          GUILayout.Space(5);
          GUILayout.Label("0x");
          configByte = GUILayout.TextField(configByte, 3, GUILayout.Width(25));

          if(GUILayout.Button("Send")){
            GameHelper.SendMessageToAll("OnTransmitByte", System.Byte.Parse(configByte, System.Globalization.NumberStyles.AllowHexSpecifier), SendMessageOptions.DontRequireReceiver);
          }

          GUILayout.Space(15);
          GUILayout.EndHorizontal();
          GUILayout.EndVertical();
        }

        GUILayout.BeginVertical(GUILayout.Height(elementHeight));
        GUILayout.FlexibleSpace();
        GUILayout.Label(Localizer.Content["connectionpanel"]["wavequality"]);
        GUILayout.EndVertical();
        GUILayout.BeginVertical();
        GUILayout.Space(-5);
        GUILayout.Label(signalTextures[signalTextureIndex]);
        GUILayout.Space(-10);
        GUILayout.EndVertical();
      }

/*
 * Disabled the Disconnect button
 */
       

      GUILayout.BeginVertical(GUILayout.Height(elementHeight));
      GUILayout.FlexibleSpace();

      if(GUILayout.Button(Localizer.Content["connectionpanel"]["disconnect"], GUILayout.Width(120)))
        GameHelper.SendMessageToAll("OnRequestHeadsetDisconnect", 
                                    ConnectionController.DisconnectStatuses.DisconnectRequested, 
                                    SendMessageOptions.DontRequireReceiver);

      GUILayout.EndVertical();

      break;
    case States.Reconnecting:
      GUILayout.BeginVertical(GUILayout.Height(elementHeight));
      GUILayout.FlexibleSpace();
      
      switch(reconnectState){
        case ReconnectStates.Idle:
          GUILayout.Label(Localizer.Content["connectionpanel"]["reconnecting"] + "...");
          break;
        case ReconnectStates.Waiting:
          GUILayout.Label(Localizer.Content["connectionpanel"]["reconnecting"] + "...");
          break;
        case ReconnectStates.Scanning:
          GUILayout.Label(Localizer.Content["connectionpanel"]["retry"] + " " + portName + "...");
          break;
      }

      GUILayout.EndVertical();

      GUILayout.BeginVertical(GUILayout.Height(elementHeight));
      GUILayout.FlexibleSpace();

      if(GUILayout.Button(Localizer.Content["connectionpanel"]["stop"], GUILayout.Width(120))){
        switch(reconnectState){
          case ReconnectStates.Waiting:
            break;
          case ReconnectStates.Scanning:
            GameHelper.SendMessageToAll("OnRequestPortScanTerminate", null, SendMessageOptions.DontRequireReceiver);
            break;
        }

        CancelInvoke("ReconnectScan");
        state = States.Disconnected;
      }

      GUILayout.EndVertical();

      break;
  }

  GUILayout.EndHorizontal();
  GUILayout.Space(20);
  GUILayout.EndHorizontal();
}

function OnLanguageChanged(){
  guiSkin = Localizer.Skins["nsskin"];
}

function OnPortScanStarted(){
  if(state == States.Reconnecting){

  }
  else {
    state = States.Scanning;
  }
}

function OnPortScanFailed(){
  if(state == States.Reconnecting){
    reconnectState = ReconnectStates.Idle;
  }
  else {
    state = States.Disconnected;
  }
}

function OnPortQueryStarted(queryPortName : String){
  portName = queryPortName;
}

function OnHeadsetConnected(portName : String){
  state = States.Connected;
}

function ReconnectScan(){
  var formattedPortName = Application.platform == RuntimePlatform.WindowsPlayer ? 
                            "\\\\.\\COM" + initialPortName : 
                            initialPortName;

  GameHelper.SendMessageToAll("OnRequestPortScan", formattedPortName, SendMessageOptions.DontRequireReceiver);

  reconnectState = ReconnectStates.Scanning;
}

function OnHeadsetDisconnected(d : ConnectionController.DisconnectStatuses){
  if(d == ConnectionController.DisconnectStatuses.TimedOut){
    state = States.Reconnecting;
    reconnectState = ReconnectStates.Idle;
  }
  else
    state = States.Disconnected;
}

