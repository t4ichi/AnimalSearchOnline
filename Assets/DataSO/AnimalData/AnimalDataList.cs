using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class AnimalDataList : ScriptableObject
{
    public List<AnimalDataSO> list = new List<AnimalDataSO>();
}
