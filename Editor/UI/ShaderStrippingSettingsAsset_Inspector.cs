using UnityEditor;
using UnityEngine.UIElements;

[CustomEditor(typeof(ShaderStrippingSettingsAsset))]
public class ShaderStrippingSettingsAsset_Inspector : Editor
{
    public VisualTreeAsset m_EditorXML;

    private VisualElement m_RootInspector;

    public override VisualElement CreateInspectorGUI()
    {
        m_RootInspector = m_EditorXML.CloneTree();

        return m_RootInspector;
    }
}
