using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.JSON
{
    [Serializable]
    public class Configuration
    {

        public Configuration()
        {
            if (allParameters == null)
            {
                allParameters = new List<ParameterType>();
            }
        }

        public List<ParameterType> allParameters;

        public string SaveToString()
        {
            return JsonUtility.ToJson(this, true);
        }

        public static Configuration CreateFromJSON(string jsonString)
        {
            return JsonUtility.FromJson<Configuration>(jsonString);
        }

        public void Add(ParameterType p)
        {
            allParameters.Add(p);
        }

        public List<ParameterType> getParameters()
        {
            return allParameters;
        }
    }

    public enum Type { ENUM, INTEGER, FLOAT, BOOLEAN };

    [Serializable]
    public class ParameterType
    {
        public Type type;

        public string name;

        public Type GetParameterType() { return type; }

        public string SaveToString()
        {
            return JsonUtility.ToJson(this, true);
        }

        public static T CreateFromJSON<T>(string jsonString)
        {
            return JsonUtility.FromJson<T>(jsonString);
        }

    }

    [Serializable]
    public class EnumParameter : ParameterType
    {
        
        public EnumParameter()
        {
            this.type = Type.ENUM;
        }

        public List<string> enumList;
        public string currentValue;

    }
    [Serializable]
    public class IntegerParameter : ParameterType
    {
        public IntegerParameter()
        {
            this.type = Type.INTEGER;
        }
        public int currentValue;

    }
    [Serializable]
    public class FloatParameter : ParameterType
    {
        public FloatParameter()
        {
            this.type = Type.FLOAT;
        }
        public float currentValue;

    }
    [Serializable]
    public class BooleanParameter : ParameterType
    {
        public BooleanParameter()
        {
            this.type = Type.BOOLEAN;
        }
        public bool currentValue;

    }
}
