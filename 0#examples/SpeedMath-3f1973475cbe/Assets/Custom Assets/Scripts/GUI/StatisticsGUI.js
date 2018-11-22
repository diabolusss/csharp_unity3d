#pragma strict
#pragma implicit
#pragma downcast

var scoreFont : Font;
var labelFont : Font;

var buttonTextures : Texture2D[];

var controller : StatisticsController;
private var operationDisplay : OperationDisplay;
private var indicator : CorrectIndicator;
private var graph : GraphDisplay;

private var debugText : String;

private var eventIndex : int = 0;

var events : ProblemEvent[];

private var activeEvent : ProblemEvent;

private var scoreStyle : GUIStyle;
private var labelStyle : GUIStyle;
private var labelValueStyle : GUIStyle;

private var skin : GUISkin;

private var labels : String[];

private var buttonHeight : int = 35;

private var trainerDB : TrainerDatabase;

function Awake(){
  controller = gameObject.GetComponent.<StatisticsController>();
  operationDisplay = gameObject.GetComponent(OperationDisplay);
  indicator = gameObject.GetComponent(CorrectIndicator);
  graph = gameObject.GetComponent(GraphDisplay);

  skin = Localizer.Skins["GameGUI"];
  
  scoreStyle = new GUIStyle();
  scoreStyle.font = scoreFont;
  scoreStyle.normal.textColor = Color.white;
  scoreStyle.margin.bottom = 20;
  scoreStyle.alignment = TextAnchor.UpperRight;
  scoreStyle.fixedHeight = 60;

  labelStyle = skin.GetStyle("StatsLabel");
  
  labelValueStyle = new GUIStyle();
  labelValueStyle.font = labelFont;
  labelValueStyle.normal.textColor = Color.white;
  labelValueStyle.margin.bottom = 10;
  labelValueStyle.alignment = TextAnchor.UpperRight;
  labelValueStyle.fixedHeight = 30;

  trainerDB = GameObject.Find("DataLogger").GetComponent.<TrainerDatabase>();
}

function Start(){
  if(!controller.IsEmpty){
    ReloadDisplay();
    ReloadOperation();
    operationDisplay.disableDisplay = false;
    graph.drawGraph = true;
  }
  else {
    operationDisplay.disableDisplay = true;
  }

  labels = [Localizer.Content["game"]["correct"], Localizer.Content["game"]["incorrect"],
            Localizer.Content["game"]["avgatt"]];
}

function Update(){
  if(!controller.IsEmpty){
    if(Input.GetKeyDown("left")){
      UpdateEventIndex(eventIndex - 1);
    }
    else if(Input.GetKeyDown("right")){
      UpdateEventIndex(eventIndex + 1);
    }
    
    graph.currentEventIndex = eventIndex;
  }
}

public function UpdateEventIndex(newIndex){
    eventIndex = Mathf.Clamp(newIndex, 0, events.Length - 1);
    ReloadOperation();
}

