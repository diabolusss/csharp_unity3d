#pragma strict
#pragma implicit
#pragma downcast

import System.Globalization;
import System.Threading;

var problemEvents : ProblemEvent[];

var debugProblems : String = "";
var debugAnswers : String = "";
var debugESense : String = "";

var sessionStart : System.DateTime;
var sessionFinish : System.DateTime;
var sessionLength : float;

var sessionID : int;

var score : int;
var correct : int;
var incorrect : int;
var numProblems : int;
var averageAttention : int;

var eSenseValues : float[];

private var trainerDB : TrainerDatabase;

private var session : TrainerSession;

// keep track of the offset of the current session ID from the most recent one
private var dataIndexOffset : int = 0;

private var problemTimes : float[];
private var operand1s : int[];
private var operand2s : int[];
private var operators : int[];

private var answers : int[];
private var answerTimes : float[];

private var eSenseTimes : float[];

private var offset : int = 0;

private var debugText : String;

public var IsEmpty : boolean = false;

function Awake(){
  // load the most recent one
  trainerDB = GameObject.Find("DataLogger").GetComponent.<TrainerDatabase>();

  IsEmpty = trainerDB.sessionIDs.Count == 0;

  if(!IsEmpty)
    LoadData(trainerDB.MostRecentSessionID());
}

function Start(){
}

function IsMostRecentSession(){
	return sessionID == trainerDB.MostRecentSessionID();
}

function IsComplete(){
	return sessionFinish != 0;
}

function DeleteCurrentSession(){
  trainerDB.DeleteSession(sessionID);
  LoadWithOffset(offset);
}

function LoadPrevious(){
  offset = Mathf.Clamp(offset + 1, 0.0, trainerDB.sessionIDs.Count - 1);
  
  LoadWithOffset(offset);
}

function LoadNext(){
  offset = Mathf.Clamp(offset - 1, 0.0, Mathf.Infinity);
  
  LoadWithOffset(offset);
}

function LoadMostRecent(){
  offset = 0;
  LoadData(trainerDB.MostRecentSessionID());
}

function LoadWithOffset(offset : int){
  sessionID = trainerDB.sessionIDs[trainerDB.sessionIDs.Count - 1 - offset];
    
  LoadData(sessionID);
}

function LoadFirst(){
  offset = trainerDB.sessionIDs.Count - 1;
  
  LoadWithOffset(offset);
}

function LoadData(id : int){
  Debug.Log("Loaded " + id);
  debugProblems = "";
  debugAnswers = "";
  debugESense = "";
  
  session = trainerDB.GetSession(id);
  
  sessionID = id;
  
  correct = session.numCorrect;
  incorrect = session.numIncorrect;
  numProblems = session.numProblems;
  
  sessionStart = System.DateTime.FromFileTime(session.startedAt);
  sessionFinish = System.DateTime.FromFileTime(session.endedAt);
  sessionLength = Mathf.Round(sessionFinish.Subtract(sessionStart).TotalSeconds);
  Debug.Log("SessionLength: " + sessionLength.ToString());
  problemEvents = session.problemEvents;

  // set up the eSense value array
  eSenseValues = new float[session.thinkGearEvents.Length];
  
  var cumulativeAttention : float = 0.0;
  
  for(var i = 0; i < session.thinkGearEvents.Length; i++){
    eSenseValues[i] = session.thinkGearEvents[i].attention;
    cumulativeAttention += eSenseValues[i];
  }
  
  averageAttention = session.thinkGearEvents.Length > 0 ? cumulativeAttention / session.thinkGearEvents.Length : 100;
  
  // calculate the score. the score is biased towards the number of problems correct,
  // but is influenced by the average attention
  score = (correct * correct * (100 - (averageAttention / 2))) / 50;
}
