using System.Collections.Generic;
using UnityEngine;

namespace Core
{
    public class ObjectPool
    {
        private Dictionary<string, Queue<GameObject>> _pushedObjects = new Dictionary<string, Queue<GameObject>>();
        private Dictionary<string, HashSet<GameObject>> _popedObjects = new Dictionary<string, HashSet<GameObject>>();
        private readonly GameObject _poolRoot;


        public ObjectPool()
        {
            _poolRoot = new GameObject("Pool");
        }
        
        public GameObject Pop(GameObject prefab, Vector3 position, Quaternion rotation, Transform parent)
        {
            var obj = Pop(prefab);
            obj.transform.position = position;
            obj.transform.rotation = rotation;
            obj.transform.SetParent(parent);
            return obj;
        }
        public GameObject Pop(GameObject prefab)
        {
            GameObject obj;

            if (_pushedObjects.TryGetValue(prefab.name, out var queue) == false)
            {
                queue = new Queue<GameObject>();
                _pushedObjects.Add(prefab.name, queue);
            }
            
            if(queue.TryDequeue(out obj))
            {
                obj.SetActive(true);
            }
            else
            {
                obj = Object.Instantiate(prefab);
                obj.name = prefab.name;
            }

            if(_popedObjects.TryGetValue(prefab.name, out var set) == false)
            {
                set = new HashSet<GameObject>();
                _popedObjects.Add(prefab.name, set);
            }
            set.Add(obj);
            return obj;
        }
        
        public void Push(GameObject obj)
        {
            obj.SetActive(false);
            obj.transform.SetParent(_poolRoot.transform);
            
            if(_popedObjects.TryGetValue(obj.name, out var set) == false)
            {
                Debug.LogError("Object is not popped");
                return;
            }
            set.Remove(obj);
            
            if (_pushedObjects.TryGetValue(obj.name, out var queue) == false)
            {
                queue = new Queue<GameObject>();
                _pushedObjects.Add(obj.name, queue);
            }
            
            queue.Enqueue(obj);
        }

        public void PushAll(IEnumerable<GameObject> children)
        {
            foreach (var child in children)
            {
                Push(child);
            }
        }
    }
}