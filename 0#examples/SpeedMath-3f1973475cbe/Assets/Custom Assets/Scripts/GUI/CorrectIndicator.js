var correct : Texture2D;
var incorrect : Texture2D;
var position : Vector3;

static var HIDE : int = 0;
static var CORRECT : int = 1;
static var INCORRECT : int = 2;

private var mode : int = 0;

function OnGUI(){
  GUI.depth = -10;
  
  switch(mode){
    case HIDE:
      break;
    case CORRECT:
      GUI.DrawTexture(Rect(position.x, position.y, correct.width, correct.height), correct);
      break;
    case INCORRECT:
      GUI.DrawTexture(Rect(position.x, position.y, incorrect.width, incorrect.height), incorrect);
      break;
  }
}

function SetMode(mode : int){
  this.mode = mode;
}