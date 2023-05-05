using UnityEngine;
using System.Collections.Generic;
using System;

[CreateAssetMenu(fileName = "ShaderStrippingSettingsAsset", menuName = "Custom Tools/Shader Stripping/Settings asset", order = 1)]
public class ShaderStrippingSettingsAsset : ScriptableObject
{
    public List<ShaderStrippingInfo> m_StrippingInfo;

    public bool m_GenerateShaderVariantCollection;
    public string m_ReportPath = "Reports/";
    public string m_ReportFileName = "ShaderBuildReport";

    public bool m_GenerateReport;
    public string m_ShaderVariantPath = "ShaderVariants/";
    public string m_ShaderVariantName = "ShaderVariantFromBuild";

    public bool m_UseDateTimeForNaming;

    public void Reset()
    {
        m_StrippingInfo = new()
        {
            new() { Target = StrippingTarget.ShaderKeyword, Option = StrippingOption.Remove, Title = "Shader keywords to Strip" },
            new() { Target = StrippingTarget.ShaderAsset, Option = StrippingOption.Remove, Title = "Strip out shader with name" },
            new() { Target = StrippingTarget.ShaderKeyword, Option = StrippingOption.Track, Title = "Shader keywords to Track" },
            new() { Target = StrippingTarget.ShaderAsset, Option = StrippingOption.Track, Title = "Shader asset to Track" }
        };
    }

    public int GetPriority(StrippingTarget target, StrippingOption option)
    {
        // Iterates over all the shader stripping options.
        // If it finds an element with the same target and option value, returns the priority based on the list index.
        for (int i = 0; i < m_StrippingInfo.Count; i++)
        {
            if (m_StrippingInfo[i].Target == target && m_StrippingInfo[i].Option == option)
                return i;
        }
        return -1;
    }

    public int GetMaxPriority()
    {
        return m_StrippingInfo.Count;
    }

    public ShaderStrippingInfo GetStrippingInfo(StrippingTarget target, StrippingOption option)
    {
        for (int i = 0; i < m_StrippingInfo.Count; i++)
        {
            if (m_StrippingInfo[i].Target == target && m_StrippingInfo[i].Option == option)
                return m_StrippingInfo[i];
        }
        return null;
    }
}
