using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System;

public class Settings<T> : MonoBehaviour where T : MonoBehaviour {

	public static string projectAssetPath = "Assets/resources/settings/";
	
	static T _instance;
	static string[] _provides;
	static bool isQuitting;
	
	
	[SerializeField]
	[HideInInspector]
	List<Component> _components = new List<Component>();
	
	List<Component> _componentsToAdd = new List<Component>();
	List<Component> _componentsToRemove = new List<Component>();
	
	public SettingsCommitter commitValuesToPrefab = new SettingsCommitter( SettingsCommitterType.Commit );
	public SettingsCommitter retrieveValuesFromPrefab = new SettingsCommitter( SettingsCommitterType.Retrieve );
	
	bool _isFiring = false;
    static Transform _settingsParent;
	
	public static T Instance
	{
		get{ 
			if(_instance == null) _instance = LoadInstanceFromPrefab();
			return _instance; 
		}
	}
		
	public static bool Subscribe( Component com )
	{
		if( Instance != null && !isQuitting)
		{
			(Instance as Settings<T>).SubscribeComponent( com );
			return true;
		} else {
			return false;
		}
	}
	
	public static bool Unsubscribe( Component com )
	{
		if( Instance != null && !isQuitting)
		{
			(Instance as Settings<T>).UnsubscribeComponent( com );
			return true;
		} else {
			return false;
		}
	}
	
	public static T LoadInstanceFromPrefab()
	{
		if( Application.isPlaying && !isQuitting) 
		{

			GameObject prefab = Resources.Load(resourcePath, typeof(GameObject)) as GameObject;
			if( prefab == null )
			{
				Debug.LogError("No prefab found to instantiate " + typeof(T) + ". Click 'Create " + typeof(T)+ " prefab' in the Settingss window to make one."  );
				return null;
			}
			
			GameObject go = Instantiate( prefab ) as GameObject;
			go.name = typeof(T).ToString();

            GameObject parentGO = GameObject.Find("_Settings");
            if ( parentGO == null )
            {
                parentGO = new GameObject( "_Settings" );
                DontDestroyOnLoad( parentGO );
            }
            _settingsParent = parentGO.transform;
            go.transform.parent = _settingsParent;
			
			T instance = go.GetComponent<T>();
			if( instance == null ) 
				Debug.LogError("Settings prefab doesn't contain " + typeof(T) + " component");
		
			DontDestroyOnLoad(go);
			return instance;
			
		} else {
			#if UNITY_EDITOR
				string path = projectAssetPath + typeof(T).Name + ".prefab";
				var asset = AssetDatabase.LoadAssetAtPath(path, typeof(T) );
				return asset as T;
			#else		
				return null;
			#endif
		}
	}
	
	static void InspectProviders()
	{	
		List<string> pro = new List<string>();
		
		Type type = typeof( T );
		FieldInfo[] infos = type.GetFields( BindingFlags.NonPublic | BindingFlags.Instance );
		foreach( FieldInfo info in infos )
		{
			if( info.FieldType == typeof(ProvidesEvent) )
			{
				ProvidesEvent newProvides = new ProvidesEvent( info.Name );
				pro.Add( info.Name );
				info.SetValue( _instance, newProvides );
			}
		}
		
		_provides = pro.ToArray();
	}
	
	static string[] provides
	{
		get
		{
			if( _provides == null ) InspectProviders();
			return _provides;
		}
	}
	
	static string resourcePath
	{
		get
		{
			return "settings/" + typeof(T);
		}
	}

	
	//------------------------------------------------------------------------------------------------
	
	
	void Awake()
	{
		_instance = this as T;
		InspectProviders();
	}
	
	void OnApplicationQuit()
    {
    	isQuitting = true;
		DestroyInstance();
    }

	void SubscribeComponent( Component com )
	{
		if( _isFiring )
		{
			_componentsToAdd.Add( com );
 		} else {
 			_components.Add( com );
 		}
	}
	
	void UnsubscribeComponent( Component com )
	{
		if( _isFiring )
		{
			_componentsToRemove.Add( com );
 		} else {
 			_components.Remove( com );
 		}
	}
	
	protected void FireEvent( ProvidesEvent provider )
	{
		FireEvent( provider, null );
	}
	
	protected void FireEvent( ProvidesEvent provider, object value )
	{
		_isFiring = true;
		foreach( Component com in _components )
		{
			if( com != null )
			{
				com.SendMessage( provider.name, value, SendMessageOptions.DontRequireReceiver );
			} else {
				//_componentsToRemove.Add( com );
			}
		}
		_isFiring = false;
		
		foreach( Component com in _componentsToAdd ) SubscribeComponent( com );
		foreach( Component com in _componentsToRemove ) UnsubscribeComponent( com );
		
		_componentsToAdd.Clear();
		_componentsToRemove.Clear();
		
		
	}
	
	void DestroyInstance()
    {
        if (_instance != null) DestroyImmediate(_instance.gameObject);    
    }
	
	
	
}

//------------------------------------------------------------------------------------------------

[System.Serializable]
public class ProvidesEvent
{ 
	public string name;
	
	public ProvidesEvent( string name )
	{
		this.name = name;
	}
	
	public override string ToString()
	{
		return "Provide Event: " + this.name;
	}
}

[System.Serializable]
public class SettingsCommitter 
{
	
	public SettingsCommitterType type;
	
	public SettingsCommitter( SettingsCommitterType type )
	{
		this.type = type;
	}
}

public enum SettingsCommitterType
{
	Commit,
	Retrieve
}
