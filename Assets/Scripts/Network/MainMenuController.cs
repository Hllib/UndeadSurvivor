using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuController : MonoBehaviour
{
    [SerializeField] private GameObject _skinsPanel;

    public void ConnectToRoom()
    {
        SceneManager.LoadScene("Loading");
    }

    public void ShowSkinPanel()
    {
        _skinsPanel.SetActive(!_skinsPanel.activeSelf);
    }

    public void Exit()
    {
        Application.Quit();
    }
}
