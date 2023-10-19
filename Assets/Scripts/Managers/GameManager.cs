using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    private bool _isDeath = false;
    private bool _gameStarted = false;
    private bool _gameEnd = false;

    public bool IsDeath { get => _isDeath; set => _isDeath = value; }
    public bool GameEnd { get => _gameEnd; set => _gameEnd = value; }
    public bool GameStarted { get => _gameStarted; set => _gameStarted = value; }

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
        _gameStarted = false;
        _isDeath = false;
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
        _gameStarted = true;
    }
    public void GameOver()
    {
        Debug.Log("Game Over!!!");
        _gameStarted = false;
        SceneManager.LoadScene(0);
    }
}
