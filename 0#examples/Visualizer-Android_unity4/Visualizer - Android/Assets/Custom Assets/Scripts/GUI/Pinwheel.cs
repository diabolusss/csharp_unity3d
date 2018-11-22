using UnityEngine;
using System.Collections;

public class Pinwheel : MonoBehaviour {

  public Texture2D currentFrame;
  public Texture2D[] frames;
  public float frameRate = 1.0f;
  
  private float elapsedSinceLastFrame = 0.0f;
  private float framePeriod;
  private int currentFrameIndex = 0;

	// Use this for initialization
	void Awake () {
    currentFrame = frames[currentFrameIndex];	
    framePeriod = 1.0f / frameRate;
	}
	
	// Update is called once per frame
	void Update () {
    elapsedSinceLastFrame += Time.deltaTime;	

    if(elapsedSinceLastFrame >= framePeriod){
      elapsedSinceLastFrame -= framePeriod;

      currentFrameIndex = (currentFrameIndex + 1) % frames.Length; 

      currentFrame = frames[currentFrameIndex];
    }
	}
}
