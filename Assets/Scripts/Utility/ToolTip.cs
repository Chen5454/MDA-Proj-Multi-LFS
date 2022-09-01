using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using TMPro;

public class ToolTip : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] private string _toolTipContent;

    public void OnPointerEnter(PointerEventData eventData)
    {
        ToolTipManager.Instance.ActivateToolTip(_toolTipContent);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        ToolTipManager.Instance.DeactivateToolTip();
    }

    //[SerializeField] private Transform _playerUI;
    //[SerializeField] private GameObject _toolTipPrefab;
    //[SerializeField] private TextMeshProUGUI _toolTipTextMesh;
    //[SerializeField] private string _toolTipContent;
    //
    //private GameObject _newToolTip;
    //private const int _half = 2;
    //private bool _mouseHover = false;
    //
    //void Update()
    //{
    //    if (_newToolTip)
    //    {
    //        _newToolTip.transform.position = Input.mousePosition;
    //    }
    //}
    //
    //public void OnPointerEnter(PointerEventData eventData)
    //{
    //    _mouseHover = true;
    //    Debug.Log("Mouse Hover");
    //
    //    _newToolTip = Instantiate(_toolTipPrefab, transform);
    //    //_newToolTip.transform.position = newPos;
    //    _toolTipTextMesh.text = _toolTipContent;
    //}
    //
    //public void OnPointerExit(PointerEventData eventData)
    //{
    //    _mouseHover = false;
    //    Debug.Log("Mouse Hover");
    //
    //    _toolTipTextMesh.text = null;
    //    Destroy(_newToolTip);
    //}
}
