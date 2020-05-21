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

    public string FileName
    {
        get
        {
            return System.IO.Path.GetFileName(_VideoPath);
        }
    }
}

public enum ClipPlaybackType
{
    OneHit,
    Loop,
    PingPong
}
