using UnityEngine;
using System.Collections;

public class GameRunner : MonoBehaviour {

	public enum GameStates {
		Invalid,
		Idle,
		HeadsetOff,
		Countdown,
		Running,
		Success,
		Paused
	}

	public const int MAX_LIVES = 3;
	// countdown before game starts
	private const float COUNTDOWN_TIME = 3.0f;
	
	private float SOLVE_TIME;
	private int problems;
	
	private volatile bool isHeadsetConnected = false;
	
	// set up initial game parameters
	private float timer = 0.0f;
	//private int currentState = IDLE;
	public GameStates currentState = GameStates.Idle;
	
	private GameStates beforePausedState = GameStates.Invalid;

	void Start(){
		ConnectionController c = GameObject.Find("ThinkGear").GetComponent<ConnectionController>();
		isHeadsetConnected = c.Connected;
		SOLVE_TIME = UserStatus.gameTime;
	}

	public GameStates GetCurrentState(){
	  return currentState;
	}
	
	public float GetCountdownTime(){
	  return currentState == GameStates.Countdown ? timer : 0.0f;
	}
	
	public float GetGameRemainingTime(){
	  return (currentState == GameStates.Running || currentState == GameStates.Paused || currentState == GameStates.Success) ? timer : 0.0f;
	}
	
	public float GetGameTime(){
	  return SOLVE_TIME - GetGameRemainingTime();
	}

	void OnHeadsetConnected(string portName){
		isHeadsetConnected = true;
	}
	
	void OnHeadsetDisconnected(ConnectionController.DisconnectStatuses d){
		isHeadsetConnected = false; 
	}
	
	// Update is called once per frame
	void Update () {
	  switch(currentState){
      case GameStates.HeadsetOff:
        if(isHeadsetConnected)
          currentState = GameStates.Countdown;

        break;
	    case GameStates.Countdown:
	      timer += Time.deltaTime;
	      
	      if(timer >= COUNTDOWN_TIME){
	        currentState = GameStates.Running;
          GameHelper.SendMessageToAll("OnAchievementStartRequested", null, SendMessageOptions.DontRequireReceiver);
	        timer = SOLVE_TIME;
	      }
	      
	      break;
	    case GameStates.Running:
	      timer -= Time.deltaTime;
	      
	      if(timer <= 0 && UserStatus.gameType == UserStatus.GameTypes.Timed){
          	GameHelper.SendMessageToAll("OnAchievementStopRequested", null, SendMessageOptions.DontRequireReceiver);
	        currentState = GameStates.Success;
	        timer = 0;
	      }
	      
	      break;
	    case GameStates.Success:
	    case GameStates.Idle:
	    default:
	      break;
	  }
	}
	
	public void TogglePause(){
	  if(beforePausedState != GameStates.Invalid){
	    currentState = beforePausedState;
	    beforePausedState = GameStates.Invalid;
	  }
	  else {
	    beforePausedState = currentState;
	    currentState = GameStates.Paused;
	  }
	}
	
	public void TriggerReset(){
	  TriggerCountdown();
	}
	
	public void TriggerCountdown(){
	  timer = 0.0f;	  
	  currentState = GameStates.HeadsetOff;
	}
}