function OnGUI(){
  /*
   * Draw the game navigation buttons
   */
  GUILayout.BeginVertical(GUILayout.Height(Screen.height));
  GUILayout.FlexibleSpace();

  if(controller.IsEmpty){
    GUILayout.BeginHorizontal(GUILayout.Width(Screen.width));
    GUILayout.FlexibleSpace();

    GUILayout.Label(Localizer.Content["game"]["nodata"], labelStyle);

    GUILayout.FlexibleSpace();
    GUILayout.EndHorizontal();
  }

  GUILayout.FlexibleSpace();

  GUILayout.BeginHorizontal(GUI.skin.GetStyle("box"), GUILayout.Width(Screen.width));

  if(GUILayout.Button(Localizer.Content["statsnav"]["mainmenu"], GUILayout.Height(buttonHeight))){
    Application.LoadLevel("MainMenu");
  }

  GUILayout.FlexibleSpace();

  if(!controller.IsEmpty){
    GUILayout.Label(Localizer.Content["statsnav"]["title"], GUILayout.Width(65));

    /*
    if(GUILayout.Button(GUIContent(Localizer.Content["statsnav"]["delete"], buttonTextures[4]), GUILayout.Height(buttonHeight))){
      controller.DeleteCurrentSession();
      ReloadDisplay();
      ReloadOperation();
    }
    */

    if(GUILayout.Button(GUIContent(Localizer.Content["statsnav"]["first"], buttonTextures[0]), GUILayout.Height(buttonHeight))){
      controller.LoadFirst();
      ReloadDisplay();
      ReloadOperation();
    }

    if(GUILayout.Button(GUIContent(Localizer.Content["statsnav"]["prev"], buttonTextures[1]), GUILayout.Height(buttonHeight))){
      controller.LoadPrevious();
      ReloadDisplay();
      ReloadOperation();
    }

    if(GUILayout.Button(GUIContent(Localizer.Content["statsnav"]["next"], buttonTextures[2]), GUILayout.Height(buttonHeight))){
      controller.LoadNext();
      ReloadDisplay();
      ReloadOperation();
    }

    if(GUILayout.Button(GUIContent(Localizer.Content["statsnav"]["last"], buttonTextures[3]), GUILayout.Height(buttonHeight))){
      controller.LoadMostRecent();
      ReloadDisplay();
      ReloadOperation();
    }
   
    GUILayout.Space(10);
    GUILayout.Label(trainerDB.sessionIDs.Count + Localizer.Content["statsnav"]["found"], GUILayout.Width(75));
  }

  GUILayout.FlexibleSpace();
  
  if(GUILayout.Button(Localizer.Content["statsnav"]["start"], GUILayout.Height(buttonHeight))){
    Application.LoadLevel("Game");
  }

  GUILayout.Space(10);

  GUILayout.EndHorizontal();

  GUILayout.EndVertical();
  
  /*
   * Draw the session statistics
   */
  if(!controller.IsEmpty){
    GUILayout.BeginArea(Rect(400, 60, 350, 550));
    
    GUILayout.BeginVertical();
    GUILayout.BeginHorizontal();
    GUILayout.BeginVertical(GUILayout.Width(155));
    GUILayout.Label(Localizer.Content["game"]["score"], labelStyle);
    GUILayout.EndVertical();
    GUILayout.BeginVertical(GUILayout.Width(190));
    GUILayout.Label(controller.score.ToString(), scoreStyle);
    GUILayout.EndVertical();
    GUILayout.EndHorizontal();
    
    GenerateTable(labels,
                  [controller.correct.ToString(), controller.incorrect.ToString(), controller.averageAttention < 0 ? "n/a" : controller.averageAttention.ToString()]);
    
    GUILayout.Space(20);
    
    GenerateTable([Localizer.Content["game"]["starttime"]], [controller.sessionStart.ToString("M/d/yy h:mm tt")]);
    
    GUILayout.EndVertical();
    GUILayout.EndArea();
  }
  else {
  }
}

function ReloadOperation(){
  activeEvent = events[eventIndex];
  operationDisplay.SetValues(activeEvent);
  
  if(activeEvent.answer != 0){
    indicator.SetMode(activeEvent.isCorrect ? CorrectIndicator.CORRECT : CorrectIndicator.INCORRECT);
  }
  else {
    indicator.SetMode(CorrectIndicator.HIDE);
  }
}

function ReloadDisplay(){
  events = controller.problemEvents;

  eventIndex = 0;
  activeEvent = events[eventIndex];
  
  graph.dataValues = controller.eSenseValues;
  graph.problemEvents = events;
}

function GenerateTable(labels : String[], values : String[]){
  for(var i = 0; i < labels.Length; i++){
    GUILayout.BeginHorizontal();
    GUILayout.BeginVertical(GUILayout.Width(155));
    GUILayout.Label(labels[i], labelStyle);
    GUILayout.EndVertical();
    GUILayout.BeginVertical(GUILayout.Width(190));
    GUILayout.Label(values[i].ToString(), labelValueStyle);
    GUILayout.EndVertical();
    GUILayout.EndHorizontal();
  }
}

@script RequireComponent(StatisticsController)
@script RequireComponent(OperationDisplay)
@script RequireComponent(CorrectIndicator)
@script RequireComponent(GraphDisplay)
