#pragma strict
#pragma implicit
#pragma downcast

var enableMove : boolean = true;
var sprite : Texture;
var isFinishedMoving : boolean = false;
var centerPosition : Vector3;

private var waypoints : Vector3[];
private var vertices : Vector3[];

private var waypointIndex : int;
private var distanceFromLastWaypoint : Vector3;

private var targetRotateAngle : float;
private var currentRotateAngle : float;

private var currentSpeed : float = 0.0;

function SetWaypoints(waypoints : Vector3[]){
  this.waypoints = waypoints;
  
  vertices = new Vector3[waypoints.Length - 1];
  
  for(var i = 1; i < waypoints.Length; i++){
    vertices[i - 1] = waypoints[i] - waypoints[i - 1];
  }
}

function EnableMovement(enable : boolean){
  enableMove = enable;
}

function Reset(){
  waypointIndex = 0;
  distanceFromLastWaypoint = Vector3.zero;
  isFinishedMoving = false;
}

function Update(){
  currentRotateAngle = Mathf.Lerp(currentRotateAngle, targetRotateAngle, Time.deltaTime * 3);
  
  if(isFinishedMoving)
    currentSpeed = 0;
}

function MoveAlongWaypoints(speed : float){
  if(isFinishedMoving || !enableMove)
    return;
    
  currentSpeed = speed;
    
  var normalizedTravelVector : Vector3 = vertices[waypointIndex].normalized;
  var dS : Vector3 = normalizedTravelVector * Mathf.Sqrt(speed);
  
  distanceFromLastWaypoint = distanceFromLastWaypoint + dS;
  
  if(distanceFromLastWaypoint.magnitude > vertices[waypointIndex].magnitude){
    if(++waypointIndex > (vertices.Length - 1)){
      isFinishedMoving = true;
      waypointIndex = vertices.Length - 1;
      return;
    }
    else {
      targetRotateAngle = Vector3.Angle(-Vector3.up, vertices[waypointIndex]);
    }
    
    distanceFromLastWaypoint = Vector3.zero;
  }
}

function OnGUI(){
  centerPosition = waypoints[waypointIndex] + distanceFromLastWaypoint;
  
  // now offset by the sprite center
  var tempMatrix = GUI.matrix;
  
  GUIUtility.RotateAroundPivot(currentRotateAngle, centerPosition);
  
  GUI.DrawTexture(Rect(centerPosition.x - (sprite.width / 2), 
                       centerPosition.y - (sprite.height / 2), 
                       sprite.width, sprite.height), sprite);
  
  GUI.matrix = tempMatrix;
}