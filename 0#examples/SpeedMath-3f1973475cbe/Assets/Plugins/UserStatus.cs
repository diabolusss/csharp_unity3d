using UnityEngine;
using System.Collections;

public class UserStatus : MonoBehaviour {
	public enum GameTypes {
		Timed,
		Numbered	
	}
	
	public static bool isHelpEnabled = false;
	public static bool isMenuDisplayed = false;
	public static bool isMouseIdle = false;
	public static bool hasBannerDisplayed = false;
	public static string dataPath = "";
	public static bool isDemoMode = false;
	public static bool isAdvancedModeEnabled = false;
	public static bool isSoundEnabled = true;
	// allowable operands
	public static int lowerOperandRange;
	public static int upperOperandRange;
	public static int multiplicationUpperOperandRange;
	public static int divisionUpperOperandRange;
	
	public static int enableFlags;
	
	public static float gameTime = 30.0f;
	public static int problems = 99;
	public static GameTypes gameType;
	
	private const int ADD_OFFSET = 0;
	private const int SUBTRACT_OFFSET = 1;
	private const int MULTIPLY_OFFSET = 2;
	private const int DIVIDE_OFFSET = 3;
	
	private const int LOWER_RANGE = 1;
	private const int UPPER_RANGE = 13;
	
	private const int BIT_MASK = 0x1;

  /**
   * Enabling the arithmetic operations is done via a bit-mask of an integer
   * value, where every bit is a flag that indicates whether the arithmetic 
   * operator is enabled or disabled.
   */

  public static bool enableAdd {
    get { return ((enableFlags >> ADD_OFFSET) & BIT_MASK) == 1; }
    set { 
      if(value) 
        enableFlags |= (BIT_MASK << ADD_OFFSET);
      else
        enableFlags &= (~BIT_MASK << ADD_OFFSET);
    }
  }

  public static bool enableSubtract {
    get { return ((enableFlags >> SUBTRACT_OFFSET) & BIT_MASK) == 1; }
    set {
      if(value) 
        enableFlags |= (BIT_MASK << SUBTRACT_OFFSET);
      else
        enableFlags &= (~BIT_MASK << SUBTRACT_OFFSET);
    }
  }

  public static bool enableDivide {
    get { return ((enableFlags >> DIVIDE_OFFSET) & BIT_MASK) == 1; }
    set {
      if(value) 
        enableFlags |= (BIT_MASK << DIVIDE_OFFSET);
      else
        enableFlags &= (~BIT_MASK << DIVIDE_OFFSET);
    }
  }

  public static bool enableMultiply {
    get { return ((enableFlags >> MULTIPLY_OFFSET) & BIT_MASK) == 1; }
    set {
      if(value) 
        enableFlags |= (BIT_MASK << MULTIPLY_OFFSET);
      else
        enableFlags &= (~BIT_MASK << MULTIPLY_OFFSET);
    }
  }

  /**
   * Load player preferences from the registry/plist when the game starts up.
   */
  void Awake(){
    lowerOperandRange = PlayerPrefs.GetInt("lowerOperand", LOWER_RANGE);
    upperOperandRange = PlayerPrefs.GetInt("upperOperand", UPPER_RANGE);
    multiplicationUpperOperandRange = PlayerPrefs.GetInt("multiplicationUpperOperand", UPPER_RANGE);
    divisionUpperOperandRange = PlayerPrefs.GetInt("divisionUpperOperand", UPPER_RANGE);
    enableFlags = PlayerPrefs.GetInt("enableFlags", 15);
	gameTime = PlayerPrefs.GetFloat("gameTime", 30);
	problems = PlayerPrefs.GetInt("problems", 99);
	if(PlayerPrefs.GetInt("enableSound", 1) == 1) {
		isSoundEnabled = true;
	}
	else
		isSoundEnabled = false;
	
    // now do range checks on these values
    lowerOperandRange = (int)Mathf.Clamp((float)lowerOperandRange, (float)LOWER_RANGE, (float)UPPER_RANGE);
    upperOperandRange = (int)Mathf.Clamp((float)upperOperandRange, (float)LOWER_RANGE, (float)UPPER_RANGE);
	multiplicationUpperOperandRange = (int)Mathf.Clamp((float)multiplicationUpperOperandRange, (float)LOWER_RANGE, (float)UPPER_RANGE);
	divisionUpperOperandRange = (int)Mathf.Clamp((float)divisionUpperOperandRange, (float)LOWER_RANGE, (float)UPPER_RANGE);

    enableFlags = (int)Mathf.Clamp((float)enableFlags, 1, 15);
  }

  /**
   * Save player preferences into the registry/plist when the game quits.
   */
  void OnApplicationQuit(){
    PlayerPrefs.SetInt("lowerOperand", lowerOperandRange);
    PlayerPrefs.SetInt("upperOperand", upperOperandRange);
    PlayerPrefs.SetInt("multiplicationUpperOperand", multiplicationUpperOperandRange);
    PlayerPrefs.SetInt("divisionUpperOperand", divisionUpperOperandRange);
    PlayerPrefs.SetInt("enableFlags", enableFlags);
    PlayerPrefs.SetFloat("gameTime", gameTime);
    PlayerPrefs.SetInt("problems", problems);
    if(isSoundEnabled) {
    	PlayerPrefs.SetInt("enableSound", 1);
    }
    else {
    	PlayerPrefs.SetInt("enableSound", 0);	
    }
  }
}
