using UnityEngine;
using System.Collections;


[System.Serializable]
public class UniqueObject
{

	[SerializeField]
	[HideInInspector]
	private string _guid;
	
	public UniqueObject()
	{
		Reset();
	}
	
	protected void SetGuid( string guid )
	{
		_guid = guid;
	}
	
	
	public void Reset()
	{
		_guid = System.Guid.NewGuid().ToString();
	}

	public string guid
	{
		get
		{
			if( _guid == null ) Reset();
			return _guid;
		}
	}
}



[System.Serializable]
public class UniqueReference<T> where T : UniqueObject
{
	[HideInInspector]
	public string id;
	private string lastId;
	
	public UniqueReference(){}
	
	public UniqueReference( string id )
	{
		this.id = id;
		lastId = id;
	}
	
	public UniqueReference( T obj )
	{
		this.id = obj.guid;
		lastId = this.id;
	}
	
	public bool HasChanged()
	{
		bool result = (lastId != id);
		lastId = id;
		return result;
	}
	
	public override bool Equals(object obj)
	{
		UniqueReference<T> reference = obj as UniqueReference<T>;
		return ( reference != null && reference.id == this.id );
	}
	
	public override int GetHashCode()
	{
		return id.ToString().GetHashCode();
	}
	


}


public class UniqueReferenceAttribute : PropertyAttribute 
{ 
	public System.Type Type;

	public UniqueReferenceAttribute(System.Type type)
	{
		this.Type = type;
	}


}