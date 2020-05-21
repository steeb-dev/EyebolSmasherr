using UnityEngine;
using System.Collections;
using UnityEngine.UI;

[RequireComponent(typeof(Text))]
public class SetText : MonoBehaviour
{
    public string _FormatString;
    public string[] _IntToStringMappings;
    Text _TextBox;
    private void Awake()
    {
        _TextBox = GetComponent<Text>();
    }
    public void SetFloatText(float val)
    {
        _TextBox.text = val.ToString(_FormatString);
    }

    public void IntToText(float val)
    {
        int intVal = (int)val - 1;
        if(intVal  < _IntToStringMappings.Length)
        {
            _TextBox.text = _IntToStringMappings[intVal];
        }
    }


}
