using System;
using System.Collections.Generic;
using UnityEngine;
public enum StrippingTarget
{
    ShaderKeyword,
    ShaderAsset
}

public enum StrippingOption
{
    Keep,
    Remove,
    Track
}

[Serializable]
public class ShaderStrippingInfo
{
    [SerializeField]
    private string m_Title;

    [SerializeField]
    private StrippingTarget m_Target;

    [SerializeField]
    private StrippingOption m_Option;

    [SerializeField]
    private List<string> m_Values = new List<string>();

    public string Title
    {
        set { m_Title = value; }
        get { return m_Title; }
    }

    public StrippingTarget Target
    {
        set { m_Target = value; }
        get { return m_Target; }
    }

    public StrippingOption Option
    {
        set { m_Option = value; }
        get { return m_Option; }
    }

    public List<string> Values
    {
        set { m_Values = value; }
        get { return m_Values; }
    }
}


