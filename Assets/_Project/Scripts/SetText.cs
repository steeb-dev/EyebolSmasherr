using UnityEngine;
using System.Collections;
using UnityEngine.UI;

[RequireComponent(typeof(Text))]
public class SetText : MonoBehaviour
{
    public string _FormatString;
    Text _TextBox;
    private void Awake()
    {
        _TextBox = GetComponent<Text>();
    }
    public void SetFloatText(float val)
    {
        _TextBox.text = val.ToString(_FormatString);
    }


}
