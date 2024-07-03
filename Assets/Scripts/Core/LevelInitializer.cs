using System.Collections.Generic;
using CardsMath;
using UI;
using UnityEngine;

namespace Core
{
    public class LevelInitializer : MonoBehaviour
    {
        [SerializeField] CardsMatchController _cardsMatchController;
        [SerializeField] Hud _hud;
        [SerializeField] private List<Sprite> _sprites;
        
        private ObjectPool _objectPool;
        private SaveProvider _saveProvider;

        private void Start()
        {
            _objectPool = new ObjectPool();
            _saveProvider = new SaveProvider();
            
            _cardsMatchController.Initialize(_sprites, _objectPool, _saveProvider);
            _hud.Initialize(_saveProvider, _cardsMatchController);
        }

        private void Update()
        {
            _saveProvider.Update();
        }
    }
}