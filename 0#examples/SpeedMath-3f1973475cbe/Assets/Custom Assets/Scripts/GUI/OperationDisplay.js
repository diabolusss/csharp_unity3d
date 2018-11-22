#pragma strict
#pragma implicit
#pragma downcast

var numericFont : Font;

var operators : Texture2D[];

var answerBackground : Texture2D;

var operand1 : int;
var operand2 : int;
var operator : ProblemEvent.Operands;
var answer : String = "";

var position : Vector3 = Vector3.zero;

var disableDisplay : boolean = true;
var enableCursor : boolean = false;

private var numberStyle : GUIStyle;

private var operand1Rect : Rect;
private var operand2Rect : Rect;
private var operatorRect : Rect;
private var operatorRect2 : Rect;
private var answerRect : Rect;
private var answerBackgroundRect : Rect;
private var cursorRect : Rect;

private var cursorBlink: boolean = false;

function Awake(){
  numberStyle = GUIStyle();
  numberStyle.font = numericFont;
  numberStyle.normal.textColor = Color.white;
  numberStyle.margin.bottom = 8;
  numberStyle.wordWrap = true;
  numberStyle.alignment = TextAnchor.UpperRight;
  
  operand1Rect = Rect(position.x + 50, position.y, 250, 150);
  operand2Rect = Rect(position.x + 50, position.y + 140, 250, 150);
  operatorRect = Rect(position.x, position.y + 150, 128, 128);
  operatorRect2 = Rect(position.x - 100, position.y + 150, 128, 128);
  
  answerRect = Rect(position.x + 2, position.y + 300, 300, 150);
  
  answerBackgroundRect = Rect(position.x + 15, position.y + 290, 300, 150);

  cursorRect = Rect(position.x + 305, position.y + 300, 3, 130);
}

function Start(){
  if(enableCursor)
    InvokeRepeating("CursorBlink", 1.0, 0.5);
}

function CursorBlink(){
  cursorBlink = !cursorBlink;
}

function SetValues(op1 : int, op2 : int, op : ProblemEvent.Operands, answer : String){
  operand1 = op1;
  operand2 = op2;
  operator = op;
  this.answer = answer;
}

function SetValues(operation : ProblemEvent){
  operand1 = operation.operand_1;
  operand2 = operation.operand_2;
  operator = operation.op;
  answer = operation.answer.ToString();
}

function OnGUI(){
  if(!disableDisplay){
    GUI.color = Color(1, 1, 1, 0.05);
    
    // draw the background behind the answer
    GUI.DrawTexture(answerBackgroundRect, answerBackground);

    GUI.color = Color(1, 1, 1, 0.8);

    if(cursorBlink)
      GUI.DrawTexture(cursorRect, answerBackground);

    GUI.color = Color.white;

    // draw the operands
    if(operand1 != 0 && operand2 != 0){
      GUI.Label(operand1Rect, operand1.ToString(), numberStyle);
      GUI.Label(operand2Rect, operand2.ToString(), numberStyle);
      
      if(operand2 > 99) 
      	GUI.DrawTexture(operatorRect2, operators[operator]);
	  else
      // draw the operator
      	GUI.DrawTexture(operatorRect, operators[operator]);
    }

    // draw the entered answer only if it's nonzero. assume 0 = no input
    if(answer != "0" && answer.Length != 0)
      GUI.Label(answerRect, answer, numberStyle);
  }
}
