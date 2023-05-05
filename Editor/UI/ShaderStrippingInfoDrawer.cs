using System;
using UnityEngine;
using UnityEditor;
using UnityEngine.UIElements;
using UnityEditor.UIElements;
using UnityEngine.Rendering;

[CustomPropertyDrawer(typeof(ShaderStrippingInfo))]
public class ShaderStrippingInfoDrawer : PropertyDrawer
{
    private VisualElement m_Inspector;
    private const string k_ValuesContainerName = "m_Values";

    public override VisualElement CreatePropertyGUI(SerializedProperty property)
    {
        int orderInArray = GetOrderInArray(property.propertyPath);
        StrippingTarget target = (StrippingTarget)property.FindPropertyRelative("m_Target").intValue;
        StrippingOption option = (StrippingOption)property.FindPropertyRelative("m_Option").intValue;

        m_Inspector = new VisualElement();

        switch (target)
        {
            case StrippingTarget.ShaderKeyword:
                m_Inspector = DrawShaderKeywordUI(orderInArray, property);
                break;
            case StrippingTarget.ShaderAsset:
                m_Inspector = DrawShaderAssetUI(orderInArray, property);
                break;
        };


        return m_Inspector;
    }

    private int GetOrderInArray(string propertyPath)
    {
        string orderInArray = propertyPath.Remove(propertyPath.Length - 1); // Remove ']'
        orderInArray = orderInArray.Remove(0, orderInArray.IndexOf('[') + 1); // Remove until found the '[' character

        return Convert.ToInt32(orderInArray);
    }

    private VisualElement DrawShaderAssetUI(int orderInArray, SerializedProperty property)
    {
        VisualElement shaderAssetContainer = new VisualElement();
        shaderAssetContainer.style.marginTop = 15;
        shaderAssetContainer.style.marginBottom = 15;

        VisualElement ve_Title = GenerateTitleUI(orderInArray, property.FindPropertyRelative("m_Title").stringValue);
        Foldout fdo_shaderInfo = new Foldout() { text = "Shader info", value = false };
        PropertyField pf_Values = new PropertyField(property.FindPropertyRelative(k_ValuesContainerName));

        // Create an ObjectField and assign its respective callback
        ObjectField of_ShaderAsset = new ObjectField("Shader") { objectType = typeof(Shader) };
        of_ShaderAsset.RegisterValueChangedCallback((evt) =>
            {
                // If a shader asset has been selected
                if (evt.newValue)
                {
                    AddNewValue_Evt(property, ((Shader)evt.newValue).name);
                }
            }
        );

        fdo_shaderInfo.Add(of_ShaderAsset);
        fdo_shaderInfo.Add(pf_Values);

        shaderAssetContainer.Add(ve_Title);
        shaderAssetContainer.Add(fdo_shaderInfo);

        return shaderAssetContainer;
    }

    private VisualElement DrawShaderKeywordUI(int orderInArray, SerializedProperty property)
    {
        VisualElement shaderKeywordContainer = new VisualElement();
        shaderKeywordContainer.style.marginTop = 15;
        shaderKeywordContainer.style.marginBottom = 15;

        #region Global keywords UI
        // Create a Foldout to store all the global keywords
        // The fouldout is shrunk since the beginning because it might contain too many shader keywords
        Foldout fdo_globalKeywords = new Foldout() { text = "Global keywords", value = false };

        VisualElement ve_globalKeywordsContainer = new VisualElement();
        // Change the style for showing the buttons in a row
        ve_globalKeywordsContainer.style.flexDirection = FlexDirection.Row;
        ve_globalKeywordsContainer.style.flexWrap = Wrap.Wrap;

        AddGlobalKeywordsToContainer(ve_globalKeywordsContainer, property);

        fdo_globalKeywords.Add(ve_globalKeywordsContainer);
        #endregion

        #region Local keywords UI
        Foldout fdo_localKeywords = new Foldout() { text = "Local keywords" };

        VisualElement ve_localKeywordsContainer = new VisualElement();
        // Change the style for showing the buttons in a row
        ve_localKeywordsContainer.style.flexDirection = FlexDirection.Row;
        ve_localKeywordsContainer.style.flexWrap = Wrap.Wrap;

        ObjectField of_ShaderAsset = new ObjectField("Shader") { objectType = typeof(Shader) };

        of_ShaderAsset.RegisterValueChangedCallback((evt) => { OnSubFieldValueChange(evt, ve_localKeywordsContainer, property); });

        fdo_localKeywords.Add(of_ShaderAsset);
        fdo_localKeywords.Add(ve_localKeywordsContainer);

        #endregion

        VisualElement ve_Title = GenerateTitleUI(orderInArray, property.FindPropertyRelative("m_Title").stringValue);

        // The main foldout is shrunk because it's up to the user to use this UI
        Foldout fdo_keywordsAnalyzer = new Foldout() { text = "Keywords analyzer", value = false };
        PropertyField pf_Values = new PropertyField(property.FindPropertyRelative(k_ValuesContainerName));

        // Add elements to the main foldout
        fdo_keywordsAnalyzer.Add(fdo_globalKeywords);
        fdo_keywordsAnalyzer.Add(fdo_localKeywords);
        fdo_keywordsAnalyzer.Add(pf_Values);

        // Add elements that belong to the item
        shaderKeywordContainer.Add(ve_Title);
        shaderKeywordContainer.Add(fdo_keywordsAnalyzer);

        return shaderKeywordContainer;
    }

