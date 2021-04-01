using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Itsdits.Ravar.UI
{
    /// <summary>
    /// Container class that holds references to the pause menu objects and screens.
    /// </summary>
    public class PauseBox : MonoBehaviour
    {
        [Header("Pause Menu")]
        [SerializeField] List<Text> pauseTexts;
        [SerializeField] GameObject pauseMenu;

        [Header("Save Menu")]
        [SerializeField] List<Text> saveTexts;
        [SerializeField] GameObject saveMenu;

        [Header("Loader Menu")]
        [SerializeField] List<Text> loaderTexts;
        [SerializeField] GameObject loaderMenu;

        [Header("Settings Menu")]
        [SerializeField] List<Text> settingsTexts;
        [SerializeField] GameObject settingsMenu;

        [Header("Variables")]
        [SerializeField] Color highlightColor;
        [SerializeField] Color standardColor;

        public List<Text> PauseTexts => pauseTexts;
        public List<Text> SaveTexts => saveTexts;
        public List<Text> LoaderTexts => loaderTexts;
        public List<Text> SettingsTexts => settingsTexts;

        public void UpdatePauseSelector(int selected)
        {
            for (int i = 0; i < pauseTexts.Count; ++i)
            {
                if (i == selected)
                {
                    pauseTexts[i].color = highlightColor;
                }
                else
                {
                    pauseTexts[i].color = standardColor;
                }
            }
        }

        public void UpdateSaveSelector(int selected)
        {
            //TODO - implement this and the corresponding UI screen
            for (int i = 0; i < saveTexts.Count; ++i)
            {
                if (i == selected)
                {
                    saveTexts[i].color = highlightColor;
                }
                else
                {
                    saveTexts[i].color = standardColor;
                }
            }
        }

        public void UpdateLoaderSelector(int selected)
        {
            //TODO - implement this and the corresponding UI screen
            for (int i = 0; i < loaderTexts.Count; ++i)
            {
                if (i == selected)
                {
                    loaderTexts[i].color = highlightColor;
                }
                else
                {
                    loaderTexts[i].color = standardColor;
                }
            }
        }

        public void UpdateSettingsSelector(int selected)
        {
            //TODO - implement this and the corresponding UI screen
            for (int i = 0; i < settingsTexts.Count; ++i)
            {
                if (i == selected)
                {
                    settingsTexts[i].color = highlightColor;
                }
                else
                {
                    settingsTexts[i].color = standardColor;
                }
            }
        }

        public void EnablePauseMenu(bool enabled)
        {
            pauseMenu.SetActive(enabled);
        }

        public void EnableSave(bool enabled)
        {
            saveMenu.SetActive(enabled);
        }

        public void EnableLoader(bool enabled)
        {
            loaderMenu.SetActive(enabled);
        }

        public void EnableSettings(bool enabled)
        {
            settingsMenu.SetActive(enabled);
        }
    }
}
