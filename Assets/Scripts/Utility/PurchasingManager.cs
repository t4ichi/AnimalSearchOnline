using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Purchasing;

public class PurchasingManager : Singleton<PurchasingManager>, IStoreListener
{
    [SerializeField] IAPProductsDataSO _productData;

    public static event Action<string> PurchaseCompleted;
    public static event Action<Product> PurchaseFailed;

    private static IStoreController sStoreController;              // Purchasing システムの参照
    private static IExtensionProvider sStoreExtensionProvider;     // 拡張した場合のPurchasing サブシステムの参照

    private bool isInitiatePurchaseCount;//購入可能になるまでのフラグ

    private void Start()
    {
        Initialize();
    }

    /// <summary>
    /// 初期化
    /// </summary>
    public void Initialize()
    {
        if (IsInitialized()) return;

        var module = StandardPurchasingModule.Instance();
        module.useFakeStoreAlways = false;
        var builder = ConfigurationBuilder.Instance(StandardPurchasingModule.Instance(), module);

        // プロダクトIDの登録
        foreach (var p in _productData.Products)
        {
#if UNITY_IOS
        builder.AddProduct(p.ID_IOS, p.productType);
#elif UNITY_ANDROID
            builder.AddProduct(p.ID_ANDROID, p.productType);

            builder.AddProduct(p.ID_IOS, p.productType);
#endif
        }
        UnityPurchasing.Initialize(this, builder);
    }

    /// <summary>
    /// 購入処理開始
    /// </summary>
    public void InitiatePurchase(string productId)
    {
        if (sStoreController == null) return;

        if (isInitiatePurchaseCount) { return; }
        else { StartCoroutine(InitiatePurchaseCountStart()); }

        var product = sStoreController.products.WithID(productId);
        sStoreController.InitiatePurchase(product);
    }

    /// <summary>
    /// 初期化したか
    /// </summary>
    public static bool IsInitialized()
    {
        return sStoreController != null && sStoreExtensionProvider != null;
    }

    /// <summary>
    /// 購入処理
    /// </summary>
    public static void Purchase(string productId)
    {
        // ストアの中で例外が出れば、try catch を使って、ここのロジックでキャッチできる
        try
        {
            // 購入が初期化されていれば
            if (IsInitialized())
            {
                // ... 汎用製品IDと、購入システムの製品群からProduct参照を取得する。
                Product product = sStoreController.products.WithID(productId);

                // もしデバイスのストアで製品が見つかったら、販売される用意が出来たということになる
                if (product != null && product.availableToPurchase)
                {
                    // ... 製品を買う。非同期で ProcessPurchase か OnPurchaseFailed の呼び出しの反応がある
                    Debug.Log(string.Format("Purchasing product asychronously: '{0}' - '{1}'", product.definition.id, product.definition.storeSpecificId));
                    sStoreController.InitiatePurchase(product);
                }
                else
                {
                    // ... さもなければ、失敗シチュエーションのレポート
                    Debug.Log("BuyProductID: FAIL. Not purchasing product, either is not found or is not available for purchase");
                }
            }
            else
            {
                // ... Purchasing で初期化が成功していない事実をレポートする。本来はもう少し待つか、初期化リトライを考慮するべき
                Debug.Log("BuyProductID FAIL. Not initialized.");
            }
        }
        // 例外ハンドリング
        catch (Exception e)
        {
            Debug.Log("BuyProductID: FAIL. Exception during purchase. " + e);
        }
    }

    /// <summary>
    /// 復元処理
    /// </summary>
    public static void RestorePurchases()
    {
        if (!IsInitialized())
        {
            Debug.Log("Couldn't restore IAP purchases: In-App Purchasing is not initialized.");
            return;
        }

        if (Application.platform == RuntimePlatform.IPhonePlayer ||
            Application.platform == RuntimePlatform.OSXPlayer)
        {
            // Apple Store固有のサブシステムを取得
            var apple = sStoreExtensionProvider.GetExtension<IAppleExtensions>();

            // 購入履歴の復元を非同期で開始
            // 以下のActionで確認応答を期待し、復元する購入済み商品がある場合はProcessPurchaseを実行する
            apple.RestoreTransactions((result) =>
            {
                // 復旧の第一段階。ProcessPurchaseでこれ以上応答がない場合、復元可能な購入はない
                Debug.Log("Restoring IAP purchases result: " + result);
            });
        }
        else
        {
            // Appleの端末で動作していない。購入した商品を復元する作業は必要ない
            Debug.Log("Couldn't restore IAP purchases: not supported on platform " + Application.platform.ToString());
        }
        Debug.Log("Couldn't restore IAP purchases: In-App Purchasing module is not enabled.");
    }

    /// <summary>
    /// ストア上での購入の失敗
    /// </summary>
    public void OnPurchaseFailed(Product product, PurchaseFailureReason failureReason)
    {
        // 製品購入が成功しなかった。詳しい情報はfailureReasonをチェック。ユーザーに失敗の理由はシェアした方がいい
        Debug.Log(string.Format("OnPurchaseFailed: FAIL. Product: '{0}', PurchaseFailureReason: {1}", product.definition.storeSpecificId, failureReason));
        PurchaseFailed?.Invoke(product);
    }

    #region private function
    // ストア上での購入の成功
    PurchaseProcessingResult IStoreListener.ProcessPurchase(PurchaseEventArgs purchaseEvent)
    {
        Debug.Log("ProcessPurchase" + purchaseEvent.purchasedProduct.definition.id);
        //報酬の受け渡しなどを追加
        PurchaseCompleted?.Invoke(purchaseEvent.purchasedProduct.definition.id);
        return PurchaseProcessingResult.Complete;   // 購入処理を完了する
    }

    // 初期化の成功
    void IStoreListener.OnInitialized(IStoreController controller, IExtensionProvider extensions)
    {
        // 購入処理に必要なので保持しておく
        sStoreController = controller;
        // リストア処理に必要なので保持しておく
        sStoreExtensionProvider = extensions;
    }

    //// 初期化の失敗
    void IStoreListener.OnInitializeFailed(InitializationFailureReason error)
    {
        // 初期化失敗時の処理
        switch (error)
        {
            case InitializationFailureReason.PurchasingUnavailable: // デバイス設定でアプリ内購入が無効になっている
                break;
            case InitializationFailureReason.NoProductsAvailable:   // 購入可能なプロダクトがない
                break;
            case InitializationFailureReason.AppNotKnown:           // 不明なアプリ
                break;
        }
    }

    //連打防止
    IEnumerator InitiatePurchaseCountStart()
    {
        isInitiatePurchaseCount = true;

        yield return new WaitForSeconds(3f);

        isInitiatePurchaseCount = false;
    }
    #endregion
}