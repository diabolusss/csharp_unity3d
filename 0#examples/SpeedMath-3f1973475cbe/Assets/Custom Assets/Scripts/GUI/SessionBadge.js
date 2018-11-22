var badgeBackground : Texture2D;
var position : Vector3;
var badgeFont : Font;

private var controller : StatisticsController;

private var badgeRect : Rect;
private var textRect : Rect;
private var sessionRect : Rect;
private var sessionDropRect : Rect;

private var textStyle : GUIStyle;
private var sessionStyle : GUIStyle;

private var skin : GUISkin;

function Awake(){
  controller = gameObject.GetComponent(StatisticsController);
  badgeRect = Rect(position.x, position.y, badgeBackground.width, badgeBackground.height);
  textRect = Rect(position.x, position.y - 12, badgeBackground.width, badgeBackground.height);
  sessionRect = Rect(position.x, position.y + 25, badgeBackground.width, badgeBackground.height);
  sessionDropRect = Rect(position.x, position.y + 26, badgeBackground.width, badgeBackground.height);
  
  textStyle = new GUIStyle();
  textStyle.font = badgeFont;
  textStyle.normal.textColor = Color.white;
  textStyle.alignment = TextAnchor.LowerCenter;
}

function Start(){
  OnLanguageChanged();

  sessionStyle = skin.GetStyle("Label");
  sessionStyle.alignment = TextAnchor.UpperCenter;
}

function OnLanguageChanged(){
  skin = Localizer.Skins["nsskin"];  
}

function OnGUI(){
  GUI.skin = skin;

  if(!controller.IsEmpty){
    GUI.DrawTexture(badgeRect, badgeBackground);
    GUI.Label(textRect, controller.sessionID.ToString(), textStyle);

    GUI.color = Color.black;
    GUI.Label(sessionDropRect, Localizer.Content["game"]["session"]);

    GUI.color = Color.white;
    GUI.Label(sessionRect, Localizer.Content["game"]["session"]);
  }
}
