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
            if (transform.parent.GetComponent<PlayerData>())
            {
                _playerData = transform.parent.GetComponent<PlayerData>();
            }
            else
            {
                return;
            }
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
