using System.Collections;
using System.Collections.Generic;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;
using UnityEditor.Rendering;
using UnityEngine;
using UnityEngine.Rendering;

public class TrackShaderKeyword : IPreprocessShaders, IPreprocessBuildWithReport
{
    private const StrippingTarget kTarget = StrippingTarget.ShaderKeyword;
    private const StrippingOption kOption = StrippingOption.Track;
    private string logColor = "";

    public int callbackOrder
    {
        get
        {
            // Returns the priority based on the list order from the ShaderStrippingSettingsAsset.
            return ShaderStrippingSettings_Provider.GetStrippingPriority(kTarget, kOption);
        }
    }

    public void OnPreprocessBuild(BuildReport report)
    {
        // Retrieves the log color in this method for performing this operation once
        logColor = ColorUtility.ToHtmlStringRGB(ShaderStrippingSettings_Provider.ShaderStrippingSettings.trackColor);
    }

    public void OnProcessShader(Shader shader, ShaderSnippetData snippet, IList<ShaderCompilerData> datas)
    {
        if (!ShaderStrippingSettings_Provider.IsShaderStrippingAssetSetup()) return;

        // Get the stripping info associated with the shader keywords that will be removed in the build
        ShaderStrippingInfo strippinInfo = ShaderStrippingSettings_Provider.GetStrippingInfo(kTarget, kOption);

        for (int i = 0; i < datas.Count; ++i)
        {
            // Iterate over all the shader keywords in the stripping info list
            for (int j = 0; j < strippinInfo.Values.Count; j++)
            {
                // If the shader keyword is found, then remove it
                if (datas[i].shaderKeywordSet.IsEnabled(new ShaderKeyword(strippinInfo.Values[j])))
                {
                    LogMessage(strippinInfo.Values[j], shader.name);
                    break;
                }
            }
        }
    }
    void LogMessage(string shaderKeyword, string shaderName)
    {
        if (ShaderStrippingSettings_Provider.ShaderStrippingSettings.shaderStrippingLog.HasFlag(ShaderStrippingLog.Track))
        {
            Debug.Log($"<color=#{logColor}><b>Shader keyword tracked </b></color>: { shaderKeyword } in the { shaderName } Shader");
        }
    }
}
