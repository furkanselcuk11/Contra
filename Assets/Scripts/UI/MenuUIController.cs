using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuUIController : MonoBehaviour
{
    
    void Start()
    {
        AudioManager.Instance.PlayMusic("TitleScreen");
    }
    void Update()
    {
        
    }
    public void GameStartButton()
    {
        GameManager.Instance.GameStart();
    }
}
