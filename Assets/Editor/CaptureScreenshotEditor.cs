using System;
using UnityEngine;
using UnityEditor;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;

public class CaptureScreenShotWindow : EditorWindow
{
    // iOS用の解像度リスト
    static CaptureResolution[] resolutions_IOS =
    {
            new CaptureResolution(1242, 2208),
            new CaptureResolution(1242, 2688),
            new CaptureResolution(2048, 2732),
        };

    [MenuItem("ScreenShot/スクリーンショットキャプチャ")]
    static void OpenWindow()
    {
        GetWindow<CaptureScreenShotWindow>(true, "スクリーンショットキャプチャ");
    }

    [Serializable]
    class CaptureResolution
    {
        public CaptureResolution(int _width, int _height)
        {
            width = _width;
            height = _height;
        }
        public int width = 1920;
        public int height = 1080;
    }

    static readonly string KeyPrefix = "CaptureScreenShot_";
    static readonly string PathKey = KeyPrefix + "Path";
    static readonly string ReverseKey = KeyPrefix + "Reverse";
    static readonly string StopTimeKey = KeyPrefix + "StopTime";
    static readonly string ResoCountKey = KeyPrefix + "ResoCount";
    static readonly string ResoWidthBaseKey = KeyPrefix + "ResoW_";
    static readonly string ResoHeightBaseKey = KeyPrefix + "ResoH_";

    string m_path = "";
    bool m_started = false;
    bool m_reverse = false;
    bool m_stopTime = true;

    [SerializeField] List<CaptureResolution> captureResolutions = null;
    SerializedObject resolutionSerializedObj;
    SerializedProperty resolutionProp;

    EditorCoroutine m_coroutine = null;
    int m_selectedIndexOld = -1;
    GameViewSizeGroupType m_currentGroupType;

    private void OnEnable()
    {
        captureResolutions = new List<CaptureResolution>(32);

        // 読み込み
        m_path = EditorPrefs.GetString(PathKey, "");
        m_reverse = EditorPrefs.GetBool(ReverseKey, false);
        m_stopTime = EditorPrefs.GetBool(StopTimeKey, true);

        int reso_count = EditorPrefs.GetInt(ResoCountKey, 0);
        for (int i = 0; i < reso_count; ++i)
        {
            string indexText = i.ToString("D2");
            int width = EditorPrefs.GetInt(ResoWidthBaseKey + indexText, 0);
            int height = EditorPrefs.GetInt(ResoHeightBaseKey + indexText, 0);
            if (width > 0 && height > 0)
            {
                captureResolutions.Add(new CaptureResolution(width, height));
            }
        }

        resolutionSerializedObj = new SerializedObject(this);
        resolutionProp = resolutionSerializedObj.FindProperty("captureResolutions");
    }

    private void OnDisable()
    {
        if (m_coroutine != null)
        {
            m_coroutine.Stop();
            m_coroutine = null;
        }

        if (m_timeScaleOld >= 0f)
        {
            Time.timeScale = m_timeScaleOld;
            m_timeScaleOld = -1f;
        }

        // 保存
        EditorPrefs.SetString(PathKey, m_path);
        EditorPrefs.SetBool(ReverseKey, m_reverse);
        EditorPrefs.SetBool(StopTimeKey, m_stopTime);

        int reso_count = captureResolutions.Count;
        EditorPrefs.SetInt(ResoCountKey, reso_count);
        for (int i = 0; i < reso_count; ++i)
        {
            var reso = captureResolutions[i];
            string indexText = i.ToString("D2");
            EditorPrefs.SetInt(ResoWidthBaseKey + indexText, reso.width);
            EditorPrefs.SetInt(ResoHeightBaseKey + indexText, reso.height);
        }

    }

    void OnGUI()
    {
        resolutionSerializedObj.Update();

        if (m_started)
        {
            GUILayout.Label("スクリーンショットを撮影中...");
        }
        else
        {
            GUILayout.Label("設定:");
            m_reverse = GUILayout.Toggle(m_reverse, "縦横反転");
            m_stopTime = GUILayout.Toggle(m_stopTime, "撮影時に止める(TimeScale)");

            GUILayout.Space(16f);

            GUILayout.Label("解像度リスト:");
            EditorGUILayout.PropertyField(resolutionProp, true);
            resolutionSerializedObj.ApplyModifiedProperties();

            if (GUILayout.Button("解像度をクリア"))
            {
                captureResolutions.Clear();
            }

            if (GUILayout.Button("iOS解像度を追加"))
            {
                AddResolutions(resolutions_IOS);
            }

            GUILayout.Space(16f);

            GUILayout.Label("保存フォルダ:");
            m_path = GUILayout.TextField(m_path);

            if (GUILayout.Button("フォルダを開く"))
            {
                string filePath = m_path;
                filePath = filePath.Replace('/', Path.DirectorySeparatorChar);
                EditorUtility.RevealInFinder(filePath);
            }

            if (GUILayout.Button("スクリーンショットを保存"))
            {
                Capture();
            }
        }
    }

