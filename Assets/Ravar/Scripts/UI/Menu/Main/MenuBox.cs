using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Itsdits.Ravar.UI
{
    /// <summary>
    /// Container class that holds references to the menu objects and screens. Also handles input and selection.
    /// </summary>
    public class MenuBox : MonoBehaviour
    {
        [Header("Main Menu")]
        [SerializeField] List<Text> mainTexts;
        [SerializeField] GameObject mainMenu;

        [Header("Loader Menu")]
        [SerializeField] List<Text> loaderTexts;
        [SerializeField] GameObject loaderMenu;

        [Header("Settings Menu")]
        [SerializeField] List<Text> settingsTexts;
        [SerializeField] GameObject settingsMenu;

        [Header("Info Menu")]
        [SerializeField] List<Text> infoTexts;
        [SerializeField] GameObject infoMenu;

        [Header("Variables")]
        [SerializeField] Color highlightColor;
        [SerializeField] Color standardColor;

        public List<Text> MainTexts => mainTexts;
        public List<Text> LoaderTexts => loaderTexts;
        public List<Text> SettingsTexts => settingsTexts;
        public List<Text> InfoTexts => infoTexts;

        public void UpdateMainSelector(int selected)
        {
            for (int i = 0; i < mainTexts.Count; ++i)
            {
                if (i == selected)
                {
                    mainTexts[i].color = highlightColor;
                }
                else
                {
                    mainTexts[i].color = standardColor;
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

        public void UpdateInfoSelector(int selected)
        {
            //TODO - implement this and the corresponding UI screen
            for (int i = 0; i < infoTexts.Count; ++i)
            {
                if (i == selected)
                {
                    infoTexts[i].color = highlightColor;
                }
                else
                {
                    infoTexts[i].color = standardColor;
                }
            }
        }

        public void EnableMainMenu(bool enabled)
        {
            mainMenu.SetActive(enabled);
        }

        public void EnableLoader(bool enabled)
        {
            loaderMenu.SetActive(enabled);
        }

        public void EnableSettings(bool enabled)
        {
            settingsMenu.SetActive(enabled);
        }

        public void EnableInfo(bool enabled)
        {
            infoMenu.SetActive(enabled);
        }
    }
}
