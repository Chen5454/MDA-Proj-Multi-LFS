using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;

public static class UniqueIDGenerator
{
   //INITIAL APPROACH
   //Basically use DateTime - something that won't repeat

    public static string GenerateUniqueID()
    {
        return $"{DateTime.Now.ToString("d-M-y-h-m-s")}";
    }
}
