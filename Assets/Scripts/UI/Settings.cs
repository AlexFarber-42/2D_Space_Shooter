using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class Settings : MonoBehaviour
{
    [Header("Sliders")]
    [SerializeField] private Slider masterVSlider;               // Handles the overall volume of the game
    [SerializeField] private Slider musicVSlider;                // Handles the volume of the music in the game
    [SerializeField] private Slider sfxVSlider;                  // Handles the volume of the sound effects in the game
    [SerializeField] private TextMeshProUGUI masterVText;        // Text that shows the percentage of the master volume
    [SerializeField] private TextMeshProUGUI musicVText;         // Text that shows the percentage of the music volume
    [SerializeField] private TextMeshProUGUI sfxVText;           // Text that shows the percentage of the sfx volume

    [Header("Dropdowns")]
    [SerializeField] private TMP_Dropdown resolutionDropdown;    // Dropdown that shows the available resolutions the user can choose to set the game too
    [SerializeField] private TMP_Dropdown displayDropdown;       // Dropdown that shows the available displays the user can choose to set the game too
    [SerializeField] private TMP_Dropdown screenModeDropdown;    // Dropdown that shows the available screen modes (i.e. fullscreen, windowed, etc.)

    private Resolution[] resolutions;
    private List<Resolution> filteredResolutions;

    private List<DisplayInfo> displays = new List<DisplayInfo>();

    private static readonly FullScreenMode[] fullScreenModes = 
    {
#if UNITY_STANDALONE_WIN
        FullScreenMode.ExclusiveFullScreen,         // Exclusive full screen is only supported on Windows Standalone player
#endif
        FullScreenMode.FullScreenWindow,
        FullScreenMode.MaximizedWindow,
        FullScreenMode.Windowed
    };

    private double currentRefreshRate;

    private int currentResolutionIndex     = 0;
    private int currentDisplayIndex        = 0;
    private int currentScreenModeIndex     = 0;

    private const float MasterDefault      = 1f;
    private const float MusicDefault       = 1f;
    private const float SFXDefault         = 1f;

    private float masterVol;
    private float musicVol;
    private float sfxVol;

    private const string MasterKey         = "MasterVolume";
    private const string MusicKey          = "MusicVolume";
    private const string SFXKey            = "SFXVolume";
    private const string ResolutionKey     = "Resolution";
    private const string DisplayKey        = "Display";
    private const string ScreenModeKey     = "ScreenMode";

    private void Start()
    {
        DisplaySetUp();
        ResolutionSetUp();
        ScreenModeSetUp();
        VolumeSetUp();
    }

    private void DisplaySetUp()
    {
        Screen.GetDisplayLayout(displays);
        DisplayInfo currentDisplay = Screen.mainWindowDisplayInfo;

        // Lists all available displays
        List<string> displayOptions = new List<string>();
        for (int i = 0; i < displays.Count; ++i)
        {
            DisplayInfo info = displays[i];
            displayOptions.Add(info.name);

            if (info.Equals(currentDisplay))
                currentDisplayIndex = i;
        }

        try
        {
            currentDisplayIndex = int.Parse(PlayerPrefs.GetString(DisplayKey));
        }
        catch 
        {
            PlayerPrefs.SetString(DisplayKey, currentDisplayIndex.ToString());
            PlayerPrefs.Save();
        }

        displayDropdown.ClearOptions();
        displayDropdown.AddOptions(displayOptions);
        displayDropdown.value = currentDisplayIndex;
        displayDropdown.RefreshShownValue();
    }

    private void ResolutionSetUp()
    {
        resolutions = Screen.resolutions;
        filteredResolutions = new List<Resolution>();
        currentRefreshRate = Screen.currentResolution.refreshRateRatio.value;

        // If the value of the refresh rate of the system matches a gathered resolution's refresh rate, add it to the filtered list
        for (int i = 0; i < resolutions.Length; ++i)
        {
            if (resolutions[i].refreshRateRatio.value == currentRefreshRate)
                filteredResolutions.Add(resolutions[i]);
        }

        List<string> resolutionOptions = new List<string>();
        for (int i = 0; i < filteredResolutions.Count; ++i)
        {
            string resoOption = $"{filteredResolutions[i].width}x{filteredResolutions[i].height} {filteredResolutions[i].refreshRateRatio.value}hz";
            resolutionOptions.Add(resoOption);

            if (filteredResolutions[i].width == Screen.width && filteredResolutions[i].height == Screen.height)
                currentResolutionIndex = i;
        }

        try
        {
            currentResolutionIndex = int.Parse(PlayerPrefs.GetString(ResolutionKey));
        }
        catch
        {
            PlayerPrefs.SetString(ResolutionKey, currentResolutionIndex.ToString());
            PlayerPrefs.Save();
        }

        resolutionDropdown.ClearOptions();
        resolutionDropdown.AddOptions(resolutionOptions);
        resolutionDropdown.value = currentResolutionIndex;
        resolutionDropdown.RefreshShownValue();
    }

    private void ScreenModeSetUp()
    {
        FullScreenMode currentFullScreenMode = Screen.fullScreenMode;

        List<string> options = new List<string>();

        for (int i = 0; i < fullScreenModes.Length; ++i)
        {
            if (fullScreenModes[i] == currentFullScreenMode)
                currentScreenModeIndex = i;

            options.Add(fullScreenModes[i].ToString());
        }

        try
        {
            currentScreenModeIndex = int.Parse(PlayerPrefs.GetString(ScreenModeKey));
        }
        catch
        {
            PlayerPrefs.SetString(ScreenModeKey, currentScreenModeIndex.ToString());
            PlayerPrefs.Save();
        }

        screenModeDropdown.ClearOptions();
        screenModeDropdown.AddOptions(options);
        screenModeDropdown.value = currentScreenModeIndex;
        screenModeDropdown.RefreshShownValue();
    }

    private void VolumeSetUp()
    {
        try
        {
            masterVol   = PlayerPrefs.GetFloat(MasterKey);
            musicVol    = PlayerPrefs.GetFloat(MusicKey);
            sfxVol      = PlayerPrefs.GetFloat(SFXKey);
        }
        catch
        {
            masterVol   = MasterDefault;
            musicVol    = MusicDefault;
            sfxVol      = SFXDefault;

            PlayerPrefs.SetFloat(MasterKey, masterVol);
            PlayerPrefs.SetFloat(MusicKey, musicVol);
            PlayerPrefs.SetFloat(SFXKey, sfxVol);
            PlayerPrefs.Save();
        }

        masterVSlider.value = masterVol;
        musicVSlider.value  = musicVol;
        sfxVSlider.value    = sfxVol;

        ChangeMasterVolume();
        ChangeMusicVolume();
        ChangeSFXVolume();

    }

    public void SetResolution(int resIndex)
    {
        Resolution res = filteredResolutions[resIndex];
        Screen.SetResolution(res.width, res.height, fullScreenModes[currentScreenModeIndex], Screen.currentResolution.refreshRateRatio);
        currentResolutionIndex = resIndex;

        PlayerPrefs.SetString(ResolutionKey, currentResolutionIndex.ToString());
        PlayerPrefs.Save();
    }

    public void SetDisplay(int disIndex)
        => StartCoroutine(MoveToDisplay(disIndex));

    private IEnumerator MoveToDisplay(int disIndex)
    {
        DisplayInfo display = displays[disIndex];

        Vector2Int targetCoords = Vector2Int.zero;
        if (Screen.fullScreenMode != FullScreenMode.Windowed)
        {
            targetCoords.x += display.width / 2;
            targetCoords.y += display.height / 2;
        }

        var moveOp = Screen.MoveMainWindowTo(display, targetCoords);
        yield return moveOp;

        PlayerPrefs.SetString(DisplayKey, currentDisplayIndex.ToString()); 
        PlayerPrefs.Save();
    }

    public void SetScreenMode(int modeIndex)
    {
        FullScreenMode newMode = fullScreenModes[modeIndex];

        if (Screen.fullScreenMode != newMode)
        {
            if (newMode == FullScreenMode.Windowed)
                Screen.fullScreenMode = newMode;
            else
            {
                DisplayInfo display = Screen.mainWindowDisplayInfo;
                Screen.SetResolution(display.width, display.height, newMode, Screen.currentResolution.refreshRateRatio);
            }
        }

        currentScreenModeIndex = modeIndex;
        PlayerPrefs.SetString(ScreenModeKey, currentScreenModeIndex.ToString());
        PlayerPrefs.Save();
    }

    public void ChangeMasterVolume()
    {
        masterVol = masterVSlider.value;
        masterVText.text = $"{(int)(masterVol * 100)}%";

        PlayerPrefs.SetFloat(MasterKey, masterVol);
        PlayerPrefs.Save();

        ChangeMusicVolume();
        ChangeSFXVolume();
    }

    private float VerifiedMusicVolume => musicVol * masterVol;

    public void ChangeMusicVolume()
    {
        musicVol = musicVSlider.value;
        musicVText.text = $"{(int)(musicVol * 100)}%";

        PlayerPrefs.SetFloat(MusicKey, musicVol);
        PlayerPrefs.Save();

        MusicManager.Instance.SetVolume(VerifiedMusicVolume);
    }

    private float VerifiedSFXVolume => sfxVol * masterVol;

    public void ChangeSFXVolume()
    {
        sfxVol = sfxVSlider.value;
        sfxVText.text = $"{(int)(sfxVol * 100)}%";

        PlayerPrefs.SetFloat(SFXKey, sfxVol);
        PlayerPrefs.Save();

        SFXManager.Instance.SetVolume(VerifiedSFXVolume);
    }
}
