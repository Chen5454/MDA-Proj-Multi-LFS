using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ToggleButton : MonoBehaviour
{
    [SerializeField] private bool _isWithCheckmark;
    [SerializeField] private ToggleButton[] _cancelBtns;

    [field: SerializeField] private Sprite _btnOnImg, _btnOffImg;
    public Sprite BtnOnImg => _btnOnImg;
    public Sprite BtnOffImg => _btnOffImg;

    [field: SerializeField] public Image BtnImg { get; set; }
    [field: SerializeField] public Image CheckmarkImg { get; set; }
    [field: SerializeField] public bool IsBtnSelected { get; set; }

    public void ToggleBtnOnClick()
    {
        if (!_isWithCheckmark)
        {
            foreach (ToggleButton tglBtn in _cancelBtns)
                tglBtn.BtnImg.sprite = tglBtn.BtnOffImg;

            if (!IsBtnSelected)
            {
                BtnImg.sprite = _btnOnImg;
                IsBtnSelected = true;
            }
            else
            {
                BtnImg.sprite = _btnOffImg;
                IsBtnSelected = false;
            }
        }
        else
        {
            foreach (ToggleButton tglBtn in _cancelBtns)
            {
                tglBtn.BtnImg.sprite = tglBtn.BtnOffImg;
                tglBtn.CheckmarkImg.gameObject.SetActive(false);
                tglBtn.IsBtnSelected = false;
            }

            if (!IsBtnSelected)
            {
                BtnImg.sprite = _btnOnImg;
                CheckmarkImg.gameObject.SetActive(true);
                IsBtnSelected = true;
            }
            else
            {
                BtnImg.sprite = _btnOffImg;
                CheckmarkImg.gameObject.SetActive(false);
                IsBtnSelected = false;
            }
        }
    }
}
