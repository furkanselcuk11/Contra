using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour
{
    public GameObject ManagerContainer; // Manager Sciptlerinin parent objecti
    public string SceneToLoad;
    IEnumerator Start()
    {
        yield return new WaitForEndOfFrame();   // Tum sahne install edildikten sonra calis

        DontDestroyOnLoad(ManagerContainer); // Manager Sciptlerini tum sahnede calistirir
        SceneManager.LoadScene(SceneToLoad);
    }
}
