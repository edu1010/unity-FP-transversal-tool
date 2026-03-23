    using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour
{
    private static SceneLoader _instance;
   

    public static SceneLoader Instance { get { return _instance; } }

    private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            DontDestroyOnLoad(this);
            _instance = this;
        }
    }
    public string[] LevelNames;

    public string LoadingSceneName;

    public void LoadLevel(int level)
    {
        LoadingSceneName = LevelNames[level];

        SceneManager.LoadScene("Loading");
        LoadSceneAsync(LoadingSceneName);
    }

    private void LoadSceneAsync(string name)
    {
        SceneManager.LoadSceneAsync(name);
    }
   
}
