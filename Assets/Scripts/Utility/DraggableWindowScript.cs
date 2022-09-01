using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class DraggableWindowScript : MonoBehaviour, IDragHandler, IBeginDragHandler, IEndDragHandler, IPointerDownHandler
{
    private RectTransform _rectTransform;
    private Canvas _canvas;
    private Image _bgImage;
    private Color _bgColor;

    public bool TransparentOnDrag = true;
    public bool SetToTop = true;


    private void Awake()
    {
        _rectTransform = GetComponent<RectTransform>();
        if (TryGetComponent(out Canvas canvas))
        {
            _canvas = canvas;
        }
        if (TryGetComponent(out Image image))
        {
            _bgImage = image;
            _bgColor = _bgImage.color;
        }
    }


    void IDragHandler.OnDrag(PointerEventData eventData)
    {
        if (_canvas != null && TransparentOnDrag)
        {
            _rectTransform.anchoredPosition += eventData.delta / _canvas.scaleFactor;
        }
        else
        {
            _rectTransform.anchoredPosition += eventData.delta;
        }
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (_bgImage != null && TransparentOnDrag)
        {
            _bgColor.a = 0.5f;
            _bgImage.color = _bgColor;
        }
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (_bgImage != null)
        {
            _bgColor.a = 1f;
            _bgImage.color = _bgColor;
        }
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (SetToTop)
        {
            _rectTransform.SetAsLastSibling();
        }
    }
}
