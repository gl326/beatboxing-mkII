using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Linq;
using System;

public class UniqueReferenceDrawer<T,U> : PropertyDrawer 
	where T : Settings<T>
	where U : UniqueObject
{
	
	static Dictionary< Type, FieldInfo > fieldCache = new Dictionary<Type, FieldInfo>();
	
	public override void OnGUI( Rect position, SerializedProperty prop, GUIContent label )
	{
		Color prev = GUI.color;
		position = EditorGUI.PrefixLabel( position, GUIUtility.GetControlID( FocusType.Passive ), label );
		
		T settings = Settings<T>.Instance;
			
		if( settings == null )
		{
			EditorGUI.LabelField( position, "No " + typeof(T).Name + "!" );
			return;
		}

		FieldInfo field = GetField();
		
		if( field == null )
		{
			if( EditorGUIUtility.systemCopyBuffer == "[UniqueReference( typeof(" +typeof(U)+") )]" )
			{
				GUI.enabled = false;
				GUI.Button( position, "...paste into " + typeof(T) );
				GUI.enabled = true;
			} else {
				if( GUI.Button( position, "Click to copy code..." ) )
				{	
					EditorGUIUtility.systemCopyBuffer = "[UniqueReference( typeof(" +typeof(U)+") )]";
				}
			}
			return;
		}
		
		
		if( prop.hasMultipleDifferentValues ) GUI.color = Color.grey;
		
		List<U> list = field.GetValue( settings ) as List<U>;
		
		if( list == null ) return;
		
		int indent = EditorGUI.indentLevel;
		EditorGUI.indentLevel = 0;
		
		
		Rect rect = new Rect( position.x, position.y, position.width, position.height );
		
		string[] names = GetNames( list );
		int objectId = 0;
		SerializedProperty idProp = prop.FindPropertyRelative("id");

		U obj = GetObjectById( list, idProp.stringValue );
		
		if( obj != null )
		{
			objectId = System.Array.IndexOf( names, obj.ToString() );
			if( objectId < 0 ) objectId = 0;
		}
		
		objectId = EditorGUI.Popup( rect, objectId, names );
		idProp.stringValue = objectId > 0 ? list[objectId - 1].guid : "";

		EditorGUI.indentLevel = indent;
		
		GUI.color = prev;
	}
	
	
	string[] GetNames( List<U> list )
	{
		List<string> sn = list.Select( x => x.ToString() ).ToList();
		sn.Insert(0, "Not Selected");
		return sn.ToArray();
	}
	
	U GetObjectById( List<U> list, string id )
	{
		for( int i = 0; i < list.Count; i++ )
		{
			if( list[i].guid == id )
			{
				return list[i];
			}
		}
		
		return default(U);
	}
	
	static FieldInfo GetField()
	{
		FieldInfo field = null;
		
		if( !fieldCache.TryGetValue( typeof(U), out field ) )
		{
			List<FieldInfo> fields = typeof(T).GetFields()
				.Where( x => x.IsDefined( typeof(UniqueReferenceAttribute), false ) ).ToList();
			
			for( int i = 0; i < fields.Count; i++ )
			{
				UniqueReferenceAttribute attribute = (UniqueReferenceAttribute)System.Attribute.GetCustomAttribute( fields[i], typeof(UniqueReferenceAttribute) );
				if( attribute.Type == typeof(U) )
				{
					field = fields[i];
					break;
				}
			}
			
			fieldCache[ typeof(U) ] = field;
		}
		
		
		return field;
		
	}

}