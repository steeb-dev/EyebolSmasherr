using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SimpleFileBrowser;
using RenderHeads.Media.AVProVideo;


public enum MediaPlayerState
{
    Idle,
    Loading,
    Playing
}

public class AppManager : MonoBehaviour
{
    public delegate void PreviewCompleted(Texture2D texture);
    public GameObject _ClipPadUIPrefab;
    public Transform _ClipPadParent;


    public ClipPadUI _CurrentPreviewingClip;
    public List<ClipPadUI> _ClipPads = new List<ClipPadUI>();
    public ClipEditorUI _ClipEditorUI;
    public RenderHeads.Media.AVProVideo.MediaPlayer _PreviewPlayer;

    public string _LoadedProjectPath;
    public string _ProjectName;
    public UnityEngine.UI.Text _FileNameText;

    public VideoPlayerController[] _VideoPlayerControllers;
    int _VPCIndex = 0;
    
    bool _SecondDisplayEnabled = false;

    public Camera _SecondaryCamera;
    public UnityEngine.Canvas _SecondaryCanvas;

    // Start is called before the first frame update
    void Start()
    {
        MidiJack.MidiDriver.Instance.noteOnDelegate += OnMidiNoteOn;
        MidiJack.MidiDriver.Instance.knobDelegate += OnMidiControlChange;

        _ProjectName = "<New Project>";
        _LoadedProjectPath = null;
        for (int i = 0; i < 16; i++)
        {
            GameObject go = Instantiate(_ClipPadUIPrefab);
            go.transform.SetParent(_ClipPadParent);
            go.transform.localPosition = Vector3.zero;
            go.transform.localScale = Vector3.one;
            go.GetComponent<ClipPadUI>()._Manager = this;
            _ClipPads.Add(go.GetComponent<ClipPadUI>());
        }
    }

    private void Update()
    {
        _FileNameText.text = _ProjectName;
        if(Input.GetKey(KeyCode.LeftShift) && !_SecondDisplayEnabled)
        {
            if (Input.GetKeyDown(KeyCode.Alpha2))
            {
                ActivateSecondaryDisplay(1);
            }
            if (Input.GetKeyDown(KeyCode.Alpha3))
            {
                ActivateSecondaryDisplay(2);
            }
        }

        if(Input.GetKeyDown(KeyCode.Escape))
        {
            Application.Quit();
        }
    }

    void ActivateSecondaryDisplay(int displayIndex)
    {
#if UNITY_EDITOR
        _SecondaryCamera.targetDisplay = displayIndex;
        _SecondaryCanvas.targetDisplay = displayIndex;
        _SecondaryCamera.gameObject.SetActive(true);
        _SecondaryCanvas.gameObject.SetActive(true);
        _SecondDisplayEnabled = true;
#else
       if (Display.displays.Length >= (displayIndex +1))
        {
            Display.displays[displayIndex].Activate();
            _SecondaryCamera.targetDisplay = displayIndex;
            _SecondaryCanvas.targetDisplay = displayIndex;
            _SecondaryCamera.gameObject.SetActive(true);
            _SecondaryCanvas.gameObject.SetActive(true);
            _SecondDisplayEnabled = true;
        }
        else
        {
            Debug.Log("Can't activate display, index: " + displayIndex + " not found.");
        }
#endif
    }

    void OnMidiNoteOn(MidiJack.MidiChannel channel, int note, float velocity)
    {
        if (note >= 48 && note <= 63)
        {
            int clipPadIndex = note - 48;
            _ClipPads[clipPadIndex].OnPadTrigger();
        }
    }
   
