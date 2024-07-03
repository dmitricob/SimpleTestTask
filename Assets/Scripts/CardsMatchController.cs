using System.Collections.Generic;
using Core;
using UI.Grid;
using UnityEngine;
using Random = UnityEngine.Random;

public class CardsMatchController : MonoBehaviour
{
    [SerializeField] private GameObject _gridObejct;
    [SerializeField] private GameObject _cardPrefab;

    [SerializeField] private List<GameObject> _cards;
    
    private IGrid _grid;
    private bool _isStarted;

    private void Awake()
    {
        _grid = _gridObejct.GetComponent<IGrid>();
    }

    private void Start()
    {
        _isStarted = true;
        StartGame(Random.Range(2, 6), Random.Range(2, 6));

    }

    private void OnEnable()
    {
        if(_isStarted)
            StartGame(Random.Range(2, 6), Random.Range(2, 6));
    }

    public void StartGame(int rows, int columns)
    {
        Clear();
        _grid.SetGridSize(rows, columns);
        for (var i = 0; i < rows * columns; i++)
        {
            _cards.Add(ObjectPool.Instance.Pop(_cardPrefab));
            _grid.AddChild(_cards[i]);
        }
    }

    private void Clear()
    {
        ObjectPool.Instance.PushAll(_cards);
        _cards.Clear();
    }
}