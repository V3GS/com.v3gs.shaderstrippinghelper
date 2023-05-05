using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class ShadersCompilationReport : ScriptableObject
{
    [SerializeField]
    private List<ShaderInfo> m_ShadersCompiled = new List<ShaderInfo>();

    public List<ShaderInfo> ShadersCompiled
    {
        get => m_ShadersCompiled;
        set => m_ShadersCompiled = value;
    }

    public void AddShaderCompiled(ShaderInfo shaderInfo)
    {
        m_ShadersCompiled.Add(shaderInfo);
    }

    public void Clone(ShadersCompilationReport report)
    {
        this.ShadersCompiled = report.ShadersCompiled;
    }
}
