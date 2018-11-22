var position : Rect;
var dataValues : float[];
var lineMaterial : Material;
var background : Texture2D;

var cursor : Texture2D;
var dragCursor : Texture2D;

var eventIndicator : Texture2D;

var problemEvents : ProblemEvent[];
var currentEventIndex : int = 0;

var maxValue : float = 100.0;
var minValue : float = 0.0;

var correctIndicatorColor : Color;
var problemIndicatorColor : Color;
var incorrectIndicatorColor : Color;
var hoverIndicatorColor : Color;
var dragIndicatorColor : Color;

var drawGraph : boolean = false;

private var stats : StatisticsGUI;

private var testTime: float = 30.0;

private var lineObject : GameObject;
private var line : LineRenderer;

private var colorIntensityReduction : Color = Color(1, 1, 1, 0.2);

private var problemLineAreas : Rect[];
private var problemHoverAreas : Rect[];

private var isDragging : boolean = false;

function Start(){
    lineObject = GameObject("Raw Line");
    
    line = lineObject.AddComponent(LineRenderer);
    
    line.material = lineMaterial;
    line.SetWidth(0.07, 0.07);
    line.SetColors(Color.white, Color.white);
    
    stats = GetComponent.<StatisticsGUI>();
}

function Update () {
  var increment : float = (position.width + 0.0) / (dataValues.Length - 1.0);
  //Debug.Log("Session length: " + stats.controller.sessionLength.ToString());
  testTime = stats.controller.sessionLength;
  line.SetVertexCount(dataValues.Length);
  
  for(var i = 0; i < dataValues.Length; i++){
    var vertex : Vector3 = Camera.main.ScreenToWorldPoint(Vector3(position.x + 0.0 + (increment * i), (Screen.height - position.y) - position.height + (dataValues[i] / maxValue * position.height), 5));
    line.SetPosition(i, vertex);
  }
  
  if(problemEvents.length > 0 && (problemLineAreas == null || problemEvents.length != problemLineAreas.length)){
    problemLineAreas = new Rect[problemEvents.length];
    problemHoverAreas = new Rect[problemEvents.length];

    for(var j : int = 0; j < problemEvents.length; j++){
      var problemEvent : ProblemEvent = problemEvents[j];

      problemLineAreas[j] = new Rect(position.x + (problemEvent.eventTime / testTime * position.width), position.y, 1, position.height);
      problemHoverAreas[j] = new Rect(position.x + (problemEvent.eventTime / testTime * position.width) - 2, position.y + position.height - 2, 4, 10);
    }
  }
}

function OnGUI(){
  if(drawGraph){
    var cursorPosition : Rect = Rect(position.x + (problemEvents[currentEventIndex].eventTime / testTime * position.width) - (cursor.width / 2) + 1, 
                                     position.y + position.height, 
                                     cursor.width, 
                                     cursor.height);

    // track dragging events from the graph area
    if(Event.current.type == EventType.MouseDown){
      isDragging = cursorPosition.Contains(Event.current.mousePosition) ||
                   position.Contains(Event.current.mousePosition);
    }
    else if(isDragging && Event.current.type == EventType.MouseUp){
      isDragging = false;
    }

    // if the user is moused over the graph area, draw the vertical hover indicator line. if the user
    // is dragging over the graph area, highlight the vertical hover indicator line.
    if(position.Contains(Event.current.mousePosition)){
        highlightLineArea = new Rect(Event.current.mousePosition.x, position.y, 1, position.height);

        GUI.color = isDragging ? dragIndicatorColor : hoverIndicatorColor;
        GUI.DrawTexture(highlightLineArea, eventIndicator);
    }

    GUI.color = Color.white;

    if(isDragging){
      // draw a shadow cursor and update its position
      var shadowCursorPosition : Rect = Rect(Event.current.mousePosition.x - (cursor.width / 2), 
                                     position.y + position.height, 
                                     cursor.width, 
                                     cursor.height);

      GUI.DrawTexture(shadowCursorPosition, dragCursor);

      for(var j : int = 0; j < problemHoverAreas.length; j++){
        if(problemHoverAreas[j].Contains(new Vector2(shadowCursorPosition.x + (cursor.width / 2), shadowCursorPosition.y)))
          stats.UpdateEventIndex(j);
      }
    }
    
    // draw the cursor
    GUI.DrawTexture(cursorPosition, cursor);
    GUI.Label(Rect(cursorPosition.x - 10, cursorPosition.y + 14, 46, 30), problemEvents[currentEventIndex].eventTime.ToString("0.00") + "s");

    // note the events
    for(var i : int = 0; i < problemEvents.length; i++){
      var problemEvent : ProblemEvent = problemEvents[i];

      if(problemEvent.answer == 0)
        GUI.color = problemIndicatorColor;
      else if(problemEvent.isCorrect)
        GUI.color = correctIndicatorColor;
      else
        GUI.color = incorrectIndicatorColor;
      
      GUI.color = GUI.color * colorIntensityReduction;

      /*
      if(problemHoverAreas[i].Contains(Event.current.mousePosition))
        stats.UpdateEventIndex(i);
       */

      GUI.DrawTexture(problemLineAreas[i], eventIndicator);
    }
    
    if(background != null){
      GUI.color = Color(1, 1, 1, 0.05);
      GUI.DrawTexture(position, background);
    }
  }
}