    void AddResolutions(CaptureResolution[] resolutions)
    {
        foreach (var reso in resolutions)
        {
            captureResolutions.Add(reso);
        }
    }

    void Capture()
    {
        if (m_started)
        {
            Debug.Log("Already Started Capture...");
            return;
        }

        // 解像度指定がない場合はそのまま
        if (captureResolutions.Count <= 0)
        {
            string dateString = DateTime.Now.ToString("yyyyMMddHHmmss");
            string filePath = string.Format(Path.Combine(m_path, "ScreenCapture_{0}.png"), dateString);
            Debug.Log(string.Format("Saved a new screenshot as {0}", filePath));
            ScreenCapture.CaptureScreenshot(filePath);
        }
        // 複数キャプチャ
        else
        {
            m_coroutine = EditorCoroutine.Start(MultipleCaptureScreen());
        }
    }


    float m_timeScaleOld = -1f;

    IEnumerator MultipleCaptureScreen()
    {
        m_started = true;

        string TmpLabel = "ScreenCaptureResoTemp";

        m_timeScaleOld = Time.timeScale;
        if (m_stopTime)
        {
            Time.timeScale = 0f;
        }

        foreach (CaptureResolution reso in captureResolutions)
        {
            int width = m_reverse ? reso.height : reso.width;
            int height = m_reverse ? reso.width : reso.height;

            string dateString = DateTime.Now.ToString("yyyyMMddHHmmss");
            string filePath = string.Format(Path.Combine(m_path, "ScreenCapture_{0}_{1}.png"), dateString, width.ToString() + "x" + height.ToString());
            Debug.Log(string.Format("Save a new screenshot as {0}", filePath));

            StartCustomSize(TmpLabel, width, height);
            yield return new WaitForSeconds(0.5f);

            // キャプチャ＆待つ
            ScreenCapture.CaptureScreenshot(string.Format(filePath));

            while (!IsSaved(filePath))
            {
                yield return new WaitForSeconds(0.1f);
            }

            EndCustomSize(TmpLabel, width, height);
        }

        Debug.Log("Saved All Captures!!");

        if (m_stopTime)
        {
            Time.timeScale = m_timeScaleOld;
            m_timeScaleOld = -1f;
        }

        m_started = false;
        m_coroutine = null;

        yield return null;
    }

    // UnityEditor.GameViewSizeTypeと合わせる
    public enum GameViewSizeType
    {
        AspectRatio = 0,
        FixedResolution = 1,
    }

