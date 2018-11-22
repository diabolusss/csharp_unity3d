private var gameRunner : GameRunner;
private var currentState : GameRunner.GameStates = GameRunner.GameStates.Idle;
private var headerStyle : GUIStyle;
private var descriptionStyle : GUIStyle;
private var buttonStyle : GUIStyle;
private var labelStyle : GUIStyle;

private var controlStyle : GUIStyle;

private var skin : GUISkin;
private var menuSkin : GUISkin;

function Start(){
  skin = Localizer.Skins["GameGUI"];
  menuSkin = Localizer.Skins["nsskin"];

  gameRunner = GetComponent(GameRunner);

  headerStyle = skin.GetStyle("ScoreHeader");
  descriptionStyle = skin.GetStyle("ScoreValue");

  buttonStyle = menuSkin.GetStyle("Button");
  labelStyle = menuSkin.GetStyle("Label");
  
  buttonStyle.font = menuSkin.font;
  labelStyle.font = menuSkin.font;
}

function Update(){
  currentState = gameRunner.GetCurrentState();
}

function OnGUI(){
  GUI.skin = skin;
  GUI.color = Color.white;

  GUILayout.BeginVertical(GUILayout.Height(Screen.height));
  
  GUILayout.Space(45);
  GUILayout.BeginHorizontal();
  
  GUILayout.Space(25);
	if(UserStatus.gameType == UserStatus.GameTypes.Timed){
		GUILayout.Label(Localizer.Content["game"]["time"], headerStyle);
		GUILayout.Space(20);
		GUILayout.Label(gameRunner.GetGameRemainingTime().ToString("0.#"), descriptionStyle);
	}

  GUILayout.FlexibleSpace();

  GUILayout.EndHorizontal();

  GUILayout.FlexibleSpace();

  GUILayout.BeginHorizontal(GUILayout.Width(Screen.width));
  GUILayout.FlexibleSpace();

  // draw statuses only when the game is in a not-running state
  if(currentState != GameRunner.GameStates.Running && currentState != GameRunner.GameStates.Paused){
    GUILayout.BeginVertical("Box", GUILayout.Width(200));

    switch(currentState){
      case GameRunner.GameStates.Paused:
        break;
      case GameRunner.GameStates.HeadsetOff:
        GUILayout.Label(Localizer.Content["game"]["noheadset"], labelStyle);

        break;
      case GameRunner.GameStates.Countdown:
        GUILayout.Label(Localizer.Content["game"]["startingin"] + " " + Mathf.CeilToInt(3.0 - gameRunner.GetCountdownTime()), labelStyle);
        break;
      case GameRunner.GameStates.Success:
        GUILayout.Label(Localizer.Content["game"]["finished"], labelStyle);

        GUILayout.Space(30);

        if(GUILayout.Button(Localizer.Content["game"]["continue"], buttonStyle)){
          Invoke("LoadStatistics", 1.0);
        }

        break;
      case GameRunner.GameStates.Idle:
        GUILayout.Label(Localizer.Content["game"]["startgame"], labelStyle);

        GUILayout.Space(30);

        if(GUILayout.Button(Localizer.Content["game"]["start"], buttonStyle))
          gameRunner.TriggerReset();

        break;
      default:
        break;
    }

    GUILayout.EndVertical();
  }

  GUILayout.FlexibleSpace();
  GUILayout.EndHorizontal();

  GUILayout.FlexibleSpace();
  GUILayout.EndVertical();
}

function LoadStatistics(){
  Application.LoadLevel("Statistics");
}

@script RequireComponent(GameRunner)
