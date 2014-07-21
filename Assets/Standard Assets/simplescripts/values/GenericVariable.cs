using System.Reflection;
using System;

[System.Serializable]
public class GenericVariable
{
	public string name;
	public GenericVariableType type = GenericVariableType.Int;
	
	public string stringValue;
	public int intValue;
	public bool boolValue;
	public float floatValue;
	public float minValue;
	public float maxValue;
	
	
	public GenericVariable() { }
	public GenericVariable( string name, GenericVariableType type )
	{
		this.name = name;
		this.type = type;
	}
	
	public GenericVariable( ParameterInfo parameter )
	{
		name = parameter.Name;

		switch( parameter.ParameterType.Name )
		{
			case "String":
				type = GenericVariableType.String;
			break;
			
			case "Int32":
				type = GenericVariableType.Int;
			break;
			
			case "Boolean":
				type = GenericVariableType.Bool;
			break;
			
			case "Single":
				type = GenericVariableType.Float;
			break;
		}
	}
	
	public object GetValue()
	{
		object result = null;
		
		switch( type )
		{
			case GenericVariableType.String:
				result = stringValue;
			break;
			
			case GenericVariableType.Int:
				result = intValue;
			break;
			
			case GenericVariableType.Bool:
				result = boolValue;
			break;
			
			case GenericVariableType.Range:
			case GenericVariableType.Float:
				result = floatValue;
			break;
		}
		
		return result;
	}
	
	public Type GetValueType()
	{
		Type result = null;
		
		switch( type )
		{
			case GenericVariableType.String:
				result = typeof(string);
			break;
			
			case GenericVariableType.Int:
				result = typeof(int);
			break;
			
			case GenericVariableType.Bool:
				result = typeof(bool);
			break;
			
			case GenericVariableType.Range:
			case GenericVariableType.Float:
				result = typeof(float);
			break;
		}
		
		return result;
	}

	public GenericVariable Clone()
	{
		GenericVariable variable = new GenericVariable( name, type );
		variable.stringValue = stringValue;
		variable.intValue = intValue;
		variable.boolValue = boolValue;
		variable.floatValue = floatValue;
		variable.minValue = minValue;
		variable.maxValue = maxValue;
		
		return variable;
	}

}


public enum GenericVariableType
{
	String,
	Int,
	Bool,
	Float,
	Range
}
