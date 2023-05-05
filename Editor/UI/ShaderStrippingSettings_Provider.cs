using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

class ShaderStrippingSettings_Provider : SettingsProvider
{
    // Reference to the scriptable object
    private static SerializedObject m_ShaderStrippingSettings;

    // Getter to the ShaderStrippingSettings
    public static ShaderStrippingSettings ShaderStrippingSettings
    {
        get { return (ShaderStrippingSettings)m_ShaderStrippingSettings.targetObject; }
    }

    public static int GetStrippingPriority(StrippingTarget shaderAsset, StrippingOption option)
    {
        if (IsShaderStrippingAssetSetup())
        {
            return ShaderStrippingSettings.shaderStrippingSettingsAsset.GetPriority(shaderAsset, option);
        }
        return 0;
    }

    public static int GetMaxPriority()
    {
        if (IsShaderStrippingAssetSetup())
        {
            return ShaderStrippingSettings.shaderStrippingSettingsAsset.GetMaxPriority();
        }
        return 0;
    }

    public static ShaderStrippingInfo GetStrippingInfo(StrippingTarget target, StrippingOption option)
    {
        return ShaderStrippingSettings.shaderStrippingSettingsAsset.GetStrippingInfo(target, option);
    }

    class Styles
    {
        public static GUIContent strippingAsset         = new GUIContent("Shader stripping asset");
        public static GUIContent strippingLogOptions    = new GUIContent("Shader stripping log options");
        public static GUIContent trackLogColor           = new GUIContent("Track element - Log color");
        public static GUIContent removeLogColor         = new GUIContent("Remove element - Log color");
    }

    public ShaderStrippingSettings_Provider(string path, SettingsScope scope = SettingsScope.Project) : base(path, scope) { }

    public override void OnActivate(string searchContext, VisualElement rootElement)
    {
        // This function is called when the user activates the Setting provider in Project settings
        m_ShaderStrippingSettings = ShaderStrippingSettings.GetSerializedSettings();
    }

    public override void OnGUI(string searchContext)
    {
        // Title spacing
        GUILayout.Space(10);

        // Elements spacing
        GUILayout.BeginHorizontal();
        GUILayout.Space(10);

        // Draw UI
        GUILayout.BeginVertical();

        // Keep track if an element has changed
        using (var hasChanged = new EditorGUI.ChangeCheckScope())
        {
            EditorGUIUtility.labelWidth = 256f;

            EditorGUILayout.PropertyField(m_ShaderStrippingSettings.FindProperty("m_ShaderStrippingSettingsAsset"), Styles.strippingAsset);
            EditorGUILayout.PropertyField(m_ShaderStrippingSettings.FindProperty("m_ShaderStrippingLog"), Styles.strippingLogOptions);
            EditorGUILayout.PropertyField(m_ShaderStrippingSettings.FindProperty("m_TrackColor"), Styles.trackLogColor);
            EditorGUILayout.PropertyField(m_ShaderStrippingSettings.FindProperty("m_RemoveColor"), Styles.removeLogColor);

            // If a property field has changed, then apply those changes to the scriptable object
            if (hasChanged.changed)
            {
                m_ShaderStrippingSettings?.ApplyModifiedProperties();
            }
        }

        GUILayout.EndVertical();
        GUILayout.EndHorizontal();
    }

    [SettingsProvider]
    public static SettingsProvider CreateMyCustomSettingsProvider()
    {
        if (!IsSettingsAvailable())
        {
            ShaderStrippingSettings.GetSerializedSettings();
        }

        // Add the provider to the Project window using the string
        var provider = new ShaderStrippingSettings_Provider("Custom tools/Graphics/ShaderStrippingHelper", SettingsScope.Project)
        {
            label = "Shader Stripping Settings"
        };

        // Automatically extract all keywords from the Styles.
        provider.keywords = GetSearchKeywordsFromGUIContentProperties<Styles>();
        return provider;
    }

    // Returns whether the ShaderStrippingAsset is correctly set up in the provider
    public static bool IsShaderStrippingAssetSetup()
    {
        return ShaderStrippingSettings.shaderStrippingSettingsAsset != null;
    }

    private static bool IsSettingsAvailable()
    {
        return File.Exists(ShaderStrippingSettings.GetPath());
    }
}

