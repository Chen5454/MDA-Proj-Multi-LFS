using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum MarkerIcon { Icon1, Icon2, Icon3, Icon4, Icon5, Icon6}

public class WorldMark : MonoBehaviour
{
    public MarkerIcon MarkerIconEnum;

    [SerializeField] private List<Sprite> _marks;
    public List<Sprite> Marks => _marks;
}
