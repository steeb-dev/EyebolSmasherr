using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using RenderHeads.Media.AVProVideo;

public class VideoPlayerController : MonoBehaviour
{
    public RenderHeads.Media.AVProVideo.MediaPlayer _Player;

    private MediaPlayerState _PlayerState = MediaPlayerState.Idle;
    public Material _VideoProjMat;

    private float _CurrentPlaybackSpeed;
    ClipPadUI _CurrentClipPad;

    AppManager _Manager;
    public RenderHeads.Media.AVProVideo.DisplayUGUI[] VideoProjections;

    Color _CurrentColor = Color.white;
    float _CurrentColorMask;
    float _CurrentLumaKey;
    float _CurrentLumaBlend;
    bool _CurrentInvertLuma;
    float _CurrentXPos;
    float _CurrentYPos;
    float _CurrentWidth;
    float _CurrentHeight;

    private void Start()
    {
        _Player.Events.AddListener(OnVideoEvent);
    }

    private void Update()
    {
        switch (_PlayerState)
        {
            case MediaPlayerState.Idle:
                break;
            case MediaPlayerState.Playing:
                HandlePlayingState();
                break;
            case MediaPlayerState.Loading:
                break;
        }
    }

    public void OnVideoEvent(MediaPlayer mp, MediaPlayerEvent.EventType et, ErrorCode code)
    {
        switch (et)
        {
            case MediaPlayerEvent.EventType.ReadyToPlay:
                StartCoroutine(StartClip());
                break;
            case MediaPlayerEvent.EventType.FirstFrameReady:
                break;
            case MediaPlayerEvent.EventType.FinishedPlaying:
                break;
        }
        Debug.Log("Event: " + et.ToString());
    }

    public void SetState(MediaPlayerState newState)
    {
        switch (newState)
        {
            case MediaPlayerState.Idle:
                _Player.Stop();
                foreach (RenderHeads.Media.AVProVideo.DisplayUGUI videoProjection in VideoProjections)
                {
                    videoProjection.color = Color.clear;
                }
                break;
            case MediaPlayerState.Loading:
                foreach (RenderHeads.Media.AVProVideo.DisplayUGUI videoProjection in VideoProjections)
                {
                    videoProjection.color = Color.clear;
                }
                break;
            case MediaPlayerState.Playing:
                foreach (RenderHeads.Media.AVProVideo.DisplayUGUI videoProjection in VideoProjections)
                {
                    videoProjection.color = _CurrentColor;
                    videoProjection.rectTransform.SetAsLastSibling();
                }
                break;
        }

        _PlayerState = newState;
    }

    public void UpdatePlaybackSpeed()
    {
        _Player.Control.SetPlaybackRate(_CurrentClipPad._ClipData._ClipPlaybackSpeed);
        _CurrentPlaybackSpeed = _CurrentClipPad._ClipData._ClipPlaybackSpeed;
    }
    void HandlePlayingState()
    {
        HandleDataUpdates();
        if (_CurrentClipPad._ClipData._ClipPlaybackSpeed >= 0)
        {
            if (_Player.Control.GetCurrentTimeMs() >= _CurrentClipPad._ClipData._VideoEndMs)
            {
                HandleClipLoopMode();
            }
        }
        else
        {
            if (_Player.Control.GetCurrentTimeMs() <= _CurrentClipPad._ClipData._VideoStartMs)
            {
                HandleClipLoopMode();
            }
        }
    }

    void HandleDataUpdates()
    {
        if (_CurrentPlaybackSpeed != _CurrentClipPad._ClipData._ClipPlaybackSpeed)
        {
            UpdatePlaybackSpeed();
        }

        if (_CurrentColor != _CurrentClipPad._ClipData._TintColor)
        {
            UpdateColor();
        }

        if (_CurrentColorMask != _CurrentClipPad._ClipData._ColorMask)
        {
            UpdateColorMask();
        }

        if (_CurrentLumaBlend != _CurrentClipPad._ClipData._LumaBlend)
        {
            UpdateLumaBlend();
        }

        if (_CurrentLumaKey != _CurrentClipPad._ClipData._LumaKey)
        {
            UpdateLumaKey();
        }

        if (_CurrentInvertLuma != _CurrentClipPad._ClipData._InvertLuma)
        {
            UpdateInvertLuma();
        }

        if (_CurrentXPos != _CurrentClipPad._ClipData._XPos)
        {
            UpdateXPos();
        }

        if (_CurrentYPos != _CurrentClipPad._ClipData._YPos)
        {
            UpdateYPos();
        }

        if (_CurrentWidth != _CurrentClipPad._ClipData._Width)
        {
            UpdateWidth();
        }

        if (_CurrentHeight != _CurrentClipPad._ClipData._Height)
        {
            UpdateHeight();
        }

    }

