using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoading : MonoBehaviour
{
    private void Start()
    {
        StartCoroutine(LoadSceneAsync());
    }

    IEnumerator LoadSceneAsync()
    {
        yield return null;

        AsyncOperation operation = SceneManager.LoadSceneAsync("PlayingRoom");
    }
}
