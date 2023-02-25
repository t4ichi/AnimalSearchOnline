using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Selector : MonoBehaviour
{
    private void Awake()
    {
        
    }

    public void Open()
    {
        gameObject.SetActive(true);
    }

    public void Close()
    {
        gameObject.SetActive(false);
    }
}
