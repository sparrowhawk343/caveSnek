using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneManager : MonoBehaviour
{
    public static SceneManager instance;
    private Snake snake;
    
    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            instance = this;
        }

        snake = FindObjectOfType<Snake>();
    }

    private void Update()
    {
        if (snake == null)
        {
            return;
        }
        if (snake.isDead)
        {
            LoadScene(1);
        }
    }

    public void LoadScene(int index)
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene(index);
    }


    public void QuitGame()
    {
        Application.Quit();
        Debug.Log("Quit request sent.");
    }
}
