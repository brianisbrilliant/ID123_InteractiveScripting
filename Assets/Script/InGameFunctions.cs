using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class InGameFunctions : MonoBehaviour
{
    Keyboard keyboard;

    private void Awake()
    {
        keyboard = Keyboard.current;
    }

    void Update()
    {
        if (keyboard == null) return;
        if(keyboard.numpad7Key.wasPressedThisFrame)
        {
            if(Time.timeScale == 1)
            {
                Time.timeScale = 0;
            }
            else
            {
                Time.timeScale = 1;
            }
        }

        if (keyboard.numpad8Key.isPressed)
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }

        if (keyboard.numpad9Key.isPressed)
        {
            Application.Quit();
        }
    }
}
