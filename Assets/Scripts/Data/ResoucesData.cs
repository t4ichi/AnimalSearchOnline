using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class ResoucesData
{
    private static AnimalDataList _animalDataList;
    public static AnimalDataList GetAnimalDataList()
    {
        if(_animalDataList == null)
        {
            _animalDataList = Resources.Load<AnimalDataList>("Settings/AnimalDataList");
        }

        return _animalDataList;
    }

    private static CreditDataSO _creditData;
    public static CreditDataSO GetCreditData()
    {
        if(_creditData == null)
        {
            _creditData = Resources.Load<CreditDataSO>("Settings/CreditData");
        }
        return _creditData;
    }

    private static NGWordDataSO _ngwordData;
    public static NGWordDataSO GetNGWordData()
    {
        if (_ngwordData == null)
        {
            _ngwordData = Resources.Load<NGWordDataSO>("Settings/NGWordData");
            return _ngwordData;
        }
        return _ngwordData;
    }

    private static IAPProductsDataSO _iAPProductsData;
    public static IAPProductsDataSO GetIAPProductData()
    {
        if (_iAPProductsData == null)
        {
            _iAPProductsData = Resources.Load<IAPProductsDataSO>("Settings/IAPProductsData");
        }
        return _iAPProductsData;
    }
}