    void OnMidiControlChange(MidiJack.MidiChannel channel, int ccNumber, float ccValue)
    {

        if (_ClipEditorUI._TargetClipPad != null)
        { if (ccNumber >= 8 && ccNumber < 20)
            {
                float alpha, H, S, V;
                int controlIndex = ccNumber - 8;
                switch (controlIndex)
                {
                    //Clip Speed
                    case 0:
                        _ClipEditorUI._TargetClipPad._ClipData._ClipPlaybackSpeed = Mathf.Lerp(0.1f, 4f, ccValue);
                        _ClipEditorUI.Refresh();
                    break;
                    //Clip Colour H
                    case 1:
                        alpha = _ClipEditorUI._TargetClipPad._ClipData._TintColor.a;
                        Color.RGBToHSV(_ClipEditorUI._TargetClipPad._ClipData._TintColor, out H, out S, out V);
                        H = ccValue;
                        _ClipEditorUI._TargetClipPad._ClipData._TintColor = Color.HSVToRGB(H, S, V);
                        _ClipEditorUI._TargetClipPad._ClipData._TintColor.a = alpha;
                        _ClipEditorUI.Refresh();
                    break;
                    //Clip Colour S
                    case 2:
                        alpha = _ClipEditorUI._TargetClipPad._ClipData._TintColor.a;
                        Color.RGBToHSV(_ClipEditorUI._TargetClipPad._ClipData._TintColor, out H, out S, out V);
                        S = ccValue;
                        _ClipEditorUI._TargetClipPad._ClipData._TintColor = Color.HSVToRGB(H, S, V);
                        _ClipEditorUI._TargetClipPad._ClipData._TintColor.a = alpha; 
                        _ClipEditorUI.Refresh();
                        break;
                    //Clip Colour V
                    case 3:
                        alpha = _ClipEditorUI._TargetClipPad._ClipData._TintColor.a;
                        Color.RGBToHSV(_ClipEditorUI._TargetClipPad._ClipData._TintColor, out H, out S, out V);
                        V = ccValue;
                        _ClipEditorUI._TargetClipPad._ClipData._TintColor = Color.HSVToRGB(H, S, V);
                        _ClipEditorUI._TargetClipPad._ClipData._TintColor.a = alpha;
                        _ClipEditorUI.Refresh();
                        break;
                    //Clip Alpha
                    case 4:
                        _ClipEditorUI._TargetClipPad._ClipData._TintColor.a = ccValue;
                        _ClipEditorUI.Refresh();
                        break;
                    //Luma Key
                    case 5:
                        _ClipEditorUI._TargetClipPad._ClipData._LumaKey = ccValue;
                        _ClipEditorUI.Refresh();
                        break;
                    //Luma Blend
                    case 6:
                        _ClipEditorUI._TargetClipPad._ClipData._LumaBlend = ccValue;
                        _ClipEditorUI.Refresh();
                        break;
                    //Colour Mask
                    case 7:
                        _ClipEditorUI._TargetClipPad._ClipData._ColorMask = Mathf.Lerp(1f, 15f, ccValue);
                        _ClipEditorUI.Refresh();
                        break;
                    //Vid Start Point;
                    case 8:
                        _ClipEditorUI._TargetClipPad._ClipData._VideoStartMs = Mathf.Lerp(0, _ClipEditorUI._TargetClipPad._ClipData._VideoEndMs -1, ccValue);
                        _ClipEditorUI.Refresh(); 
                        break;
                    //Vid End Point;
                    case 9:
                        _ClipEditorUI._TargetClipPad._ClipData._VideoEndMs = Mathf.Lerp(_ClipEditorUI._TargetClipPad._ClipData._VideoStartMs + 1, _ClipEditorUI._TargetClipPad._ClipData._VideoLengthMs, ccValue);
                        _ClipEditorUI.Refresh();
                        break;
                }
            }
        }
    }

    public void FocusClipEditor(ClipPadUI clipPadUI)
    {
        if(_ClipEditorUI._TargetClipPad != null)
            _ClipEditorUI._TargetClipPad._ClipStateImage.color = Color.red;

        _ClipEditorUI._TargetClipPad = clipPadUI;
        _ClipEditorUI._TargetClipPad._ClipStateImage.color = Color.green;
        _ClipEditorUI.Refresh();
    }

