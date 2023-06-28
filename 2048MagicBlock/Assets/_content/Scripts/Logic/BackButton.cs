using UnityEngine;

public class BackButton : MonoBehaviour
{
    [SerializeField] private PausePanel _pauseController;

    public void Update()
    {
        if (Application.platform == RuntimePlatform.Android)
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                MainMenu();
            }
        }
    }

    public void MainMenu()
    {
        _pauseController.Home();
    }
}
