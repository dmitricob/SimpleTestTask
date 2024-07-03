using System;
using System.Collections.Generic;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Core
{
    public class SaveProvider : ISaveProvider, IUpdatable
    {
        public event Action<object> Saved;
        
        private Dictionary<Type, Action> _saveActions = new Dictionary<Type, Action>();
        public void Update()
        {
            if(_saveActions.Count == 0)
                return;
            
            foreach (var dirtyObject in _saveActions)
            {
                dirtyObject.Value();
            }
            _saveActions.Clear();    
        }
        
        // avoid multiple saves of the same object at one frame

        public void Save<T>(T obj, string key = null)
        {
            if(_saveActions.ContainsKey(typeof(T)))
                return;
            
            _saveActions.Add(typeof(T), () => SaveInternal(obj, key));
        }
        
        private void SaveInternal<T>(T obj, string key = null)
        {
            var json = JsonUtility.ToJson(obj);
            PlayerPrefs.SetString(typeof(T) +  key, json);
            Saved?.Invoke(obj);
        }
        
        public T Load<T>(string key = null)
        {
            var json = PlayerPrefs.GetString(typeof(T) +  key);
            return JsonUtility.FromJson<T>(json);
        }
        public bool HasSave<T>(string key = null)
        {
            return PlayerPrefs.HasKey(typeof(T) +  key);
        }
    }

    public interface IUpdatable
    {
        void Update();
    }

    public interface ISaveProvider
    {
        //ToDo: change to observer pattern
        event Action<object> Saved; // boxing is bad :C, but now its for save time,   
        
        void Save<T>(T obj, string key = null);
        T Load<T>(string key = null);
        bool HasSave<T>(string key = null);
    }
}