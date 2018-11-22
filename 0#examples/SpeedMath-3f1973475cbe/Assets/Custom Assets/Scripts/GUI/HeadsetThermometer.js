/**
 * Do the thermometer-type headset force strength
 */
#pragma strict
#pragma implicit
#pragma downcast

var background : Texture2D;
var position : Rect;

private var thinkGear : DataController;

private var meterValue : float = 0.0;

private var meterWidth : float;
private var lerpedMeterWidth : float;
private var meterVelocity : float;

private var backgroundRect : Rect;
private var labelRect : Rect;

private var labelStyle : GUIStyle;

function Awake(){
  
  labelRect = Rect(position.x, position.y, 75, position.height);
  backgroundRect = Rect(position.x + 75, position.y, position.width - 75, position.height);
}

function Start(){
  labelStyle = Localizer.Skins["GameGUI"].GetStyle("ScoreHeader");
  thinkGear = GameObject.Find("ThinkGear").GetComponent.<DataController>();
  InvokeRepeating("UpdateMeter", 0.0, 1.0);
}

function Update(){
  lerpedMeterWidth = Mathf.SmoothDamp(lerpedMeterWidth, meterWidth, meterVelocity, 1.0);
}

function UpdateMeter(){
  meterValue = thinkGear.headsetData.attention;
  meterWidth = (meterValue / 100.0) * backgroundRect.width;
}

function OnGUI(){
  GUI.color = Color.white;
  GUI.Label(labelRect, Localizer.Content["game"]["attention"], labelStyle);
  
  GUI.color = Color(1, 1, 1, 0.1);
  GUI.DrawTexture(backgroundRect, background);
  
  GUI.color = Color.white;
  GUI.DrawTexture(Rect(backgroundRect.x, backgroundRect.y, lerpedMeterWidth, backgroundRect.height), background);
}

function OnApplicationQuit(){
  CancelInvoke("UpdateMeter");
}
