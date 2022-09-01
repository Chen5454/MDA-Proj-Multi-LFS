using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldMark : MonoBehaviour
{
    [SerializeField] private List<Sprite> _marks;
    public List<Sprite> Marks => _marks;
}
