using System.Collections;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Rooms are also determined by ALS/BLS but that has its own parameter
/// </summary>
public enum DestinationRoom { CT, Maternity, Bypass };//TBF place where enums should be
public class DestinationRoomDropdown : MonoBehaviour
{
    [SerializeField]
    TMPro.TMP_Dropdown dropdown;
    private void OnEnable()
    {
        if (!dropdown)
            dropdown = GetComponent<TMPro.TMP_Dropdown>();

        dropdown.ClearOptions();
        
        dropdown.AddOptions(System.Enum.GetNames(typeof( DestinationRoom)).ToList());
    }
}
