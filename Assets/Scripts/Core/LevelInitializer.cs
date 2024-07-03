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
        [SerializeField] private AudioSource _audioSource;
        [SerializeField] private List<AudioClip> _audioClips;
        
        private ObjectPool _objectPool;
        private SaveProvider _saveProvider;
        private UnitySoundSystem _soundSystem;

        private void Start()
        {
            _objectPool = new ObjectPool();
            _saveProvider = new SaveProvider();
            _soundSystem = new UnitySoundSystem(_audioSource, _audioClips);

            _cardsMatchController.Initialize(_sprites, _objectPool, _saveProvider, _soundSystem);
            _hud.Initialize(_saveProvider, _cardsMatchController);
        }

        private void Update()
        {
            _saveProvider.Update();
        }
    }
}