using System;
using System.Collections;
using Core;
using UnityEngine;
using UnityEngine.UI;

namespace UI.Cards
{
    public class CardView : MonoBehaviour
    {
        public event Action<CardView> Flipped;
        
        [SerializeField] private Image _cardImage;
        [SerializeField] private Button _cardButton;
        [SerializeField] private float _flipSpeed = 2;
        [SerializeField] private CardData _cardData;

        // states in better way should be made some kind of state machine
        private bool _isMatched;
        private bool _isFlipped;

        private Sprite _baseSprite;
        
        private float _flipSpeedDeltaTime => _flipSpeed * Time.deltaTime;
        
        private ISoundSystem _soundSystem;
        private Coroutine _coroutine;
        private bool _isStarted;
        public int Id { get; private set; }
        
        public CardData CardData => _cardData;

        private void Start()
        {
            _isStarted = true;
        }

        private void OnEnable()
        {
            if(_isStarted == false)
            {
                _baseSprite = _cardImage.sprite;
                // _flipSpeedDeltaTime = _flipSpeed * Time.deltaTime;
                _cardButton.onClick.AddListener(OnCardClick);
            }
            
            Reset();
        }
        
        private void Reset()
        {
            _isMatched = false;
            _isFlipped = false;
            _cardImage.sprite = _baseSprite;
            _cardImage.transform.localScale = Vector3.one;
            _cardButton.interactable = true;
        }

        public void Initialize(ISoundSystem soundSystem, int number, int id, Sprite sprite, bool isFlipped = false)
        {
            _soundSystem = soundSystem;
            Id = number;
            _cardData = new CardData
            {
                Id = id,
                Sprite = sprite
            };
        }
        
        private void OnCardClick()
        {
            FLip();
            _soundSystem.PlaySound(Const.Sounds.CardFlip);
        }

        public void SetFlipped(bool isFlipped)
        {
            _isFlipped = isFlipped;
            _cardButton.interactable = !_isFlipped;
            _cardImage.sprite = _isFlipped ? _cardData.Sprite : _baseSprite;
        }
        
        public void FLip(float delay = 0)
        {
            _cardButton.interactable = false;
            // StopCoroutine(_coroutine);
            _coroutine = StartCoroutine(FlipAnimation(delay));
        }

        private IEnumerator FlipAnimation(float delay)
        {
            yield return new WaitForSeconds(delay);
            
            var scale = _cardImage.transform.localScale;
            while (scale.x > 0)
            {
                scale.x -= _flipSpeedDeltaTime;
                _cardImage.transform.localScale = scale;
                yield return null;
            }
            
            _isFlipped = !_isFlipped;
            _cardImage.sprite = _isFlipped ? _cardData.Sprite : _baseSprite;
            
            while (scale.x < 1)
            {
                scale.x += _flipSpeedDeltaTime;
                _cardImage.transform.localScale = scale;
                yield return null;
            }
            
            _cardButton.interactable = true;

            if(_isFlipped)
                Flipped?.Invoke(this);
        }

        public void Matched(bool isEmidiately = false)
        {
            _isMatched = true;
            // StopCoroutine(_coroutine);
            _coroutine = StartCoroutine(MatchedAnimation(isEmidiately));
        }

        private IEnumerator MatchedAnimation(bool isEmidiately)
        {
            if(isEmidiately)
            {
                SetMatched();
                yield break;
            }
            
            Vector3 originalScale = _cardImage.transform.localScale;
            Vector3 enlargedScale = originalScale * 1.3f;
            float duration = 0.1f; // Duration of the animation in seconds

            // Scale up
            for (float t = 0; t < duration; t += Time.deltaTime)
            {
                _cardImage.transform.localScale = Vector3.Lerp(originalScale, enlargedScale, t / duration);
                yield return null;
            }

            // Scale down to disappear
            for (float t = 0; t < duration; t += Time.deltaTime)
            {
                _cardImage.transform.localScale = Vector3.Lerp(enlargedScale, Vector3.zero, t / duration);
                yield return null;
            }

            SetMatched();
        }
        
        private void SetMatched()
        {
            _cardImage.transform.localScale = Vector3.zero;
        }
        
        private void OnDestroy()
        {
            _cardButton.onClick.RemoveListener(OnCardClick);
        }

        
    }
}