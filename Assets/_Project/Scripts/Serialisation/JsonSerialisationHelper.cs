﻿using Newtonsoft.Json.Old;
using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

//Save load helper is a helper class intended to abstract away the save, load, copying data processes for splash files 
public static class JsonSerialisationHelper
{
    private static JsonSerializerSettings _settings;
    private static JsonSerializerSettings Settings
    {
        get
        {
            if (_settings == null)
            {
                GetSettings();
            }
            return _settings;
        }
    }

    private static void GetSettings()
    {
        _settings = new JsonSerializerSettings();
        _settings.TypeNameHandling = TypeNameHandling.Auto;
        _settings.Converters.Add(new EXPToolkit.Serializers.JsonNet.ColorConverter());
        _settings.Converters.Add(new EXPToolkit.Serializers.JsonNet.TextureConverter());
        _settings.Converters.Add(new EXPToolkit.Serializers.JsonNet.Vector2Converter());
        _settings.Converters.Add(new EXPToolkit.Serializers.JsonNet.Vector3Converter());
        _settings.Converters.Add(new EXPToolkit.Serializers.JsonNet.Vector4Converter());
    }

    
    public static void Save<T>(string path, T obj)
    {
        // Save module count
        string jsonPayload = ConvertToJson(obj);
        System.IO.File.WriteAllText(path, jsonPayload);        
    }

    public static string ConvertToJson<T>(T obj)
    {
       return JsonConvert.SerializeObject(obj, typeof(T), Settings);
    }

    public static string ConvertToJson(object obj, Type t)
    {
        return JsonConvert.SerializeObject(obj, t, Settings);
    }
    
    public static object LoadFromFile<T>(string path)
    {
        Debug.Log("-------------  LOADING: " + path);
        string payload = System.IO.File.ReadAllText(path);
        return LoadFromString<T>(payload);
    }

    public static object LoadFromFileWithCulture<T>(string path, string culture)
    {
        Debug.Log("-------------  LOADING: " + path);
        string payload = System.IO.File.ReadAllText(path);
        JsonSerializerSettings settings = Settings;
        settings.Culture = new System.Globalization.CultureInfo(culture);

        return LoadFromString<T>(payload, settings);
    }


    public static T LoadFromString<T>(string payload)
    {
        return (T)JsonConvert.DeserializeObject(payload, typeof(T), Settings);
    }
    
    public static T LoadFromString<T>(string payload, JsonSerializerSettings settings)
    {
        return (T)JsonConvert.DeserializeObject(payload, typeof(T), settings);
    }
}

