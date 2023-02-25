using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu()]
public class LeaderBoardDataSO : ScriptableObject
{
    public List<LeaderBoards> LeaderBoards = new List<LeaderBoards>();
}

[System.Serializable]
public class LeaderBoards
{
    public string Name;
    public string ID;
}