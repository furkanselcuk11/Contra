using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneCheck : MonoBehaviour
{    void Start()
    {
        if (GameManager.Instance == null)
        {
            // Her zaman ilk sahneden başlat
            SceneManager.LoadScene(0);
        }
    }
}
