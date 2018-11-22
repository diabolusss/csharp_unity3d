using UnityEngine;
using System.Collections;

public class ModeSwitchForVisualizer : MonoBehaviour
{
    private bool startDetectTouch = false;

    void Start()
    {
        Invoke("detectTouch", 2.0f);
    }

    void detectTouch()
    {
        startDetectTouch = true;
    }

    // Update is called once per frame
    void Update()
    {

        if (startDetectTouch && Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Moved)
        {
            if (Input.GetTouch(0).deltaPosition.x < -10)
            {
                Application.LoadLevel("Game");
                //Application.LoadLevelAsync("Game");
            }
            if (Input.GetTouch(0).deltaPosition.x > 10)
            {
                Application.LoadLevel("Display");
                //Application.LoadLevelAsync("Display");
            }
        }

    }
}
