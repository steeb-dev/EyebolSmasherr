using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RenderHeads.Media.AVProVideo;

public class ProjectClip
{
    public string _VideoPath;
    public float _VideoStartMs;
    public float _VideoEndMs;
    public float _VideoLengthMs;
    public ClipPlaybackType _PlaybackType;
    public float _ClipPlaybackSpeed;
    public Texture2D _PreviewImg;
    public Color _TintColor;
    public float _ColorMask;
    public float _LumaKey;
    public float _LumaBlend;
    public bool _InvertLuma;
    public float _XPos;
    public float _YPos;
    public float _Width;
    public float _Height;


    public string FileName
    {
        get
        {
            return System.IO.Path.GetFileName(_VideoPath);
        }
    }

    public void InitSettings()
    {
        _VideoStartMs = 0f;
        _VideoEndMs = _VideoLengthMs - 1;
        _PlaybackType = ClipPlaybackType.Loop;
        _ClipPlaybackSpeed = 1.0f;
        _TintColor = Color.white;
        _ColorMask = 15;
        _LumaKey = 0;
        _LumaBlend = 1;
        _Width = 1;
        _Height = 1;
        _XPos = 0;
        _YPos = 0;
        _InvertLuma = false;
    }

}

public enum ClipPlaybackType
{
    OneHit,
    Loop,
    PingPong
}