    void HandleClipLoopMode()
    {
        switch (_CurrentClipPad._ClipData._PlaybackType)
        {
            case ClipPlaybackType.Loop:
                RestartClip(true);
                break;
            case ClipPlaybackType.OneHit:
                SetState(MediaPlayerState.Idle);
                break;
            case ClipPlaybackType.PingPong:
                _CurrentClipPad._ClipData._ClipPlaybackSpeed = -_CurrentClipPad._ClipData._ClipPlaybackSpeed;
                UpdatePlaybackSpeed();
                break;
        }
    }
    public void PlayClip(string clipPath, ClipPadUI clipPad)
    {
        if (_PlayerState != MediaPlayerState.Loading)
        {
            SetState(MediaPlayerState.Loading);

            if (_CurrentClipPad != null)
            {
                _CurrentClipPad._VideoPlayerController = null;
                _CurrentClipPad._State = ClipPadState.Loaded;
            }

            clipPad._VideoPlayerController = this;
            clipPad._State = ClipPadState.Playing;
            _CurrentClipPad = clipPad;
            _Player.OpenVideoFromFile(RenderHeads.Media.AVProVideo.MediaPlayer.FileLocation.AbsolutePathOrURL, clipPath, false);
        }
    }

    public IEnumerator StartClip()
    {
        yield return new WaitForEndOfFrame();
        _Player.Control.Seek(_CurrentClipPad._ClipData._VideoStartMs);
        while (_Player.Control.IsSeeking())
            yield return new WaitForEndOfFrame();
        _CurrentPlaybackSpeed = _CurrentClipPad._ClipData._ClipPlaybackSpeed;
        _Player.Control.SetPlaybackRate(_CurrentClipPad._ClipData._ClipPlaybackSpeed);
        _Player.Control.Play();
        while (!_Player.Control.IsPlaying())
            yield return new WaitForEndOfFrame();

        yield return new WaitForEndOfFrame();
        SetState(MediaPlayerState.Playing);
    }

    public void UpdateColor()
    {
        _CurrentColor = _CurrentClipPad._ClipData._TintColor;
        foreach (DisplayUGUI ugui in VideoProjections)
        {
            ugui.color = _CurrentColor;
        }
    }
    public void UpdateXPos()
    {
        _CurrentXPos = _CurrentClipPad._ClipData._XPos;
        foreach (DisplayUGUI ugui in VideoProjections)
        {
            Rect rect = ugui.uvRect;
            rect.x = _CurrentXPos;
            ugui.uvRect = rect; 
        }
    }
    public void UpdateYPos()
    {
        _CurrentYPos = _CurrentClipPad._ClipData._YPos;
        foreach (DisplayUGUI ugui in VideoProjections)
        {
            Rect rect = ugui.uvRect;
            rect.y = _CurrentYPos;
            ugui.uvRect = rect;
        }
    }

    public void UpdateWidth()
    {
        _CurrentWidth = _CurrentClipPad._ClipData._Width;
        foreach (DisplayUGUI ugui in VideoProjections)
        {
            Rect rect = ugui.uvRect;
            rect.width = _CurrentWidth;
            ugui.uvRect = rect;
        }
    }
    public void UpdateHeight()
    {
        _CurrentHeight = _CurrentClipPad._ClipData._Height;
        foreach (DisplayUGUI ugui in VideoProjections)
        {
            Rect rect = ugui.uvRect;
            rect.height = _CurrentHeight;
            ugui.uvRect = rect;
        }
    }


    public void UpdateColorMask()
    {
        _CurrentColorMask = _CurrentClipPad._ClipData._ColorMask;
        foreach (DisplayUGUI ugui in VideoProjections)
        {
            ugui.material.SetFloat("_ColorMask", _CurrentColorMask);
        }
    }
    
    public void UpdateLumaKey()
    {
        _CurrentLumaKey = _CurrentClipPad._ClipData._LumaKey;
        foreach (DisplayUGUI ugui in VideoProjections)
        {
            ugui.material.SetFloat("_Threshold", _CurrentLumaKey);
        }
    }
    public void UpdateLumaBlend()
    {
        _CurrentLumaBlend = _CurrentClipPad._ClipData._LumaBlend;
        foreach (DisplayUGUI ugui in VideoProjections)
        {
            ugui.material.SetFloat("_Blend", _CurrentLumaBlend);
        }
    }

    public void UpdateInvertLuma()
    {
        _CurrentInvertLuma = _CurrentClipPad._ClipData._InvertLuma;
        foreach (DisplayUGUI ugui in VideoProjections)
        {
            if (_CurrentInvertLuma)
                ugui.material.SetFloat("_InvLuma", 1.0f);
            else
                ugui.material.SetFloat("_InvLuma", 0f);
        }
    }

    public void RestartClip(bool loopHit)
    {
        if (_CurrentClipPad._ClipData._ClipPlaybackSpeed >= 0)
        {
            _Player.Control.Pause();
            _Player.Control.Seek(_CurrentClipPad._ClipData._VideoStartMs);
            _Player.Control.Play();
        }
        else
        {
            _Player.Control.Pause();
            _Player.Control.Seek(_CurrentClipPad._ClipData._VideoEndMs);
            _Player.Control.Play();
        }

        if (_PlayerState == MediaPlayerState.Idle || !loopHit)
            SetState(MediaPlayerState.Playing);
    }


}