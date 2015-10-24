using GTUtils;

using UnityEditor;

using UnityEngine;

[CustomPropertyDrawer(typeof(BitMaskAttribute))]
public class EnumFlagsAttributeDrawer : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property,
        GUIContent label)
    {
        var buttonsIntValue = 0;
        var enumLength = property.enumNames.Length;
        var buttonPressed = new bool[enumLength];
        var buttonWidth = (position.width - EditorGUIUtility.labelWidth) /
                          enumLength;

        EditorGUI.LabelField(
            new Rect(position.x, position.y, EditorGUIUtility.labelWidth,
                position.height), label);

        EditorGUI.BeginChangeCheck();

        for ( var index = 0; index < enumLength; index++ )
        {
            // Check if the button is/was pressed 
            if ( (property.intValue & (1 << index)) == 1 << index )
            {
                buttonPressed[index] = true;
            }

            var buttonPos =
                new Rect(
                    position.x + EditorGUIUtility.labelWidth +
                    buttonWidth * index,
                    position.y, buttonWidth,
                    position.height);

            buttonPressed[index] = GUI.Toggle(buttonPos, buttonPressed[index],
                property.enumNames[index], "Button");

            if ( buttonPressed[index] )
            {
                buttonsIntValue += 1 << index;
            }
        }

        if ( EditorGUI.EndChangeCheck() )
        {
            property.intValue = buttonsIntValue;
        }
    }
}