using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMarkColorizer : MonoBehaviour
{
    [SerializeField] private SpriteRenderer _markRenderer;

    private PlayerData _playerData;

    private void Start()
    {
        _playerData = transform.parent.GetComponent<PlayerData>();
    }

    private void Update()
    {
        if (!_playerData)
        {
            _playerData = transform.parent.GetComponent<PlayerData>();
        }

        if (_markRenderer.color != _playerData.CrewColor)
        {
            UpdateMarkColor();
        }
    }

    private void UpdateMarkColor()
    {
        _markRenderer.color = _playerData.CrewColor;
    }
}
