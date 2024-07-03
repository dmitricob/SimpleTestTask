
using CardsMath;
using CardsMath.Data;
using Core;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class Hud : MonoBehaviour
    {
        [SerializeField] private TMP_Text _scoreText;
        [SerializeField] private TMP_Text _comboText;
        [SerializeField] private TMP_Text _maxComboText;
        [SerializeField] private Button _newGameButton;

        public void Initialize(ISaveProvider saveProvider, CardsMatchController cardsMatchController)
        {
            _newGameButton.onClick.AddListener(cardsMatchController.StartNewRandomGame);
            
            saveProvider.Saved += OnSave;
            if(saveProvider.HasSave<CardsMatchSaveData>())
                UpdateHudElements(saveProvider.Load<CardsMatchSaveData>());
        }

        private void OnSave(object obj)
        {
            if (obj is CardsMatchSaveData data)
            {
                UpdateHudElements(data);
            }
        }
        
        private void UpdateHudElements(CardsMatchSaveData saveData)
        {
            _scoreText.text = $"Score: {saveData.Score}";
            _comboText.text = $"Combo: {saveData.CurrentCombo}";
            _maxComboText.text = $"Max Combo: {saveData.MaxCombo}";
        }
    }
}