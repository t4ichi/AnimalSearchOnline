using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;

public class ShareManager : MonoBehaviour
{
    public enum ScaleMode
    {
        AutoHeight,
        AutoWidth
    }

    public Texture2D ImgTex { get; set; }
    public Texture2D CapturedScreenshot { get; private set; }
    public Image staticImage;

    public GameObject container;

    public ScaleMode scaleMode = ScaleMode.AutoHeight;
    RectTransform containerRT;

    [SerializeField] Camera _camera;
    [SerializeField] RectTransform _screen;
    [SerializeField] RectTransform _middleBoard;

    /// <summary>
    /// 画面をキャプチャー
    /// </summary>
    public void Capture()
    {
        StartCoroutine(CRCaptureScreenshot());
    }

    /// <summary>
    /// ロードしてstaticImageに描画
    /// </summary>
    public void Load()
    {
        Texture2D texture = CapturedScreenshot;
        ImgTex = texture;
        LoadStaticImage();
    }

    /// <summary>
    /// PNGに変換
    /// </summary>
    public void CaputureTOPNG()
    {
        StartCoroutine(CaptureCoroutine());
    }


    #region private function
    private void Awake()
    {
        containerRT = container.GetComponent<RectTransform>();
        staticImage.GetComponent<RectTransform>().sizeDelta = containerRT.sizeDelta;
    }


    private IEnumerator CRCaptureScreenshot()
    {
        yield return new WaitForEndOfFrame();
        RenderTexture tempRT = RenderTexture.GetTemporary(Screen.width, Screen.height, 24);
        Camera.main.targetTexture = tempRT;
        Camera.main.Render();
        yield return null;
        Camera.main.targetTexture = null;
        yield return null;
        RenderTexture.active = tempRT;

        if (CapturedScreenshot == null)
        {
            //比は300:450 
            int height = (int)_middleBoard.sizeDelta.y;
            int width = (int)(height * 0.7);

            // カメラのレンダリング待ち
            yield return new WaitForEndOfFrame();
            Texture2D tex = ScreenCapture.CaptureScreenshotAsTexture();
            // 切り取る画像の左下位置を求める
            int x = (tex.width - width) / 2;
            int y = (tex.height - height) / 2;
            Color[] colors = tex.GetPixels(x, y + ScreenYPoint(), width, height);
            Texture2D saveTex = new Texture2D(width, height, TextureFormat.ARGB32, false);
            saveTex.SetPixels(colors);
            CapturedScreenshot = saveTex;
        }
        CapturedScreenshot.Apply();

        RenderTexture.active = null;
        RenderTexture.ReleaseTemporary(tempRT);
    }

    private void LoadStaticImage()
    {
        if (ImgTex != null)
        {
            Sprite sprite = Sprite.Create(ImgTex, new Rect(0.0f, 0.0f, ImgTex.width, ImgTex.height), new Vector2(0.5f, 0.5f));
            Transform imgTf = staticImage.gameObject.transform;
            RectTransform imgRtf = staticImage.GetComponent<RectTransform>();

            float scaleFactor;
            if (scaleMode == ScaleMode.AutoHeight)
                scaleFactor = imgRtf.rect.width / sprite.rect.width;
            else
                scaleFactor = imgRtf.rect.height / sprite.rect.height;

            staticImage.sprite = sprite;
            staticImage.SetNativeSize();
            imgTf.localScale = imgTf.localScale * scaleFactor;

            ScaleContainer(sprite.rect.width / sprite.rect.height);
        }

        staticImage.gameObject.SetActive(true);
    }


    private int ScreenYPoint()
    {
        int width = (int)RectTransformUtility.WorldToScreenPoint(_camera, _middleBoard.transform.position).x;
        int height = (int)RectTransformUtility.WorldToScreenPoint(_camera, _middleBoard.transform.position).y;

        int screenheight = (int)_screen.sizeDelta.y;
        int screeny = height - (screenheight / 2);

        Debug.Log("width :" + width + "height:" + height);
        Debug.Log("screenY:" + screeny + "screenheight:" + screenheight);

        return screeny;
    }

    private void ScaleContainer(float aspect)
    {
        if (scaleMode == ScaleMode.AutoHeight)
        {
            float y = containerRT.sizeDelta.x / aspect;
            containerRT.sizeDelta = new Vector2(containerRT.sizeDelta.x, y);
 
        }
        else
        {
            float x = containerRT.sizeDelta.y * aspect;
            containerRT.sizeDelta = new Vector2(x, containerRT.sizeDelta.y);
        }
    }

    private IEnumerator CaptureCoroutine()
    {
        int width = (int)_middleBoard.sizeDelta.x;
        int height = (int)_middleBoard.sizeDelta.y;

        // カメラのレンダリング待ち
        yield return new WaitForEndOfFrame();
        Texture2D tex = ScreenCapture.CaptureScreenshotAsTexture();
        // 切り取る画像の左下位置を求める
        int x = (tex.width - width) / 2;
        int y = (tex.height - height) / 2;
        Color[] colors = tex.GetPixels(x, y + ScreenYPoint(), width, height);
        Texture2D saveTex = new Texture2D(width, height, TextureFormat.ARGB32, false);
        saveTex.SetPixels(colors);
        File.WriteAllBytes("ss.png", saveTex.EncodeToPNG());
    }
    #endregion
}
