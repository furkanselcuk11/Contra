using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletObjectPool : MonoBehaviour
{
    public static BulletObjectPool Instance;

    [Serializable]
    public struct Pool
    {   // Pool Birden fazla farkl� nesne ekler
        // Quene= S�ran�n sonuna ekleyip, Ba��ndan ��kar�r�r
        public Queue<GameObject> pooledObjects; // Olu�turulacak nesneler bir s�rada tutulur - Quene,Liste veya dizide tutulablir
        public string name;
        public GameObject objectPrefab;   // Olu�turulacak nesne prefab�
        public int poolSize;  // Olu�turulacak nesne say�s�
    }

    [SerializeField] private Pool[] pools = null;

    public Pool[] Pools { get => pools; set => pools = value; }

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

        // --- Havuz olu�turma �oklu Obje ---   
        for (int j = 0; j < Pools.Length; j++)
        {
            Pools[j].pooledObjects = new Queue<GameObject>(); // Yeni bir s�ra olu�turulur
            for (int i = 0; i < Pools[j].poolSize; i++)
            {
                // for d�ng�s� ile"poolSize" Olu�turulacak nesne say�s� kadar yeni nesne olu�turur
                GameObject newObj = Instantiate(Pools[j].objectPrefab);  // Yeni olu�turulan nesneleri "newObj" ismi ile olu�turur
                newObj.SetActive(false);    // Ba�lang��ta t�m nesnenin aktifli�i false yapar

                newObj.transform.parent = GameObject.Find(Pools[j].name).gameObject.transform;
                newObj.name = Pools[j].name;

                Pools[j].pooledObjects.Enqueue(newObj);  // Olu�turulan yeni nesneler s�raya eklenir "poolSize" de�eri kadar nesne eklenir
            }
        }
    }
    public GameObject GetPooledObject(int objectType)
    {
        if (objectType >= Pools.Length)
        {
            return null;
        }
        GameObject newObj = Pools[objectType].pooledObjects.Dequeue();    // S�ran�n ba��ndan ilk nesneyi ��kar�r
        newObj.SetActive(true); // ��kar�lan nesnenin aktifli�i true yapar

        Pools[objectType].pooledObjects.Enqueue(newObj);  // ��kar�lan nesneyi tekrar s�ran�n sonune ekler ve havuzda d�ng� olu�turur 
        return newObj;
    }
    public void SetPooledObject(GameObject obj, int objectType)
    {
        if (objectType >= Pools.Length)
        {
            return;
        }

        // Nesneyi havuzun i�ine geri ekler
        obj.SetActive(false); // Nesneyi devre d��� b�rak�r
        Pools[objectType].pooledObjects.Enqueue(obj);
    }
}
