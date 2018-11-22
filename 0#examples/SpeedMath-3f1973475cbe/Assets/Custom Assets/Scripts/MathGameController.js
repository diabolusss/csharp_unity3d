#pragma strict
#pragma implicit
#pragma downcast

import System;
import System.Globalization;

// operators
enum MathStates {
  Unsolved,
  SolvedCorrect,
  SolvedIncorrect,
  WaitingForProblem
}

var wrongSound : AudioClip;
var correctSound : AudioClip;

private var gameRunner : GameRunner;

private var gameState : int;
private var mathState : MathStates;

private var thinkGear : DataController;

private var normalizedAttention : float;

private var contactDistance : float;

private var numberStyle : GUIStyle;

private var operand1 : int;
private var operand2 : int;
private var operator : ProblemEvent.Operands;
private var enteredAnswer : String = "";
private var desiredAnswer : String = "";

private var trainerDB : TrainerDatabase;

private var sessionID : int;

private var operationDisplay : OperationDisplay;

private var session : TrainerSession;

private var eSenseValues : Array;
private var problemEvents : Array;

function Awake(){
  mathState = MathStates.Unsolved;
  gameObject.AddComponent(AudioSource);
}

function Start(){
  thinkGear = GameObject.Find("ThinkGear").GetComponent.<DataController>();
  trainerDB = GameObject.Find("DataLogger").GetComponent(TrainerDatabase);
  
  gameRunner = gameObject.GetComponent(GameRunner);
  operationDisplay = gameObject.GetComponent(OperationDisplay);
}

function Update () {
  normalizedAttention = Mathf.Clamp(thinkGear.headsetData.attention - 30, 0, 70) / 70.0;
  normalizedAttention = normalizedAttention * normalizedAttention;
  
  // handle numeric input
  for(var i = 0; i <= 9; i++){
    if(Input.GetKeyDown(i.ToString()) || Input.GetKeyDown("[" + i.ToString() + "]"))
      AddDigit(i);
  }
  
  // handle backspaces
  if(Input.GetKeyDown("backspace"))
    RemoveDigit();
    
  switch(gameRunner.GetCurrentState()){
    case GameRunner.GameStates.Countdown:
      // set up the level if there was a state transition
      if(gameState != GameRunner.GameStates.Countdown){
      }
      
      operationDisplay.disableDisplay = true;
      
      break;
    case GameRunner.GameStates.Running:
      // do something once the game starts running
      if(gameState != GameRunner.GameStates.Running){
        session = new TrainerSession(trainerDB.MostRecentSessionID() + 1, DateTime.Now.ToFileTime());
        //Debug.Log("training session");
        // initialize arrays to store the various values
        eSenseValues = new Array();
        problemEvents = new Array();
        
        operationDisplay.disableDisplay = false;
        GenerateNewProblem();
        
        InvokeRepeating("LogHeadsetValue", 1.0, 1.0);
      }

      break;
      
    case GameRunner.GameStates.Success:
      if(gameState != GameRunner.GameStates.Success){
        CancelInvoke("LogHeadsetValue");
        
        // set the finish time
        session.endedAt = DateTime.Now.ToFileTime();
        
        // now convert the Array() values into built-in array types
        var eSenseValuesBuiltin : ThinkGearEvent[] = eSenseValues.ToBuiltin(ThinkGearEvent);
        var problemEventsBuiltin : ProblemEvent[] = problemEvents.ToBuiltin(ProblemEvent);
        
        // write to the data store
        session.problemEvents = problemEventsBuiltin;
        session.thinkGearEvents = eSenseValuesBuiltin;
        //Debug.Log("training session3");
        trainerDB.WriteSession(session);
      }
      
      operationDisplay.disableDisplay = true;
      
      break;
    default:
      operationDisplay.disableDisplay = true;
      break;
  }
  
  switch(mathState){
    case MathStates.Unsolved:
      if(enteredAnswer == desiredAnswer && gameState == GameRunner.GameStates.Running){
        mathState = MathStates.SolvedCorrect;
        break;
      }

      if((enteredAnswer.Length > desiredAnswer.Length) ||
         (enteredAnswer.Length == desiredAnswer.length && enteredAnswer != desiredAnswer)){
        mathState = MathStates.SolvedIncorrect;
      }
    
      break;
      
    case MathStates.SolvedCorrect:
    	if(UserStatus.isSoundEnabled) {
      		audio.clip = correctSound;
      		audio.Play();
    	}
      session.numCorrect++;
      Debug.Log("Problems: " + session.numProblems + " of " + UserStatus.problems);
      if(UserStatus.gameType == UserStatus.GameTypes.Numbered && session.numProblems > UserStatus.problems) { 
			problemEvents.Add(new ProblemEvent(operand1, operand2, operator, System.Convert.ToInt32(enteredAnswer), gameRunner.GetGameTime()));
			mathState = MathStates.WaitingForProblem;
      } else {
      	  problemEvents.Add(new ProblemEvent(operand1, operand2, operator, System.Convert.ToInt32(enteredAnswer), gameRunner.GetGameTime()));
	      Invoke("SwitchToUnsolved", 0.1);
	      mathState = MathStates.WaitingForProblem;
      	
      }
      break;
      
    case MathStates.SolvedIncorrect:
    if(UserStatus.isSoundEnabled) {
      audio.clip = wrongSound;
      audio.Play();
    }
      session.numIncorrect++;
      problemEvents.Add(new ProblemEvent(operand1, operand2, operator, System.Convert.ToInt32(enteredAnswer), gameRunner.GetGameTime()));
      enteredAnswer = "";
      mathState = MathStates.Unsolved;
      break;
      
    case MathStates.WaitingForProblem:
      break;
  }
  
  gameState = gameRunner.GetCurrentState();
  
  //operationDisplay.SetValues(operand1, operand2, operator, enteredAnswer);
  
  	if(UserStatus.gameType == UserStatus.GameTypes.Numbered && session.numProblems > UserStatus.problems) {  
    	GameHelper.SendMessageToAll("OnAchievementStopRequested", null, SendMessageOptions.DontRequireReceiver);
        gameRunner.currentState = GameRunner.GameStates.Success;
    } else {
    	operationDisplay.SetValues(operand1, operand2, operator, enteredAnswer);
    }
        
        
}


