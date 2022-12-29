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

    [SerializeField] private string _ct, _maternity, _bypass;

    private void OnEnable()
    {
        if (!dropdown)
            dropdown = GetComponent<TMPro.TMP_Dropdown>();

        dropdown.ClearOptions();
        List<string> options = new List<string>();
        options.Add(_ct);
        options.Add(_maternity);
        options.Add(_bypass);
        dropdown.AddOptions(options);
        //dropdown.AddOptions(System.Enum.GetNames(typeof( DestinationRoom)).ToList());
    }
}
