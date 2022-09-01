using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GenericDisplayer : MonoBehaviour
{
    public Image GraphImage;

    public List<TextMeshProUGUI> _textsToDisplay;

    public void SetDisplayTexts(List<string> strings)
    {
        if (strings.Count != _textsToDisplay.Count)
            return;

        for (int i = 0; i < _textsToDisplay.Count; i++)
        {
            _textsToDisplay[i].text = strings[i];
        }
    }

    public void SetDisplayGraph(Sprite graph)
    {
        GraphImage.sprite = graph;
    }
}
