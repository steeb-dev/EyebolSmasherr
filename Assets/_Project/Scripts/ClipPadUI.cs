using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using SimpleFileBrowser;


public enum ClipPadState
{
    Empty,
    Loading,
    Loaded,
    Playing
}

public class ClipPadUI : MonoBehaviour
{
    public AppManager _Manager;

    public Button _ClipButton;
    public RawImage _ClipButtonImage;
    public Text _ClipButtonText;
    public Scrollbar _ProgressBar;
    public Image _ClipStateImage;

    public ClipPadState _State;
    public ProjectClip _ClipData;

    public VideoPlayerController _VideoPlayerController;

    private void Awake()
    {
        _ClipButton.onClick.AddListener(() => OnPadTrigger());
    }

    public void OnPadTrigger()
    {
        switch (_State)
        {
            case ClipPadState.Empty:
                LoadFileDialog();
                break;
            case ClipPadState.Loaded:
                _Manager.FocusClipEditor(this);
                _Manager.PlayClip(_ClipData._VideoPath, this);
                break;
            case ClipPadState.Playing:
                _Manager.FocusClipEditor(this);
                _VideoPlayerController.RestartClip(false);
                break;
        }
    }

    public void LoadFileDialog()
    {
        string startPath = PlayerPrefs.GetString("LastVideoPath", null);
        FileBrowser.ShowLoadDialog(OnLoadSuccess, null, false, startPath);
    }

    private void OnLoadSuccess(string filePath)
    {
        System.IO.FileInfo fileInfo = new System.IO.FileInfo(filePath);
        PlayerPrefs.SetString("LastVideoPath", fileInfo.Directory.FullName);

        _ClipData = new ProjectClip();
        _ClipData._VideoPath = filePath;

        _Manager.GetClipPreview(filePath, this);
    }

    public void LoadFromClipData(ProjectClip clipData)
    {
        _ClipData = clipData;
        _State = ClipPadState.Loaded;
        RefreshUI();
    }


    public void OnClipPreviewComplete(Texture2D previewTexture)
    {
        _ClipData._PreviewImg = previewTexture;
        
        //NFI why this is needed, but otherwise randomly half of the preview images won't render on the pad UI
        _ClipData._PreviewImg.LoadImage(previewTexture.EncodeToPNG());

        //If we're previewing, not loading from serialised, initialise the clipdata for first run
        _ClipData._VideoLengthMs = _Manager._PreviewPlayer.Info.GetDurationMs();
        _ClipData.InitSettings();

        _State = ClipPadState.Loaded;
        RefreshUI();
    }

    void RefreshUI()
    {
        _ClipButtonImage.texture = _ClipData._PreviewImg;
        _ClipButton.GetComponent<Image>().color = Color.clear;
        _ClipButtonText.enabled = false;
        _Manager.FocusClipEditor(this);
    }

    public void Clear()
    {
        _ClipData = null;
        _ClipButtonImage.texture = null;
        _ClipButton.GetComponent<Image>().color = Color.white;
        _ClipButtonText.enabled = true;
        _State = ClipPadState.Empty;
    }
}
