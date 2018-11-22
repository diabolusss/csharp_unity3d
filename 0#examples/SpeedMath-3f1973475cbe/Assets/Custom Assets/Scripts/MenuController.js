#pragma strict //force static typing
#pragma implicit //allow implicit variable declaration
#pragma downcast

var background : Texture2D;
var banner : Texture2D;

private var buttonSize : Vector2 = Vector2(150, 30);

private var backgroundRect : Rect;
function Awake(){

}
function Start(){
  // scale the background rectangle to always occupy the entire screen
  if(background)
    backgroundRect = Rect(0, 0, Screen.width, Screen.height);

}

function OnGUI(){
  GUI.depth = 5;
  
  if(background)
    GUI.DrawTexture(backgroundRect, background, ScaleMode.ScaleAndCrop);
  
  if(banner){
    GUILayout.Space(Screen.height / 5);
    GUILayout.BeginHorizontal(GUILayout.Width(Screen.width));
    GUILayout.FlexibleSpace();
    GUILayout.Label(banner);
    GUILayout.FlexibleSpace();
    GUILayout.EndHorizontal();
  }
}
