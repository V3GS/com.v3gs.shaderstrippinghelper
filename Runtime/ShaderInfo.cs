using System.Collections;
using System.Collections.Generic;
using UnityEditor.Rendering;
using UnityEngine;
using UnityEngine.Rendering;

[System.Serializable]
public class ShaderInfo
{
	[SerializeField]
	// Shader name
	private string m_Name;
	[SerializeField]
	// Pass name
	private string m_PassName;
	[SerializeField]
	// Pass type
	private PassType m_PassType;
	[SerializeField]
	// Shader type
	private ShaderType m_ShaderType;
	[SerializeField]
	// Shader compiler API Platform
	private ShaderCompilerPlatform m_ShaderCompilerPlatform;

	[SerializeField]
	// Data count (variants)
	private int m_Count;
	[SerializeField]
	// Keywords
	private List<string> m_Keywords = new List<string>();


    public string Name { get => m_Name; set => m_Name = value; }
    public string PassName { get => m_PassName; set => m_PassName = value; }
    public PassType PassType { get => m_PassType; set => m_PassType = value; }
	public ShaderType ShaderType { get => m_ShaderType; set => m_ShaderType = value; }
	public ShaderCompilerPlatform ShaderCompilerPlatform { get => m_ShaderCompilerPlatform; set => m_ShaderCompilerPlatform = value; }
	public int Count { get => m_Count; set => m_Count = value; }
    public List<string> Keywords { get => m_Keywords; set => m_Keywords = value; }

	public void AddKeyword(string keyword, bool avoidDuplicatedKeyword)
	{
		if (keyword == "") return;
		
		if (avoidDuplicatedKeyword)
		{
			for (int i = 0; i < m_Keywords.Count; i++)
			{
				if (m_Keywords[i] == keyword)
					return;
			}
			m_Keywords.Add(keyword);
		}
		else
		{
			m_Keywords.Add(keyword);
		}
	}

	public void ClearAllKeywords()
	{
		m_Keywords.Clear();
	}

	public override string ToString()
	{
		return ($"Name: {m_Name} - Pass name: {m_PassName} - Pass type: {m_PassType.ToString()} | Shader type: { m_ShaderType.ToString() } | Keywords: {GetKeywordsString()}");
	}

	private string GetKeywordsString()
	{
		string keywords = "";
		for (int i = 0; i < m_Keywords.Count; i++)
		{
			if (i < m_Keywords.Count - 1)
				keywords += m_Keywords[i] + ", ";
			else
				keywords += m_Keywords[i];
		}

		return keywords;
	}
}
