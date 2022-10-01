using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SliderButton : MonoBehaviour
{
    [SerializeField] private Button _sliderBtn;
    [SerializeField] private Image _btnImg, _iconImg;
    [SerializeField] private TextMeshProUGUI _btnTxt;
    [SerializeField] private Sprite _btnOnImg, _btnOffImg, _iconOnImg, _iconOffImg;
    [SerializeField] private Color _onColor, _offColor;

    private bool _isBtnSelected = false;

    public void SliderBtnOnClick()
    {
        if (!_isBtnSelected)
        {
            _btnImg.sprite = _btnOnImg;
            _iconImg.sprite = _iconOnImg;
            _btnTxt.color = _onColor;
            _isBtnSelected = true;
        }
        else
        {
            _btnImg.sprite = _btnOffImg;
            _iconImg.sprite = _iconOffImg;
            _btnTxt.color = _offColor;
            _isBtnSelected = false;
        }

        _btnTxt.alpha = 255f;
    }
}