    private VisualElement GenerateTitleUI(int orderInArray, string title)
    {
        VisualElement ve_Title = new VisualElement();
        ve_Title.style.flexDirection = FlexDirection.Row;
        ve_Title.style.justifyContent = Justify.SpaceBetween;

        Label lb_Title = new Label($"<b>{title}</b>");
        Label lb_Priority = new Label($"Priority { orderInArray }");
        lb_Priority.style.backgroundColor = new StyleColor(new Color(0.28f, 0.28f, 0.28f, 1));
        lb_Priority.style.borderTopLeftRadius = lb_Priority.style.borderTopRightRadius = lb_Priority.style.borderBottomLeftRadius = lb_Priority.style.borderBottomRightRadius = 8.0f;
        lb_Priority.style.paddingLeft = lb_Priority.style.paddingRight = 10;

        ve_Title.Add(lb_Title);
        ve_Title.Add(lb_Priority);

        return ve_Title;
    }

    private void AddGlobalKeywordsToContainer(VisualElement ve_globalKeywordsContainer, SerializedProperty property)
    {
        // Get all global keywords
        GlobalKeyword[] globalKeywords = Shader.globalKeywords;

        // Create a new button per keyword
        foreach (var globalKeyword in globalKeywords)
        {
            // When the shader keyword button is created, register its callback and add it to the container
            Button btn_Keyword = new Button() { text = globalKeyword.name };

            btn_Keyword.RegisterCallback<ClickEvent>((evt) => { AddNewValue_Evt(property, globalKeyword.name); });

            ve_globalKeywordsContainer.Add(btn_Keyword);
        }
    }

    private void OnSubFieldValueChange(ChangeEvent<UnityEngine.Object> evt, VisualElement localKeywordsContainer, SerializedProperty property)
    {
        // Clean up the keywords container when the Shader selected changes
        localKeywordsContainer.Clear();

        // Retrieve the current shader selected
        Shader currentShader = (Shader)evt.newValue;

        // If the shader exist, then retrieve all its local keywords and create buttons in the process
        if (currentShader)
        {
            LocalKeyword[] localKeywords = currentShader.keywordSpace.keywords;

            foreach (var localKeyword in localKeywords)
            {
                // When the shader keyword button is created, register its callback and add it to the container
                Button btn_Keyword = new Button() { text = localKeyword.name };
                btn_Keyword.RegisterCallback<ClickEvent>((evt) => { AddNewValue_Evt(property, localKeyword.name); });

                localKeywordsContainer.Add(btn_Keyword);
            }
        }
    }

    private void AddNewValue_Evt(SerializedProperty property, string newKeyword)
    {
        // Obtain the container list and add a new value at the end of it.
        SerializedProperty keywords = property.FindPropertyRelative(k_ValuesContainerName);
        keywords.InsertArrayElementAtIndex(keywords.arraySize);
        keywords.GetArrayElementAtIndex(keywords.arraySize - 1).stringValue = newKeyword;

        property.serializedObject.ApplyModifiedProperties();
    }
}
