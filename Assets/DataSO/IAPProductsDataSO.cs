using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Purchasing;

[CreateAssetMenu]
public class IAPProductsDataSO : ScriptableObject
{
    public List<IAPProduct> Products = new List<IAPProduct>();
}

[System.Serializable]
public class IAPProduct
{
    public string ID_IOS;
    public string ID_ANDROID;

    public ProductType productType;
}