    public void GetClipPreview(string filePath, ClipPadUI cpu)
    {
        _CurrentPreviewingClip = cpu;
        _PreviewPlayer.gameObject.SetActive(true);
        StartCoroutine(GrabPreviewFrameAsync(filePath));
    }

    IEnumerator GrabPreviewFrameAsync(string filePath)
    { 
        _PreviewPlayer.OpenVideoFromFile(RenderHeads.Media.AVProVideo.MediaPlayer.FileLocation.AbsolutePathOrURL, filePath, true);
        while (!_PreviewPlayer.Control.IsPlaying())
            yield return new WaitForEndOfFrame();
        int width = _PreviewPlayer.Info.GetVideoWidth();
        int height = _PreviewPlayer.Info.GetVideoHeight();
        Texture2D prevImg = new Texture2D(width, height);
        _PreviewPlayer.ExtractFrameAsync(prevImg, ExtractFrameCallback, 0.25f);
    }

    void ExtractFrameCallback(Texture2D frame)
    {
        _CurrentPreviewingClip.OnClipPreviewComplete(frame);
        _PreviewPlayer.gameObject.SetActive(false);
        _CurrentPreviewingClip = null;
    }

    public void NewProjectClicked()
    {
        _LoadedProjectPath = null;
        _ProjectName = "<New Project>";

        foreach (ClipPadUI cpu in _ClipPads)
        {
            cpu.Clear();
        }
        foreach (VideoPlayerController vpc in _VideoPlayerControllers)
            vpc.SetState(MediaPlayerState.Idle);
    }

    public void SaveAsProjectClicked()
    {
        string startPath = PlayerPrefs.GetString("LastProjectPath", null);
        FileBrowser.ShowSaveDialog(SaveProject, null, false, startPath);
    }

    public void SaveProjectClicked()
    {
        if (!string.IsNullOrEmpty(_LoadedProjectPath))
        {
            SaveProject(_LoadedProjectPath);
        }
        else
        {
            SaveAsProjectClicked();
        }
    }

    private void SaveProject(string filePath)
    {
        System.IO.FileInfo fileInfo = new System.IO.FileInfo(filePath);
        PlayerPrefs.SetString("LastProjectPath", fileInfo.Directory.FullName);
        string fileName = System.IO.Path.GetFileName(filePath);
        _ProjectName = fileName;
        Project proj = new Project();
        proj._Clips = new ProjectClip[16];

        int indexer = 0;
        foreach (ClipPadUI cpu in _ClipPads)
        {
            proj._Clips[indexer] = cpu._ClipData;
            indexer++;
        }
        JsonSerialisationHelper.Save<Project>(filePath, proj);
    }

    public void LoadProjectClicked()
    {
        string startPath = PlayerPrefs.GetString("LastProjectPath", null);
        FileBrowser.ShowLoadDialog(OnLoadProjectSuccess, null, false, startPath);
    }

    private void OnLoadProjectSuccess(string filePath)
    {
        System.IO.FileInfo fileInfo = new System.IO.FileInfo(filePath);
        PlayerPrefs.SetString("LastProjectPath", fileInfo.Directory.FullName);
        string fileName = System.IO.Path.GetFileName(filePath);
        _ProjectName = fileName;
        Project loadedProj = (Project)JsonSerialisationHelper.LoadFromFile<Project>(filePath);
        int i = 0;
        foreach (ProjectClip clipData in loadedProj._Clips)
        {
            if (clipData != null)
            {
                _ClipPads[i].LoadFromClipData(clipData);
                i++;
            }
            else
            {
                _ClipPads[i].Clear();
            }
        }
        _LoadedProjectPath = filePath;
    }

    public void PlayClip(string clipPath, ClipPadUI clipPad)
    {
        _VideoPlayerControllers[_VPCIndex].PlayClip(clipPath, clipPad);
        _VPCIndex++;
        if(_VPCIndex >= _VideoPlayerControllers.Length)
        {
            _VPCIndex = 0;
        }
    }
}

