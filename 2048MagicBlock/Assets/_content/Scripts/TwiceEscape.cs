using UnityEngine;

public class TwiceEscape
{
    private float timer = -2;
    private const float exitDelay = 0.3f;

    public void Update()
    {
        if (timer > -1)
            timer += Time.deltaTime;

        if(timer > exitDelay)
        {
            timer = -2;
        }

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (timer < -1)
                timer = 0;
            else
            {
#if UNITY_EDITOR
                UnityEditor.EditorApplication.isPlaying = false;
#else
         Application.Quit();
#endif
            }
        }
    }
}

