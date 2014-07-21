using UnityEngine;
using UnityEditor;
using System.Collections;

[CustomPropertyDrawer( typeof(GenericVariable) )]
public class GenericVariableDrawer : PropertyDrawer {


	public override void OnGUI( Rect position, SerializedProperty prop, GUIContent label )
	{
		position = EditorGUI.PrefixLabel( position, GUIUtility.GetControlID( FocusType.Passive ), label );
		
		Rect enumRect = position;
		enumRect.width = 80;
		position.width -= enumRect.width;
		position.x += enumRect.width;
		
		EditorGUI.PropertyField( enumRect, prop.FindPropertyRelative("type"), GUIContent.none );
		GenericVariableType type = (GenericVariableType)prop.FindPropertyRelative("type").enumValueIndex;
		
		switch( type )
		{
			case GenericVariableType.String:
				EditorGUI.PropertyField( position, prop.FindPropertyRelative("stringValue"), GUIContent.none );
			break;
			
			case GenericVariableType.Int:
				EditorGUI.PropertyField( position, prop.FindPropertyRelative("intValue"), GUIContent.none );
			break;
			
			case GenericVariableType.Bool:
				EditorGUI.PropertyField( position, prop.FindPropertyRelative("boolValue"), GUIContent.none );
			break;
			
			case GenericVariableType.Float:
				EditorGUI.PropertyField( position, prop.FindPropertyRelative("floatValue"), GUIContent.none );
			break;
			
			case GenericVariableType.Range:
				position.width /= 3;
			
				EditorGUI.PropertyField( position, prop.FindPropertyRelative("floatValue"), GUIContent.none );
				position.x += position.width;
				EditorGUI.PropertyField( position, prop.FindPropertyRelative("minValue"), GUIContent.none );
				position.x += position.width;
				EditorGUI.PropertyField( position, prop.FindPropertyRelative("maxValue"), GUIContent.none );

				//data.floatValue = Mathf.Clamp( data.floatValue, data.minValue, data.maxValue );
			break;
		}
		
	}
	
	
}
