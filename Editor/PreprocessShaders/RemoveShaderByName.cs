using System.Collections.Generic;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;
using UnityEditor.Rendering;
using UnityEngine;

public class RemoveShaderByName : IPreprocessShaders, IPreprocessBuildWithReport
{
    private const StrippingTarget kTarget = StrippingTarget.ShaderAsset;
    private const StrippingOption kOption = StrippingOption.Remove;
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
        logColor = ColorUtility.ToHtmlStringRGB(ShaderStrippingSettings_Provider.ShaderStrippingSettings.removeColor);
    }

    public void OnProcessShader(Shader shader, ShaderSnippetData snippet, IList<ShaderCompilerData> data)
    {
        if (!ShaderStrippingSettings_Provider.IsShaderStrippingAssetSetup()) return;

        // Get the stripping info associated with the shaders that will be removed in the build
        ShaderStrippingInfo strippinInfo = ShaderStrippingSettings_Provider.GetStrippingInfo(kTarget, kOption);

        for (int i = 0; i < data.Count; ++i)
        {
            bool shouldRemove = false;

            // Iterate over all the shader names in the stripping info list
            foreach (string shaderName in strippinInfo.Values)
            {
                // If the shader name is found, then remove it
                if (shaderName == shader.name)
                {
                    shouldRemove = true;
                    break;
                }
            }

            if (shouldRemove)
            {
                LogMessage(shader.name);

                data.RemoveAt(i);
                --i;
            }
        }
    }

    void LogMessage(string shaderName)
    {
        if (ShaderStrippingSettings_Provider.ShaderStrippingSettings.shaderStrippingLog.HasFlag(ShaderStrippingLog.Remove))
        {
            Debug.Log($"<color=#{logColor}><b>Removed shader by name</b></color>: { shaderName }");
        }
    }
}
