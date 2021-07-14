using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectPooler
{
    private GameObject prefab = null;
    private string name;
    private int size;
    private Transform root;
    private Queue<GameObject> queueObjectPool = new Queue<GameObject>();

    public void Initialize(GameObject _prefab, string _name, int _size, Transform _root) {
        prefab = _prefab;
        name = _name;
        size = _size;
        root = _root;

        if(prefab == null || size < 0 || root == null) {
            return;
        }

        for(int i = 0; i < size; i++) {
            queueObjectPool.Enqueue(CreateObject());
        }
    }

    private GameObject CreateObject() {
        GameObject newObject = Object.Instantiate(prefab, root);
        newObject.name = name + " (pooled)";
        newObject.SetActive(false);

        return newObject;
    }

    public GameObject Pop() {
        GameObject obj;
        
        if(0 < queueObjectPool.Count) {
            obj = queueObjectPool.Dequeue();
        }
        else {
            obj = CreateObject();
            size++;
        }
        obj.name = name;
        obj.SetActive(true);

        return obj;
    }

    public void Push(GameObject obj) {
        if(obj == null) {
            return;
        }

        obj.name = name + "(pooled)";
        obj.SetActive(false);
        queueObjectPool.Enqueue(obj);
    }
}