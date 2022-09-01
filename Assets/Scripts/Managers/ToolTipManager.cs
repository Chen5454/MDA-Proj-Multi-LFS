using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ToolTipManager : MonoBehaviour
{
    public static ToolTipManager Instance;

    [SerializeField] private float _xOffset, _yOffset;
    public TextMeshProUGUI ToolTipTextMesh;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
        }
    }

    private void Start()
    {
        gameObject.SetActive(false);
    }

    private void Update()
    {
        if (gameObject.activeInHierarchy)
        {
            transform.position = new Vector2(Input.mousePosition.x + _xOffset, Input.mousePosition.y + _yOffset);
        }
    }

    public void ActivateToolTip(string content)
    {
        gameObject.SetActive(true);
        ToolTipTextMesh.text = content;
    }

    public void DeactivateToolTip()
    {
        gameObject.SetActive(false);
        ToolTipTextMesh.text = string.Empty;
    }
}
