#if UNITY_2018_1_OR_NEWER && UNITY_ANDROID

using System.IO;
using System.Text;
using System.Xml;
using UnityEditor;
using UnityEditor.Android;
using UnityEngine;

public class AdfurikunManifestModify : IPostGenerateGradleAndroidProject {

    private string m_manifestFilePath;

    public void OnPostGenerateGradleAndroidProject(string basePath) {
        var androidManifest = new AndroidManifest(GetManifestPath(basePath));
        androidManifest.SetActivityHardwareAccelerated("true");
        androidManifest.Save();
    }

    public int callbackOrder {
        get {
            return 1; 
        } 
    }

    private string GetManifestPath(string basePath)
    {
        if (string.IsNullOrEmpty(m_manifestFilePath))
        {
            var pathBuilder = new StringBuilder(basePath);
            pathBuilder.Append(Path.DirectorySeparatorChar).Append("src");
            pathBuilder.Append(Path.DirectorySeparatorChar).Append("main");
            pathBuilder.Append(Path.DirectorySeparatorChar).Append("AndroidManifest.xml");
            m_manifestFilePath = pathBuilder.ToString();
        }
        return m_manifestFilePath;
    }
}


internal class AndroidXmlDocument : XmlDocument {
    private string m_Path;
    protected XmlNamespaceManager m_xmlNamespaceManager;
    public readonly string AndroidXmlNamespace = "http://schemas.android.com/apk/res/android";
    public AndroidXmlDocument(string path) {
        m_Path = path;
        using (var reader = new XmlTextReader(m_Path)) {
            reader.Read();
            Load(reader);
        }
        m_xmlNamespaceManager = new XmlNamespaceManager(NameTable);
        m_xmlNamespaceManager.AddNamespace("android", AndroidXmlNamespace);
    }

    public string Save() {
        return SaveAs(m_Path);
    }

    public string SaveAs(string path) {
        using (var writer = new XmlTextWriter(path, new UTF8Encoding(false))) {
            writer.Formatting = Formatting.Indented;
            Save(writer);
        }
        return path;
    }
}

internal class AndroidManifest : AndroidXmlDocument {

    private readonly XmlElement ApplicationElement;

    public AndroidManifest(string path) : base(path) {
        ApplicationElement = SelectSingleNode("/manifest/application") as XmlElement;
    }

    private XmlAttribute CreateAndroidAttribute(string key, string value) {
        XmlAttribute attr = CreateAttribute("android", key, AndroidXmlNamespace);
        attr.Value = value;
        return attr;
    }

    internal XmlNode GetActivityWithLaunchIntent() {
        return SelectSingleNode("/manifest/application/activity[intent-filter/action/@android:name='android.intent.action.MAIN' and " +
                "intent-filter/category/@android:name='android.intent.category.LAUNCHER']", m_xmlNamespaceManager);
    }

    internal void SetActivityHardwareAccelerated(string value) {
        XmlNode activityXmlNode = GetActivityWithLaunchIntent();
        if (activityXmlNode != null) {
            activityXmlNode.Attributes.Append(CreateAndroidAttribute("hardwareAccelerated", value));
        }
    }
}

#endif