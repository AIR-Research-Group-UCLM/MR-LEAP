using SharpConfig;
using System;
using UnityEngine;

public abstract class ConfigurableMonoBehaviour : MonoBehaviour
{
    /// <summary>
    /// Prevent classes from using Awake to avoid problems while loading config
    /// </summary>
    protected virtual void Awake()
    {
    }

    public abstract void SetupCleanConfiguration(Configuration cfg);

    public abstract void LoadConfiguration(Configuration cfg);

    protected void ParseConfigInt(Setting setting, ref int defaultParameter)
    {
        if (setting.IsEmpty)
        {
            setting.IntValue = defaultParameter;
        }
        else
        {
            defaultParameter = setting.IntValue;
        }
    }

    protected void ParseConfigFloat(Setting setting, ref float defaultParameter)
    {
        if (setting.IsEmpty)
        {
            setting.FloatValue = defaultParameter;
        }
        else
        {
            defaultParameter = setting.FloatValue;
        }
    }

    protected void ParseConfigString(Setting setting, ref string defaultParameter)
    {
        if (setting.IsEmpty)
        {
            setting.StringValue = defaultParameter;
        }
        else
        {
            defaultParameter = setting.StringValue;
        }
    }

    protected void ParseConfigStringArray(Setting setting, ref string[] defaultParameter)
    {
        if (setting.IsEmpty)
        {
            setting.StringValueArray = defaultParameter;
        }
        else
        {
            defaultParameter = setting.StringValueArray;
        }
    }

    protected void ParseConfigBool(Setting setting, ref bool defaultParameter)
    {
        if (setting.IsEmpty)
        {
            setting.BoolValue = defaultParameter;
        }
        else
        {
            defaultParameter = setting.BoolValue;
        }
    }


    protected void ParseConfigInt<TEnum>(Setting setting, ref TEnum defaultParameter) where TEnum : struct, Enum
    {
        if (setting.IsEmpty)
        {
            defaultParameter = default(TEnum);
        }
        else
        {
            if (Enum.TryParse<TEnum>(setting.IntValue.ToString(), out TEnum enumValue))
            {
                defaultParameter = enumValue;
            }
            else
            {
                Debug.LogWarning("No se pudo convertir el valor int al enum deseado.");
                defaultParameter = default(TEnum);
            }
        }
    }




}