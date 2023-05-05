using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;
using UnityEditor.Rendering;
using UnityEngine;
using UnityEngine.Rendering;

public class GenerateShaderCompilationReport : IPreprocessShaders, IPostprocessBuildWithReport
{
	public int callbackOrder
	{
		get
		{
			// Returns the maximum priority based on the amount of elements in list from the ShaderStrippingSettingsAsset.
			return ShaderStrippingSettings_Provider.GetMaxPriority();
		}
	}

	private ShadersCompilationReport m_Report = new ShadersCompilationReport();

    public void OnProcessShader(Shader shader, ShaderSnippetData snippet, IList<ShaderCompilerData> datas)
    {
		ShaderInfo shaderInfo = new ShaderInfo()
		{
			Name = shader.name,
			PassName = snippet.passName,
			PassType = snippet.passType,
			ShaderType = snippet.shaderType,
			Count = datas.Count
		};

		foreach (ShaderCompilerData data in datas)
		{
			shaderInfo.ShaderCompilerPlatform = data.shaderCompilerPlatform;

			foreach (ShaderKeyword keyword in data.shaderKeywordSet.GetShaderKeywords())
			{
				shaderInfo.AddKeyword(keyword.name, true);
			}
		}

		m_Report.AddShaderCompiled(shaderInfo);
	}
    
    public void OnPostprocessBuild(BuildReport report)
    {
		if (ShaderStrippingSettings_Provider.IsShaderStrippingAssetSetup())
		{
			// Retrieve the current shader stripping settings asset
			ShaderStrippingSettingsAsset settingsAsset = ShaderStrippingSettings_Provider.ShaderStrippingSettings.shaderStrippingSettingsAsset;

			if (settingsAsset.m_GenerateReport)
			{
				ShadersCompilationReport m_ReportScriptableObject = ScriptableObject.CreateInstance<ShadersCompilationReport>();
				string timestamp = "";
				if (settingsAsset.m_UseDateTimeForNaming)
				{
					timestamp = "_" + DateTime.Now.ToString("yyyyMdd_HHmmss");
				}

				GenerateReport("Assets/" + settingsAsset.m_ReportPath, settingsAsset.m_ReportFileName + timestamp, ref m_ReportScriptableObject);

				if (settingsAsset.m_GenerateShaderVariantCollection)
				{
					GenerateSVC("Assets/" + settingsAsset.m_ShaderVariantPath, settingsAsset.m_ShaderVariantName + timestamp, ref m_ReportScriptableObject);
				}

				AssetDatabase.SaveAssets();
				AssetDatabase.Refresh();
			}
		}
    }

    void GenerateReport(string reportPath, string reportName, ref ShadersCompilationReport m_ReportScriptableObject)
	{
		string fullPath = Path.Combine(reportPath, reportName + ".asset");

		// If the folders structure don't exist, create them
		if (!Directory.Exists(reportPath))
		{
			Directory.CreateDirectory(reportPath);
		}

		m_ReportScriptableObject.Clone(m_Report);

		AssetDatabase.CreateAsset(m_ReportScriptableObject, fullPath);

		//string jsonstr = JsonUtility.ToJson(m_Report);
		//System.IO.File.WriteAllText("Assets/ShadersCompilationReport.json", jsonstr);
		Debug.Log($"Report file successfully created at: { fullPath }");
	}

	private void GenerateSVC(string shaderVariantPath, string m_ShaderVariantName, ref ShadersCompilationReport m_ReportScriptableObject)
	{
		string fullPath = Path.Combine(shaderVariantPath, m_ShaderVariantName + ".shadervariants");
		ShaderVariantCollection svcFile = new ShaderVariantCollection();

		// If the folders structure don't exist, create them
		if (!Directory.Exists(shaderVariantPath))
		{
			Directory.CreateDirectory(shaderVariantPath);
		}

		for (int i = 0; i < m_ReportScriptableObject.ShadersCompiled.Count; i++)
		{
			// Bring back the information from the report
			ShaderInfo shaderInfo = m_ReportScriptableObject.ShadersCompiled[i];

			// Find the shader by the shader name retrieve in the report
			Shader shader = Shader.Find(shaderInfo.Name);

			// Iterate over all the shader keywords for creating the variant
			for (int j = 0; j < shaderInfo.Keywords.Count; j++)
			{
				// Create a shader variant based on the information of the shader compilation report
				ShaderVariantCollection.ShaderVariant variant = new ShaderVariantCollection.ShaderVariant(shader, shaderInfo.PassType, shaderInfo.Keywords[j]);
				// Add the variant to the shader variant collection file
				svcFile.Add(variant);
			}
		}

		AssetDatabase.CreateAsset(svcFile, fullPath);

		Debug.Log($"Shader Variant Collection file successfully created at: { fullPath }");
	}
}