    void StartCustomSize(string name, int width, int height)
    {
        var asm = typeof(Editor).Assembly;
        Type gameViewType = asm.GetType("UnityEditor.GameView");
        Type gameViewSize = asm.GetType("UnityEditor.GameViewSize");
        Type gameViewSizes = asm.GetType("UnityEditor.GameViewSizes");
        Type gameViewSizeType = asm.GetType("UnityEditor.GameViewSizeType");
        Type gameViewSizeGroup = asm.GetType("UnityEditor.GameViewSizeGroup");

        EditorWindow gameView = EditorWindow.GetWindow(gameViewType, false, "Game", false);

        PropertyInfo currentSizeGroupType = gameViewType.GetProperty("currentSizeGroupType", BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Static);
        m_currentGroupType = (GameViewSizeGroupType)currentSizeGroupType.GetValue(gameView, null);

        MethodInfo getGroup = gameViewSizes.GetMethod("GetGroup");
        Type scriptableSingleton = typeof(ScriptableSingleton<>).MakeGenericType(gameViewSizes);
        PropertyInfo scriptableSingletonInstance = scriptableSingleton.GetProperty("instance");
        object gameViewSizesInstance = scriptableSingletonInstance.GetValue(null, null);
        object group = getGroup.Invoke(gameViewSizesInstance, new object[] { m_currentGroupType });

        Type[] paramTypes = new Type[] { gameViewSize };
        MethodInfo addCustomSize = gameViewSizeGroup.GetMethod("AddCustomSize", BindingFlags.Public | BindingFlags.Instance, null, paramTypes, null);
        ConstructorInfo constructor = gameViewSize.GetConstructor(new Type[] { gameViewSizeType, typeof(int), typeof(int), typeof(string) });

        // インデックスを保持しておく
        PropertyInfo selectedSizeIndex = gameViewType.GetProperty("selectedSizeIndex", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
        m_selectedIndexOld = (int)selectedSizeIndex.GetValue(gameView, null);

        // 追加
        {
            object newSize = constructor.Invoke(new object[] { (int)GameViewSizeType.FixedResolution, width, height, name });
            addCustomSize.Invoke(group, new object[] { newSize });
        }

        // 追加したのをインデックス設定
        int index = FindSameResolution(group, name, width, height);
        if (index >= 0)
        {
            selectedSizeIndex.SetValue(gameView, index, null);
        }
    }

    void EndCustomSize(string name, int width, int height)
    {
        var asm = typeof(Editor).Assembly;
        Type gameViewSize = asm.GetType("UnityEditor.GameViewSize");
        Type gameViewSizes = asm.GetType("UnityEditor.GameViewSizes");
        Type gameViewSizeType = asm.GetType("UnityEditor.GameViewSizeType");
        Type gameViewSizeGroup = asm.GetType("UnityEditor.GameViewSizeGroup");

        MethodInfo getGroup = gameViewSizes.GetMethod("GetGroup");
        Type scriptableSingleton = typeof(ScriptableSingleton<>).MakeGenericType(gameViewSizes);
        PropertyInfo scriptableSingletonInstance = scriptableSingleton.GetProperty("instance");
        object gameViewSizesInstance = scriptableSingletonInstance.GetValue(null, null);
        object group = getGroup.Invoke(gameViewSizesInstance, new object[] { m_currentGroupType });

        Type[] paramTypes = new Type[] { typeof(int) };
        MethodInfo removeCustomSize = gameViewSizeGroup.GetMethod("RemoveCustomSize", BindingFlags.Public | BindingFlags.Instance, null, paramTypes, null);

        // 同名のインデックスを探す
        int index = FindSameResolution(group, name, width, height);
        if (index >= 0)
        {
            removeCustomSize.Invoke(group, new object[] { index });
        }

        // 元に戻す
        Type gameViewType = asm.GetType("UnityEditor.GameView");
        PropertyInfo selectedSizeIndex = gameViewType.GetProperty("selectedSizeIndex", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
        EditorWindow gameView = EditorWindow.GetWindow(gameViewType, false, "Game", false);
        selectedSizeIndex.SetValue(gameView, m_selectedIndexOld, null);
    }

    // 同名のインデックスを探す
    int FindSameResolution(object group, string name, int width, int height)
    {
        int index = -1;
        var getDisplayTexts = group.GetType().GetMethod("GetDisplayTexts");
        var displayTexts = getDisplayTexts.Invoke(group, null) as string[];
        string targetName = string.Format("{0} ({1}x{2})", name, width, height);
        for (int i = 0; i < displayTexts.Length; ++i)
        {
            string textTmp = displayTexts[i];
            if (textTmp == targetName)
            {
                index = i;
                break;
            }
        }
        return index;
    }

    // 保存されているか
    bool IsSaved(string path)
    {
        bool isExist = File.Exists(path);
        bool isLocked = false;
        FileStream stream = null;
        try
        {
            stream = new FileStream(path, FileMode.Open, FileAccess.ReadWrite, FileShare.None);
        }
        catch
        {
            isLocked = true;
        }
        finally
        {
            if (stream != null)
            {
                stream.Close();
            }
        }
        return isExist && !isLocked;
    }

    // コルーチン用
    public class EditorCoroutine
    {
        public static EditorCoroutine Start(IEnumerator _routine)
        {
            EditorCoroutine coroutine = new EditorCoroutine(_routine);
            coroutine.Start();
            return coroutine;
        }
        readonly IEnumerator routine;
        EditorCoroutine(IEnumerator _routine)
        {
            routine = _routine;
        }

        void Start()
        {
            EditorApplication.update += Update;
        }
        public void Stop()
        {
            EditorApplication.update -= Update;
        }
        void Update()
        {
            if (!routine.MoveNext())
            {
                Stop();
            }
        }
    }
}