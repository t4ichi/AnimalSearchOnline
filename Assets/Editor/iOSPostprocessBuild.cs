using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
#if UNITY_IOS
using UnityEditor;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;
using UnityEditor.iOS.Xcode;

public class iOSPostprocessBuild : IPostprocessBuildWithReport
{
    public int callbackOrder { get { return 0; } }

    public void OnPostprocessBuild(BuildReport report)
    {
        if (report.summary.platform == BuildTarget.iOS)
        {
            string path = Path.Combine(report.summary.outputPath, "Info.plist");
            PlistDocument plist = new PlistDocument();
            plist.ReadFromFile(path);
            plist.root.SetString("NSPhotoLibraryUsageDescription", "ここに説明文");
            plist.root.SetString("NSPhotoLibraryAddUsageDescription", "ここに説明文");
            plist.WriteToFile(path);
        }
    }
}
#endif