using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    private void Awake()
    {
        DontDestroyOnLoad(this);
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }
    void Start()
    {

    }
    void Update()
    {

    }
    public void GameStart()
    {
        SceneManager.LoadScene(2);
        AudioManager.Instance.PlayMusic("JungleHangar");
    }
    public void GameOver()
    {
        Debug.Log("Game Over!!!");
        SceneManager.LoadScene(0);
    }
}