function OnApplicationQuit(){
  CancelInvoke("LogHeadsetValue");
  CancelInvoke("SwitchToUnsolved");
}


function SwitchToUnsolved(){
  enteredAnswer = "";
  GenerateNewProblem();
  mathState = MathStates.Unsolved;
}

function LogHeadsetValue(){
  eSenseValues.Add(new ThinkGearEvent(0, thinkGear.headsetData.attention, gameRunner.GetGameTime()));
}

function GenerateNewProblem(){
	operator = GenerateOperator();
	operand1 = GenerateOperand();
	operand2 = GenerateOperand();
  
  
  // now compute the answer
  switch(operator){
    case ProblemEvent.Operands.Subtract:
      while(operand1 == operand2){
        operand1 = GenerateOperand();
        operand2 = GenerateOperand();
      }
        
      if(operand2 > operand1){
        var temp = operand1;
        operand1 = operand2;
        operand2 = temp;
      }

      desiredAnswer = (operand1 - operand2).ToString();
      break;
    case ProblemEvent.Operands.Add:
      desiredAnswer = (operand1 + operand2).ToString();
      break;
    case ProblemEvent.Operands.Multiply:
      desiredAnswer = (operand1 * operand2).ToString();
      break;
    case ProblemEvent.Operands.Divide:
      
      var answer : int = UnityEngine.Random.Range(1, 11);
      desiredAnswer = answer.ToString();
      operand1 = operand2 * answer;
      
      break;
  }

  session.numProblems++;
  problemEvents.Add(new ProblemEvent(operand1, operand2, operator, gameRunner.GetGameTime()));
}

function GenerateOperator() : ProblemEvent.Operands {
  while(true){
    var op : ProblemEvent.Operands = UnityEngine.Random.Range(0, 4);

    switch(op){
      case ProblemEvent.Operands.Add:
        if(!UserStatus.enableAdd)
          continue;
        else
          return op;

        break;
      case ProblemEvent.Operands.Subtract:
        if(!UserStatus.enableSubtract)
          continue;
        else
          return op;

        break;
      case ProblemEvent.Operands.Multiply:
        if(!UserStatus.enableMultiply)
          continue;
        else
          return op;

        break;
      case ProblemEvent.Operands.Divide:
        if(!UserStatus.enableDivide)
          continue;
        else
          return op;

        break;
      default:
        return ProblemEvent.Operands.Subtract;
    }
  }
}

function GenerateOperand() : int {
	if(operator == ProblemEvent.Operands.Multiply || operator == ProblemEvent.Operands.Divide)
		return UnityEngine.Random.Range(UserStatus.lowerOperandRange, UserStatus.multiplicationUpperOperandRange + 1);
	else
		return UnityEngine.Random.Range(UserStatus.lowerOperandRange, UserStatus.upperOperandRange + 1);
}

function RemoveDigit(){
  var enteredAnswerNumber : int = System.Convert.ToInt32(enteredAnswer);
  enteredAnswerNumber = enteredAnswerNumber / 10;
  enteredAnswer = enteredAnswerNumber == 0 ? "" : enteredAnswerNumber.ToString();
}

// add the entered number into the least significant digit of the existing
// number. chop off the most significant digit if the number gets too long
function AddDigit(i : int){
  var tempAnswer : String = enteredAnswer;

  tempAnswer = tempAnswer.Insert(tempAnswer.Length, i.ToString());
  
  enteredAnswer = tempAnswer.Length > 4 ? tempAnswer.Remove(0, 1) : tempAnswer;
}

@script RequireComponent(GameRunner)
@script RequireComponent(OperationDisplay)
