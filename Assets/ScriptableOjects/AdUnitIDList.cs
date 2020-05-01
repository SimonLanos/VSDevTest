using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/AdUnitIDList", order = 1)]
public class AdUnitIDList : ScriptableObject
{
    public List<string> adUnitIDList = new List<string>();
}
