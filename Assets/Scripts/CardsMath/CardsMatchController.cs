using System.Collections.Generic;
using System.Linq;
using CardsMath.Data;
using Core;
using UI.Cards;
using UI.Grid;
using UnityEngine;
using Random = UnityEngine.Random;

namespace CardsMath
{
    public class CardsMatchController : MonoBehaviour
    {
        [SerializeField] private int _maxRows = 6;
        [SerializeField] private int _maxColumns = 6;
        
        [SerializeField] private GameObject _gridObejct;
        [SerializeField] private GameObject _cardPrefab;

        private readonly List<CardView> _cards = new List<CardView>();
        private IGrid _grid;
        private bool _hasFlippedCard;
        private CardView _lastFlippedCardView;
        private bool _isInitialized;
        
        private CardsMatchSaveData _cardsMatchSaveData;

        private List<Sprite> _sprites;
        private ObjectPool _objectPool;
        private ISaveProvider _saveProvider;
        private ISoundSystem _soundSystem;

        private void Awake()
        {
            _grid = _gridObejct.GetComponent<IGrid>();
        }
    
        public void Initialize(List<Sprite> sprites, ObjectPool objectPool, ISaveProvider saveProvider, ISoundSystem soundSystem)
        {
            _sprites = sprites;
            _objectPool = objectPool;
            _saveProvider = saveProvider;
            _soundSystem = soundSystem;
            
            _isInitialized = true;
            if(saveProvider.HasSave<CardsMatchSaveData>())
                StartGame(saveProvider.Load<CardsMatchSaveData>());
            else
                StartNewRandomGame();
        }
    
        private void OnEnable()
        {
            if (_isInitialized)
                StartNewRandomGame();
        }

        public void StartNewRandomGame()
        {
            int rows, columns;
            do
            {
                rows = Random.Range(2, _maxRows);
                columns = Random.Range(2, _maxColumns);
            } while (rows * columns % 2 != 0);

            var cardsMatchSaveData = NewGameDataInit(rows, columns);
            _saveProvider.Save(cardsMatchSaveData);
            StartGame(cardsMatchSaveData);
        }

        private CardsMatchSaveData NewGameDataInit(int rows, int columns)
        {
            return new CardsMatchSaveData
            {
                Rows = rows,
                Columns = columns,
                Score = 0,
                MaxCombo = 0,
                CurrentCombo = 0,
                CardsMatched = new bool[rows * columns],
                CardsId = ShuffleId(rows, columns).ToArray()
            };;
        }
        
        private List<int> ShuffleId(int rows, int columns)
        {
            var shuffledId = new List<int>();
            var cardsAmount = rows * columns / 2;
            for (var i = 0; i < cardsAmount; i++)
            {
                shuffledId.Add(i);
                shuffledId.Add(i);
            }
            
            for (var i = 0; i < shuffledId.Count; i++)
            {
                var temp = shuffledId[i];
                var randomIndex = Random.Range(i, shuffledId.Count);
                shuffledId[i] = shuffledId[randomIndex];
                shuffledId[randomIndex] = temp;
            }

            return shuffledId;
        }

        public void StartGame(CardsMatchSaveData cardsMatchSaveData)
        {
            _cardsMatchSaveData = cardsMatchSaveData;
            Clear();
            _grid.SetGridSize(cardsMatchSaveData.Rows, cardsMatchSaveData.Columns);
            for (var i = 0; i < cardsMatchSaveData.Rows * cardsMatchSaveData.Columns; i++)
            {
                var card = _objectPool.Pop(_cardPrefab).GetComponent<CardView>();
                card.Initialize(_soundSystem,i, cardsMatchSaveData.CardsId[i], _sprites[cardsMatchSaveData.CardsId[i]]);
                if (cardsMatchSaveData.CardsMatched[i])
                {
                    card.Matched(true);
                }
                else
                {
                    card.SetFlipped(true);
                    card.FLip(1);
                    card.Flipped += OnCardFlipped;
                }
                
                _grid.AddChild(card.gameObject);
                _cards.Add(card);
            }
        }

        private void OnCardFlipped(CardView currentCardView)
        {
            if(_hasFlippedCard == false)
            {
                _hasFlippedCard = true;
                _lastFlippedCardView = currentCardView;
                return;
            }
        
            if(CheckMatch(_lastFlippedCardView.CardData, currentCardView.CardData))
            {
                _soundSystem.PlaySound(Const.Sounds.CardMatch);
                
                currentCardView.Matched();
                _lastFlippedCardView.Matched();
                
                _cardsMatchSaveData.Score +=10;
                _cardsMatchSaveData.CurrentCombo++;
                if (_cardsMatchSaveData.CurrentCombo > _cardsMatchSaveData.MaxCombo)
                    _cardsMatchSaveData.MaxCombo = _cardsMatchSaveData.CurrentCombo;
                
                _cardsMatchSaveData.CardsMatched[_lastFlippedCardView.Id] = true;
                _cardsMatchSaveData.CardsMatched[currentCardView.Id] = true;
                
                if(_cardsMatchSaveData.CardsMatched.All(matched => matched))
                    _soundSystem.PlaySound(Const.Sounds.GameFinished);
            }
            else
            {
                _soundSystem.PlaySound(Const.Sounds.CardMismatch);
                _cardsMatchSaveData.CurrentCombo = 0;
                
                _lastFlippedCardView.FLip(1);
                currentCardView.FLip(1);
            }
            _saveProvider.Save(_cardsMatchSaveData);
            
            _hasFlippedCard = false;
            _lastFlippedCardView = null;
        
        }
    
        // ToDo: can be moved to higher level of abstraction to handle different types of cards data
        private bool CheckMatch(CardData card1, CardData card2)
        {
            return card1.Id == card2.Id;
        }

        private void Clear()
        {
            foreach (var card in _cards)
            {
                card.Flipped -= OnCardFlipped;
                _objectPool.Push(card.gameObject);
            }
            _cards.Clear();
        }
    }
}