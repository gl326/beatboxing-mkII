using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using Mono;
using System.Reflection;
using System.IO;
using System;

public class SettingsWindow : EditorWindow {

	public static string projectAssetPath = "Assets/resources/settings/";
	private Editor settingsEditor;
	private Vector2 scrollPos = Vector2.zero;
	private Type[] _managerTypes;
	private bool _shouldRefresh = false;
	
	private Dictionary<Type, Editor> _editors = new Dictionary<Type, Editor>();

	[MenuItem ("Window/Settings %g")]
    static void Init()
    {
        SettingsWindow window = (SettingsWindow)EditorWindow.GetWindow (typeof (SettingsWindow));
        window.title = "Settings";
        window.Show();
    }
	void OnInspectorUpdate()
    {
		Repaint();
    } 
    
    void Update()
    {
    	if(EditorApplication.isCompiling)
    	{
    		_shouldRefresh = true;
		} else if( _shouldRefresh )
		{
			RefreshList();
			_shouldRefresh = false;
		}
    }        
    
	void OnGUI ()
	{
		scrollPos = EditorGUILayout.BeginScrollView( scrollPos );
		
		EditorGUILayout.BeginVertical();
		
		GUIStyle style = new GUIStyle( EditorStyles.foldout );
		
		style.active = EditorStyles.boldLabel.active;
		style.font = EditorStyles.boldLabel.font;
		style.fontSize = EditorStyles.boldLabel.fontSize;
		style.fontStyle = EditorStyles.boldLabel.fontStyle;
		
		if( managerTypes.Length == 0 )
		{
			EditorGUILayout.HelpBox("No Settings in this project. Create a class that extends Settings to get started, like so:", MessageType.Info, true);
			EditorGUILayout.HelpBox("public class MySettings : Settings<MySettings>", MessageType.None, true);
		}
		
		foreach(Type type in managerTypes)
		{			
		GUI.enabled = true;
	
			GUILayout.Box("", new GUILayoutOption[]{GUILayout.ExpandWidth(true), GUILayout.Height(1)});
			bool isOpen = EditorPrefs.GetBool( "Foldout" + type.Name );
			isOpen = EditorGUILayout.Foldout( isOpen, Humanize( type.Name ), style );
			EditorPrefs.SetBool( "Foldout" + type.Name, isOpen );
			
			EditorGUI.indentLevel++;
			
			
			if( isOpen )
			{
				
				string path = projectAssetPath + type.Name + ".prefab";
				
				if( Application.isPlaying )
				{
					if( GUILayout.Button("Select " + type.Name + " in scene") )
					{
						GameObject obj = (FindObjectOfType( type ) as Component).gameObject;
						Selection.objects = new GameObject[]{ obj };
					}
				} 
				
				GUI.enabled = !Application.isPlaying;

				var instance = AssetDatabase.LoadAssetAtPath(path, type);			

				if( instance == null )
				{
					if( GUILayout.Button("Create " + type.Name + " prefab") )
					{
						CheckPath();
						UnityEngine.Object prefab = PrefabUtility.CreateEmptyPrefab( path );		
						GameObject go = new GameObject( type.Name );
						go.AddComponent( type.Name );
						PrefabUtility.ReplacePrefab( go, prefab, ReplacePrefabOptions.ConnectToPrefab );
						GameObject.DestroyImmediate( go );
					}
					
					Editor editor = null;
					
					if( _editors.TryGetValue( type, out editor ) )
					{
						DestroyImmediate( editor );
						_editors.Remove( type );
					}
					
				} else if ( instance != null ) {
					
					Editor editor = null;
					
					if( !_editors.TryGetValue( type, out editor ) )
					{
						editor = Editor.CreateEditor( instance );
						_editors[type] = editor;
					}
	
					editor.OnInspectorGUI();

				}
				
			}
			
			
			EditorGUIUtility.LookLikeControls();
			EditorGUI.indentLevel--;
		}
		
		EditorGUILayout.EndVertical();
		EditorGUILayout.EndScrollView();
	}
	
	
	void CheckPath()
	{
		if(!Directory.Exists( Application.dataPath + "/resources" ) )
		{
			AssetDatabase.CreateFolder("Assets", "resources");
		}
		
		if(!Directory.Exists( Application.dataPath + "/resources/settings" ) )
		{
			AssetDatabase.CreateFolder("Assets/resources", "settings");
		}
	}
	
	void RefreshList()
	{
		_managerTypes = GetAllSubTypes( typeof(Settings<>) );
	}
	
	
	public Type[] managerTypes
	{
		get
		{
			if( _managerTypes == null)
			{
				RefreshList();
			}
			
			return _managerTypes;
		}
	}
	
	string Humanize( string name )
	{
		if( name.IndexOf("Settings") >= 0 ) return name.Replace("Settings", "") + "s";
		else return name;
	}
	
	static Type[] GetAllSubTypes(Type aBaseClass)
	{
		List<Type> result = new List<Type>();
		Assembly[] AS = System.AppDomain.CurrentDomain.GetAssemblies();
		foreach (var A in AS)
		{
			Type[] types = A.GetTypes();
			foreach (Type T in types)
			{
				if (aBaseClass != T && IsSubclassOfRawGeneric(aBaseClass, T))
					result.Add(T);
			}
		}
		return result.ToArray();
	}
	
	static bool IsSubclassOfRawGeneric(Type generic, Type toCheck) {
		while (toCheck != null && toCheck != typeof(object)) {
			var cur = toCheck.IsGenericType ? toCheck.GetGenericTypeDefinition() : toCheck;
			if (generic == cur) {
				return true;
			}
			toCheck = toCheck.BaseType;
		}
		return false;
	}
}



[CustomPropertyDrawer (typeof (SettingsCommitter))]
public class CommitterDrawer : PropertyDrawer {
    public override void OnGUI (Rect position, SerializedProperty prop, GUIContent label) {
    	if( Application.isPlaying )
   		{
   		
			//EditorGUI.BeginProperty (position, label, prop);
			Rect buttonRect = new Rect( position.x + (position.width - 230) / 2, position.y, 230, position.height );
			
			
			if(GUI.Button(buttonRect, label.text))
			{
			
				
				
				Component com = prop.serializedObject.targetObject as Component;
				SettingsCommitterType committerType = (SettingsCommitterType)prop.FindPropertyRelative("type").enumValueIndex;
				Type type = com.GetType();				
				
				string path = SettingsWindow.projectAssetPath + type.Name + ".prefab";
				var asset = AssetDatabase.LoadAssetAtPath(path, type);
				
				FieldInfo[] infos  = type.GetFields();
				foreach( FieldInfo info in infos )
				{
					switch(committerType)
					{
						case SettingsCommitterType.Commit:
							info.SetValue( asset,  info.GetValue( com ) );
						break;
						
						case SettingsCommitterType.Retrieve:
							info.SetValue( com,  info.GetValue( asset ) );
						break;
					}
					
					
				}
			
			}
			
	      	//EditorGUI.EndProperty();
     	} 	

    }
 
    
    public override float GetPropertyHeight( SerializedProperty prop, GUIContent label )
    {
    
    	if( Application.isPlaying )
    		return 20;
    	else
    		return 0;
    }
}
