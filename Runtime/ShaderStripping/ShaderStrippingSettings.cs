using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

[Flags]
public enum ShaderStrippingLog
{
    None = 0,
    Track = 1,
    Remove = 2,
}

public class ShaderStrippingSettings : ScriptableObject
{
    private const string k_StrippingShaderSettingsPath = "Assets/Settings/ShaderStrippingHelper";
    private const string k_StrippingShaderSettingsFileName = "ShaderStrippingSettings.asset";

    [SerializeField]
    private ShaderStrippingSettingsAsset m_ShaderStrippingSettingsAsset;
    [SerializeField]
    private ShaderStrippingLog m_ShaderStrippingLog = ShaderStrippingLog.None;
    [SerializeField]
    private Color m_KeepColor = new Color32(255, 165, 0, 255); // Orange color
    [SerializeField]
    private Color m_RemoveColor = new Color32(255, 37, 0, 255); // Red color
    [SerializeField]
    private Color m_TrackColor = new Color32(129, 182, 34, 255); // Green color

    // Properties
    public ShaderStrippingSettingsAsset shaderStrippingSettingsAsset => m_ShaderStrippingSettingsAsset;
    public ShaderStrippingLog shaderStrippingLog => m_ShaderStrippingLog;
    public Color keepColor => m_KeepColor;
    public Color removeColor => m_RemoveColor;
    public Color trackColor => m_TrackColor;

    public static SerializedObject GetSerializedSettings()
    {
        return new SerializedObject(GetOrCreateSettings());
    }

    public static string GetPath()
    {
        return Path.Combine(k_StrippingShaderSettingsPath, k_StrippingShaderSettingsFileName);
    }

    private static ShaderStrippingSettings GetOrCreateSettings()
    {
        // If the folders structure don't exist, create them
        if (!Directory.Exists(k_StrippingShaderSettingsPath))
        {
            Directory.CreateDirectory(k_StrippingShaderSettingsPath);
        }

        string fullPath = GetPath();

        // It tries to load a settings file, but if it doesn't exist, create one with default values
        var settings = AssetDatabase.LoadAssetAtPath<ShaderStrippingSettings>(fullPath);
        if (settings == null)
        {
            settings = ScriptableObject.CreateInstance<ShaderStrippingSettings>();
            AssetDatabase.CreateAsset(settings, fullPath);
            AssetDatabase.SaveAssets();

            Debug.Log($"A new shader stripping settings file has been created at: { fullPath }", AssetDatabase.LoadMainAssetAtPath(fullPath));
        }
        return settings;
    }
}
