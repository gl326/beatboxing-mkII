using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Linq.Expressions;
using System;

public class ComponentGUILayout {
	

	public static bool Foldout( UniqueObject obj)
	{
		return Foldout( "", obj, "" );
	}

	public static bool Foldout( string label, UniqueObject obj)
	{
		return Foldout( label, obj, "" );
	}
	
	public static bool Foldout( string label, UniqueObject obj, string unique)
	{
		return Foldout( new GUIContent(label), obj, unique );
	}
	
	public static bool Foldout( GUIContent label, UniqueObject obj, string unique)
	{
		bool foldout = EditorPrefs.GetBool( obj.guid + "_Foldout" + unique, false);
		
		foldout = EditorGUILayout.Foldout( foldout, label );
		
		EditorPrefs.SetBool( obj.guid + "_Foldout" + unique, foldout );
		return foldout;
	}
	
	
	public static bool EditableFoldout( ref string name, UniqueObject obj )
	{
		bool foldout = EditorPrefs.GetBool( obj.guid + "_Foldout", true);
		
		EditorGUILayout.BeginHorizontal();
		
		Rect rect = EditorGUILayout.GetControlRect( false, new GUILayoutOption[]{ GUILayout.Width(30), GUILayout.ExpandWidth(false) });
		foldout = EditorGUI.Foldout( rect, foldout, "" );


		name = EditorGUILayout.TextField( name );

		
		EditorGUILayout.EndHorizontal();
		
		EditorPrefs.SetBool( obj.guid + "_Foldout", foldout );
		return foldout;
	}
	
	public static void GenericVariableField( GenericVariable data )
	{
		GenericVariableField( GUIContent.none, data );
	}
	
	public static void GenericVariableField( string label, GenericVariable data )
	{
		GenericVariableField( new GUIContent(label), data );
	}
	
	public static void GenericVariableField( GUIContent label, GenericVariable data )
	{
		
		Rect rect = EditorGUILayout.GetControlRect(true);
		
		switch( data.type )
		{
			case GenericVariableType.String:
				data.stringValue = EditorGUI.TextField( rect, label, data.stringValue );
			break;
			
			case GenericVariableType.Int:
				data.intValue = EditorGUI.IntField( rect, label, data.intValue );
			break;
			
			case GenericVariableType.Bool:
				data.boolValue = EditorGUI.Toggle( rect, label, data.boolValue );
			break;
			
			case GenericVariableType.Float:
				data.floatValue = EditorGUI.FloatField( rect, label, data.floatValue );
			break;
			
			case GenericVariableType.Range:
				rect.width /= 3;
			
				data.floatValue = EditorGUI.FloatField( rect, label, data.floatValue );
				rect.x += rect.width;
				data.minValue = EditorGUI.FloatField( rect, label, data.minValue );
				rect.x += rect.width;
				data.maxValue = EditorGUI.FloatField( rect, label, data.maxValue );

				data.floatValue = Mathf.Clamp( data.floatValue, data.minValue, data.maxValue );
			break;
		}
		
	}
	
	
	public static bool AddRemoveButtons<T>( List<T> collection, int index ) where T : new()
	{
		if(GUILayout.Button("-", EditorStyles.miniButtonLeft, GUILayout.Width(20) ))
		{
			collection.RemoveAt( index );
			return false;
		}

		if(GUILayout.Button("+", EditorStyles.miniButtonRight, GUILayout.Width(20)) )
		{
			T obj = new T();
			if( obj is UniqueObject ) (obj as UniqueObject).Reset();
			collection.Insert( index + 1, obj );
			return true;
		}
		
		return false;
	}
	
	public static void ReorderableListItem( SerializedProperty prop, int index )
	{
		bool state = GUI.enabled;
		if( index == 0 ) GUI.enabled = false;
		
		if(GUILayout.Button("\u25B2", EditorStyles.miniButtonLeft, GUILayout.Width(20) ))
		{
			prop.MoveArrayElement( index, index - 1 );
			prop.serializedObject.ApplyModifiedProperties();
		}

		if( index == prop.arraySize - 1 ) GUI.enabled = false;
		else GUI.enabled = state && true;
		
		if(GUILayout.Button("\u25BC", EditorStyles.miniButtonRight, GUILayout.Width(20)) )
		{
			prop.MoveArrayElement( index, index + 1 );
			prop.serializedObject.ApplyModifiedProperties();
		}
		GUI.enabled = state;
		return;
	}
	
	public static void TwoDimensionalSlider( Rect rect, Vector2 pos, Vector2 min, Vector2 max )
    {
		rect.x += EditorGUI.indentLevel * 12;
		rect.width -= EditorGUI.indentLevel * 12;
		
		GUI.color = Color.black;
		GUI.Box( rect, "" );
		
		float x = ((pos.x - min.x) / (max.x - min.x)) * rect.width + rect.x;
		float y = (1 - (pos.y - min.y) / (max.y - min.y)) * rect.height + rect.y;
		
		Rect buttonRect = new Rect( x - 7, y - 7, 0, 0 );
		GUI.color = Color.white;
    	GUI.Button( buttonRect, "", GUI.skin.horizontalSliderThumb);
    }
	
    
    public static void Divider( )
    {
    	Divider( Color.white );
    }
    public static void Divider( Color color )
    {
    	Divider( EditorGUILayout.GetControlRect(true, 1), color );
    }
    
    public static void Divider( Rect rect, Color color )
    {
    	Color old = GUI.color;
    	GUI.color = color;
    	GUI.Box(rect, "");
    	GUI.color = old;
    }
    
    
    
    
    
    public static T AdvancedPopup<T>( Rect rect, T value, Expression<Func<T,string>> lambda )
    {
    	return value;
    }

}