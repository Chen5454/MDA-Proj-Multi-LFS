using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ToggleButton : MonoBehaviour
{
    [SerializeField] private Button _toggleBtn;
    [SerializeField] private ToggleButton[] _cancelBtns;

    [field: SerializeField] private Sprite _btnOnImg, _btnOffImg;
    public Sprite BtnOnImg => _btnOnImg;
    public Sprite BtnOffImg => _btnOffImg;

    [field: SerializeField] public Image BtnImg { get; set; }

    private bool _isBtnSelected = false;
    public bool IsBtnSelected => _isBtnSelected;

    public void ToggleBtnOnClick()
    {
        if (!_isBtnSelected)
        {
            BtnImg.sprite = _btnOnImg;
            _isBtnSelected = true;
        }
        else
        {
            BtnImg.sprite = _btnOffImg;
            _isBtnSelected = false;
        }

        foreach (ToggleButton tglBtn in _cancelBtns)
            tglBtn.BtnImg.sprite = tglBtn.BtnOffImg;
    }
}
