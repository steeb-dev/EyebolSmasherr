using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ClipEditorUI : MonoBehaviour
{
    public ClipPadUI _TargetClipPad;
    public GameObject _EditorPanel;

    public Text _FileNameText;
    public Toggle _OneHitToggle;
    public Toggle _LoopToggle;
    public Toggle _PingPongToggle;
    public Button _OneHitButton;
    public Button _LoopButton;
    public Button _PingPongButton;

    public Slider _PlaybackSpeedSlider;
    public MinMaxSlider _MinMaxSlider;
    public ColorPicker _Picker;

    public Slider _LumaKeySlider;
    public Slider _LumaBlendSlider;
    public Slider _ColorMaskSlider;


    public Slider _XPosSlider;
    public Slider _YPosSlider;
    public Slider _WidthSlider;
    public Slider _HeightSlider;

    public Toggle _InvertLumaToggle;

    bool _IgnoreUpdateEvent = false;

    private AppManager _AppManager;

    private void Update()
    {
        if(_AppManager == null)
        {
            _AppManager = FindObjectOfType<AppManager>();
        }

        if (_TargetClipPad == null)
        {
            _EditorPanel.SetActive(false);
        }
        else
        {
            _EditorPanel.SetActive(true);
        }
    }

    public void ChangeFile()
    {
        _TargetClipPad.LoadFileDialog();
    }

    public void Refresh()
    {
        _IgnoreUpdateEvent = true;
        _FileNameText.text = _TargetClipPad._ClipData.FileName;
        _MinMaxSlider.SetLimits(0f, _TargetClipPad._ClipData._VideoLengthMs - 1);
        _MinMaxSlider.SetValues(_TargetClipPad._ClipData._VideoStartMs, _TargetClipPad._ClipData._VideoEndMs);
        _PlaybackSpeedSlider.value = _TargetClipPad._ClipData._ClipPlaybackSpeed;
        _Picker.CurrentColor = _TargetClipPad._ClipData._TintColor;
        _ColorMaskSlider.value = _TargetClipPad._ClipData._ColorMask;
        _LumaBlendSlider.value = _TargetClipPad._ClipData._LumaBlend;
        _LumaKeySlider.value = _TargetClipPad._ClipData._LumaKey;
        _InvertLumaToggle.isOn = _TargetClipPad._ClipData._InvertLuma;

        _XPosSlider.value = _TargetClipPad._ClipData._XPos;
        _YPosSlider.value = _TargetClipPad._ClipData._YPos;
        _WidthSlider.value = _TargetClipPad._ClipData._Width;
        _HeightSlider.value = _TargetClipPad._ClipData._Height;

        RefreshModeToggleGroup();
        _IgnoreUpdateEvent = false;
    }

    public void OneHitToggleSelect()
    {
        _TargetClipPad._ClipData._PlaybackType = ClipPlaybackType.OneHit;
        RefreshModeToggleGroup();
    }
    public void LoopToggleSet()
    {
        _TargetClipPad._ClipData._PlaybackType = ClipPlaybackType.Loop;
        RefreshModeToggleGroup();
    }
    public void PingPongToggleSet()
    {
        _TargetClipPad._ClipData._PlaybackType = ClipPlaybackType.PingPong;
        RefreshModeToggleGroup();
    }
       
    void RefreshModeToggleGroup()
    {
        _OneHitToggle.isOn = _TargetClipPad._ClipData._PlaybackType == ClipPlaybackType.OneHit;
        _OneHitButton.interactable = _TargetClipPad._ClipData._PlaybackType != ClipPlaybackType.OneHit;
        _LoopToggle.isOn = _TargetClipPad._ClipData._PlaybackType == ClipPlaybackType.Loop;
        _LoopButton.interactable = _TargetClipPad._ClipData._PlaybackType != ClipPlaybackType.Loop;
        _PingPongToggle.isOn = _TargetClipPad._ClipData._PlaybackType == ClipPlaybackType.PingPong;
        _PingPongButton.interactable = _TargetClipPad._ClipData._PlaybackType != ClipPlaybackType.PingPong;
    }

    public void UpdatePlaybackSpeed(float newSpeed)
    {
        if(!_IgnoreUpdateEvent)
        {
            _TargetClipPad._ClipData._ClipPlaybackSpeed = newSpeed;
        }
    }

    public void UpdateClipLoopPoints(float start, float end)
    {
        _TargetClipPad._ClipData._VideoStartMs = start;
        _TargetClipPad._ClipData._VideoEndMs = end;
    }    

    public void UpdateColor(Color c)
    {
        if(_TargetClipPad != null && !_IgnoreUpdateEvent)
            _TargetClipPad._ClipData._TintColor = c;
    }

    public void UpdateColorMask(float maskVal)
    {
        if (_TargetClipPad != null && !_IgnoreUpdateEvent)
            _TargetClipPad._ClipData._ColorMask = maskVal;
    }
    
    public void UpdateLumaKey(float keyVal)
    {
        if (_TargetClipPad != null && !_IgnoreUpdateEvent)
            _TargetClipPad._ClipData._LumaKey = keyVal;
    }
    public void UpdateLumaBlend(float blendVal)
    {
        if (_TargetClipPad != null && !_IgnoreUpdateEvent)
            _TargetClipPad._ClipData._LumaBlend = blendVal;
    }

    public void UpdateXPos(float xPos)
    {
        if (_TargetClipPad != null && !_IgnoreUpdateEvent)
            _TargetClipPad._ClipData._XPos = xPos;
    }

    public void UpdateYPos(float yPos)
    {
        if (_TargetClipPad != null && !_IgnoreUpdateEvent)
            _TargetClipPad._ClipData._YPos = yPos;
    }

    public void UpdateWidth(float width)
    {
        if (_TargetClipPad != null && !_IgnoreUpdateEvent)
            _TargetClipPad._ClipData._Width = width;
    }

    public void UpdateHeight(float height)
    {
        if (_TargetClipPad != null && !_IgnoreUpdateEvent)
            _TargetClipPad._ClipData._Height = height;
    }

    public void ToggleInvertLuma()
    {
        if (_TargetClipPad != null && !_IgnoreUpdateEvent)
            _TargetClipPad._ClipData._InvertLuma = _InvertLumaToggle.isOn;
    }
}
