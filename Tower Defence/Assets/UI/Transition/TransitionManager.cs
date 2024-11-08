using System.Collections;
using MaterialLibrary.HexagonSpread;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Localization.Settings;
using UnityEngine.Localization.Tables;
using UnityEngine.Rendering.Universal;
using UnityEngine.SceneManagement;

namespace UI.Transition
{
    /// <summary>
    /// Automatically animates the transition and manages the loading of new scenes
    /// </summary>
    public class TransitionManager : MonoBehaviour
    {
        // Triggers
        private static readonly int ClosingTrigger = Animator.StringToHash("Close");
        private static readonly int OpeningTrigger = Animator.StringToHash("Open");

        private Animator _animator;
        
        [Header("Visuals")]
        [Tooltip("The hexagon transition that fills the screen")]
        [SerializeField]
        private HexagonTransition transition;

        [Tooltip("The text object that displays the scene name that's being loaded")]
        [SerializeField]
        private TextMeshProUGUI sceneName;
        [Tooltip("The Localisation TableReference to find scene name localisations in.")]
        public TableReference tableReference;

        [Header("Rings")]
        [Tooltip("The inner ring that appears on the press location")]
        [SerializeField]
        private RectTransform innerRing;
        [Tooltip("The outer ring that appears on the press location")]
        [SerializeField]
        private RectTransform outerRing;

        [Header("System")]
        [Tooltip("The camera used to calculate the press location")]
        [SerializeField]
        // ReSharper disable once InconsistentNaming
        private Camera _camera;

        private string _loadingScene = string.Empty;

        /// <summary>
        /// Singleton pattern and finds its own animator
        /// </summary>
        private void Awake()
        {
            if (Instance != null)
            {
                Destroy(gameObject);
                return;
            }

            Instance = this;

            _animator = GetComponentInChildren<Animator>();

            SceneManager.sceneLoaded += SceneLoadEvent;
        }

        /// <summary>
        /// The singleton instance of the transition
        /// </summary>
        public static TransitionManager Instance { get; private set; }

        /// <summary>
        /// The duration of the closing transition
        /// </summary>
        private float TransitionDuration => Mathf.Max(transition.GetDuration(State.IN), _animator.GetCurrentAnimatorClipInfo(0)[0].clip.length);

        /// <summary>
        /// Runs the closing animation
        /// </summary>
        private void Close()
        {
            _animator.SetTrigger(ClosingTrigger);
        }

        /// <summary>
        /// Runs the opening animation
        /// </summary>
        private void Open()
        {
            _animator.SetTrigger(OpeningTrigger);
        }

        /// <summary>
        /// Handles the scene switch by playing the animation and waiting for it to finish
        /// </summary>
        /// <param name="newSceneName">The new scene to load</param>
        private IEnumerator Animate(string newSceneName)
        {
            Close();

            yield return new WaitForSecondsRealtime(TransitionDuration);
            
            _loadingScene = newSceneName;
            SceneManager.LoadScene(newSceneName);
        }

        /// <summary>
        /// The event when the scene has finished loading.<br/>
        /// Only runs the opening animation
        /// </summary>
        private void SceneLoadEvent(Scene scene, LoadSceneMode mode)
        {
            if (_loadingScene != scene.name) return;
            
            GameObject.FindGameObjectsWithTag("MainCamera")[0].GetComponent<Camera>().GetUniversalAdditionalCameraData().cameraStack.Add(_camera);
            Open();
            _loadingScene = null;
        }

        /// <summary>
        /// Loads a scene and handles transitions
        /// </summary>
        /// <param name="newSceneName">The scene to load</param>
        public void LoadScene(string newSceneName)
        {
            // Make sure time is back to normal
            Time.timeScale = 1f;
            
            // Get the location of the press that started the scene load
            Vector2 pointer = _camera.ScreenToWorldPoint(Pointer.current.position.ReadValue());

            // Update visuals to start at the click location
            transition.SetOrigin(State.IN, pointer);

            innerRing.position = pointer;
            outerRing.position = pointer;
            
            // Update the bottom text of the transition to match the new scene name
            var entryReference = (TableEntryReference)newSceneName;
            sceneName.text =
                new LocalizedStringDatabase().GetLocalizedString(tableReference, entryReference, LocalizationSettings.SelectedLocale);
            
            StartCoroutine(Animate(newSceneName));
        }
    }
}
