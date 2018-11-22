using UnityEngine;
using System.Collections;

[AddComponentMenu("Camera-Control/Mouse Orbit")]
public class MouseOrbit : MonoBehaviour {

  public Transform target;
  public float distance = 10.0f;

  public float xSpeed = 250.0f;
  public float ySpeed = 120.0f;
  
  public int yMinLimit = -20;
  public int yMaxLimit = 80;

  private float x = 0.0f;
  private float y = 0.0f;

  private int yPos;
  private float yVelocity;
  private float smoothedYPosition;

	// Use this for initialization
	void Start () {
    Vector3 angles = transform.eulerAngles;
    x = angles.y;
    y = angles.x;

    if(rigidbody)
      rigidbody.freezeRotation = true;

    if(target)
      ReorientCamera(true);
	}
	
	// Update is called once per frame
	void LateUpdate() {
    int targetYPos = target ? (int)(target.position.y * 100) : 0;	

    // only rotate if the right mouse button is clicked or if there was a y displacement
    if((target && Input.GetMouseButton(1)) || (target && (yPos != targetYPos)))
      // if the mouse button is not held down, just reposition the camera vertically without
      // interpreting mouse input
      ReorientCamera(Input.GetMouseButton(1));
    
    yPos = target ? (int)(smoothedYPosition * 100) : 0;	
  }

  void ReorientCamera(bool receiveMouseInput){
    if(receiveMouseInput){
      x += Input.GetAxis("Mouse X") * xSpeed * 0.02f;
      y -= Input.GetAxis("Mouse Y") * ySpeed * 0.02f;
    }

    y = ClampAngle(y, yMinLimit, yMaxLimit);

    Quaternion rotation = Quaternion.Euler(y, x, 0);
    Vector3 position = rotation * new Vector3(0.0f, 0.0f, -distance) + target.position;

    smoothedYPosition = Mathf.SmoothDamp(transform.position.y, position.y, ref yVelocity, 0.35f);

    transform.rotation = rotation;

    transform.position = new Vector3(position.x,
                                     receiveMouseInput ? position.y : smoothedYPosition,
                                     position.z);
  }

  private static float ClampAngle(float angle, float min, float max){
    if(angle < -360)
      angle += 360;
    if(angle > 360)
      angle -= 360;

    return Mathf.Clamp(angle, min, max);
  }
}

