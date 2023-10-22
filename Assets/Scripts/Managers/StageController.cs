using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StageController : MonoBehaviour
{
    public static StageController Instance;

    [SerializeField] private GameObject[] _flyBonuses;
    [SerializeField] private Transform[] _climberEnemiesSpawnerPoint;
    [SerializeField] private GameObject _enemyPrefab;
    [SerializeField] private float _spawnInterval = 1.0f; // Düþman spawn aralýðý (saniye)
    [SerializeField] private float _spawnDuration = 5.0f; // Toplam spawn süresi (saniye)

    private bool _isSpawning = false;
    private float _timer = 0.0f;

    public bool IsSpawning { get => _isSpawning; set => _isSpawning = value; }
    public float Timer { get => _timer; set => _timer = value; }

    private void Awake()
    {
        //DontDestroyOnLoad(this);
        if (Instance == null)
        {
            Instance = this;
        }
        //else
        //{
        //    Destroy(gameObject);
        //}
    }
    public void SpawnEnemies(int _climberEnemies)
    {
        StartCoroutine(SpawnEnemiesCoroutine(_climberEnemies));
    }
    private IEnumerator SpawnEnemiesCoroutine(int _climberEnemies)
    {
        while (Timer < _spawnDuration)
        {
            Instantiate(_enemyPrefab, _climberEnemiesSpawnerPoint[_climberEnemies].position, _climberEnemiesSpawnerPoint[_climberEnemies].rotation);
            yield return new WaitForSeconds(_spawnInterval);
            Timer += _spawnInterval;
        }
        IsSpawning = false;
    }
    public void FlyBonusOpen(int flyBonusNumber)
    {
        _flyBonuses[flyBonusNumber].gameObject.SetActive(true);
    }
}
