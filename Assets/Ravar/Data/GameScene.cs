using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

namespace Itsdits.Ravar.Data
{
    /// <summary>
    /// Base class used to create data ScriptableObjects for level and menu scenes.
    /// </summary>
    public class GameScene : ScriptableObject
    {
        [Header("Information")]
        [Tooltip("Name of this scene. Used for loading and reference.")]
        [SerializeField] private string _sceneName;
        [Tooltip("Description of this scene used for clarification about what this scene contains and does.")]
        [SerializeField] private string _shortDescription;
 
        [Header("Sounds")]
        [Tooltip("Background music to be played when this scene is active.")]
        [SerializeField] private AudioClip _music;
        [Tooltip("Volume of the background music being played.")]
        [Range(0.0f, 1.0f)]
        [SerializeField] private float _musicVolume;
 
        [Header("Visuals")]
        [Tooltip("Post processing profile to be applied to the camera in this scene.")]
        [SerializeField] private PostProcessProfile _postprocess;
    }
}