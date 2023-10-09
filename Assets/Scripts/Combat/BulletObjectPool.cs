using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletObjectPool : MonoBehaviour
{
    public static BulletObjectPool Instance;

    [Serializable]
    public struct Pool
    {   // Pool Birden fazla farklý nesne ekler
        // Quene= Sýranýn sonuna ekleyip, Baþýndan çýkarýrýr
        public Queue<GameObject> pooledObjects; // Oluþturulacak nesneler bir sýrada tutulur - Quene,Liste veya dizide tutulablir
        public string name;
        public GameObject objectPrefab;   // Oluþturulacak nesne prefabý
        public int poolSize;  // Oluþturulacak nesne sayýsý
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

        // --- Havuz oluþturma Çoklu Obje ---   
        for (int j = 0; j < Pools.Length; j++)
        {
            Pools[j].pooledObjects = new Queue<GameObject>(); // Yeni bir sýra oluþturulur
            for (int i = 0; i < Pools[j].poolSize; i++)
            {
                // for döngüsü ile"poolSize" Oluþturulacak nesne sayýsý kadar yeni nesne oluþturur
                GameObject newObj = Instantiate(Pools[j].objectPrefab);  // Yeni oluþturulan nesneleri "newObj" ismi ile oluþturur
                newObj.SetActive(false);    // Baþlangýçta tüm nesnenin aktifliði false yapar

                newObj.transform.parent = GameObject.Find(Pools[j].name).gameObject.transform;
                newObj.name = Pools[j].name;

                Pools[j].pooledObjects.Enqueue(newObj);  // Oluþturulan yeni nesneler sýraya eklenir "poolSize" deðeri kadar nesne eklenir
            }
        }
    }
    public GameObject GetPooledObject(int objectType)
    {
        if (objectType >= Pools.Length)
        {
            return null;
        }
        GameObject newObj = Pools[objectType].pooledObjects.Dequeue();    // Sýranýn baþýndan ilk nesneyi çýkarýr
        newObj.SetActive(true); // Çýkarýlan nesnenin aktifliði true yapar

        Pools[objectType].pooledObjects.Enqueue(newObj);  // Çýkarýlan nesneyi tekrar sýranýn sonune ekler ve havuzda döngü oluþturur 
        return newObj;
    }
    public void SetPooledObject(GameObject obj, int objectType)
    {
        if (objectType >= Pools.Length)
        {
            return;
        }

        // Nesneyi havuzun içine geri ekler
        obj.SetActive(false); // Nesneyi devre dýþý býrakýr
        Pools[objectType].pooledObjects.Enqueue(obj);
    }
}
