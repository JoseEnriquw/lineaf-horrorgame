using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Newtonsoft.Json.Linq;
using ThunderWire.Attributes;

namespace UHFPS.Runtime
{
    /// <summary>
    /// Controla el estado de energía del ascensor, incluyendo:
    /// - Habilitación/Deshabilitación del sistema del ascensor
    /// - Control de luces con fade suave
    /// - Control de botones de interacción (ElevatorInteract y ElevatorCall)
    /// - Reproducción de efectos de sonido
    /// - Persistencia de datos (ISaveable)
    /// </summary>
    [InspectorHeader("Elevator Power Control")]
    public class ElevatorPowerControl : MonoBehaviour, ISaveable
    {
        #region References

        [Header("References")]
        [SerializeField]
        private ElevatorSystem elevatorSystem;

        [SerializeField]
        private List<Light> elevatorLights = new();

        [SerializeField]
        private AudioSource audioSource;

        #endregion

        #region Power Settings

        [Header("Power Settings")]
        [SerializeField]
        private bool isPowered = false;

        [SerializeField]
        [Range(0f, 1f)]
        private float lightIntensityWhenPowered = 1f;

        [SerializeField]
        private float powerToggleDuration = 0.5f;
        [SerializeField] private List<ElevatorInteract> elevatorCallButtons = new();

        #endregion

        #region Audio Settings

        [Header("Audio Settings")]
        [SerializeField]
        private SoundClip powerOnSound;

        [SerializeField]
        private SoundClip powerOffSound;

        [SerializeField]
        private SoundClip powerBuzzSound;

        #endregion

        #region Events

        [Header("Events")]
        public UnityEvent OnPowerEnabled;
        public UnityEvent OnPowerDisabled;
        public UnityEvent<bool> OnPowerStateChanged;

        #endregion

        #region Private Fields

        private List<ElevatorInteract> elevatorInteractButtons = new();
        private float lightFadeTimer = 0f;
        private bool isFadingLights = false;
        private bool previousPowerState;

        #endregion

        #region Lifecycle

        private void Awake()
        {
            ValidateReferences();
            CacheInteractButtons();
        }

        private void Start()
        {
            previousPowerState = isPowered;
            ApplyInitialPowerState();
        }

        private void Update()
        {
            UpdateLightFade();
        }

        #endregion

        #region Validation and Initialization

        private void ValidateReferences()
        {
            if (elevatorSystem == null)
                Debug.LogError($"[{nameof(ElevatorPowerControl)}] ElevatorSystem no asignado en {gameObject.name}", gameObject);

            if (audioSource == null)
                Debug.LogWarning($"[{nameof(ElevatorPowerControl)}] AudioSource no asignado en {gameObject.name}", gameObject);

            if (elevatorLights.Count == 0)
                Debug.LogWarning($"[{nameof(ElevatorPowerControl)}] No hay luces asignadas en {gameObject.name}", gameObject);
        }

        private void CacheInteractButtons()
        {
            ElevatorInteract[] interactButtons = GetComponentsInChildren<ElevatorInteract>();
            elevatorInteractButtons = new List<ElevatorInteract>(interactButtons);
        }

        private void ApplyInitialPowerState()
        {
            if (isPowered)
            {
                EnableAllLights(1f);
                EnableElevatorSystem();
                EnableInteractButtons();
            }
            else
            {
                DisableAllLights(0f);
                DisableElevatorSystem();
                DisableInteractButtons();
            }
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Habilita toda la energía del ascensor
        /// </summary>
        public void PowerEnabled()
        {
            if (isPowered)
                return;

            isPowered = true;
            HandlePowerStateChange(true);
        }

        /// <summary>
        /// Deshabilita toda la energía del ascensor
        /// </summary>
        public void PowerDisabled()
        {
            if (!isPowered)
                return;

            isPowered = false;
            HandlePowerStateChange(false);
        }

        /// <summary>
        /// Alterna el estado de energía del ascensor
        /// </summary>
        public void TogglePower()
        {
            if (isPowered)
                PowerDisabled();
            else
                PowerEnabled();
        }

        /// <summary>
        /// Obtiene el estado actual de energía
        /// </summary>
        public bool GetPowerState()
        {
            return isPowered;
        }

        /// <summary>
        /// Obtiene si las luces están encendidas
        /// </summary>
        public bool AreLightsOn()
        {
            foreach (Light light in elevatorLights)
            {
                if (light != null && light.enabled)
                    return true;
            }
            return false;
        }

        #endregion

        #region Power State Management

        private void HandlePowerStateChange(bool newState)
        {
            if (newState)
            {
                EnableElevatorPower();
            }
            else
            {
                DisableElevatorPower();
            }

            OnPowerStateChanged?.Invoke(isPowered);
        }

        private void EnableElevatorPower()
        {
            PlayAudioClip(powerOnSound);
            FadeLightsIn();
            EnableElevatorSystem();
            EnableInteractButtons();

            OnPowerEnabled?.Invoke();

            Debug.Log($"[{nameof(ElevatorPowerControl)}] Energía del ascensor ACTIVADA", gameObject);
        }

        private void DisableElevatorPower()
        {
            PlayAudioClip(powerOffSound);
            FadeLightsOut();
            ForceStopElevatorIfMoving();
            DisableElevatorSystem();
            DisableInteractButtons();

            OnPowerDisabled?.Invoke();

            Debug.Log($"[{nameof(ElevatorPowerControl)}] Energía del ascensor DESACTIVADA", gameObject);
        }

        #endregion

        #region Elevator System Control

        private void EnableElevatorSystem()
        {
            if (elevatorSystem != null)
                elevatorSystem.enabled = true;
        }

        private void DisableElevatorSystem()
        {
            if (elevatorSystem != null)
                elevatorSystem.enabled = false;
        }

        private void ForceStopElevatorIfMoving()
        {
            if (elevatorSystem == null)
                return;

            // Si el ascensor está en movimiento o las puertas están abiertas, detenerlo
            if (elevatorSystem.State == ElevatorSystem.ElevatorState.Moving ||
                elevatorSystem.State == ElevatorSystem.ElevatorState.DoorOpen ||
                elevatorSystem.State == ElevatorSystem.ElevatorState.DoorOpening)
            {
                elevatorSystem.StopAllCoroutines();
                Debug.Log($"[{nameof(ElevatorPowerControl)}] Ascensor detenido de emergencia", gameObject);
            }
        }

        #endregion

        #region Interact Buttons Control

        private void EnableInteractButtons()
        {
            foreach (ElevatorInteract button in elevatorInteractButtons)
            {
                if (button != null)
                {
                    button.SetCanInteract(true);
                }
            }

            foreach (var button in elevatorCallButtons)
            {
                if (button != null)
                {
                    button.SetCanInteract(true);
                }

            }
        }

        private void DisableInteractButtons()
        {
            foreach (ElevatorInteract button in elevatorInteractButtons)
            {
                if (button != null)
                {
                    button.SetEmission(false);
                    button.SetCanInteract(false);
                }
            }

            foreach (var button in elevatorCallButtons)
            {
                if (button != null)
                {
                    button.SetEmission(false);
                    button.SetCanInteract(false);
                }
            }
        }

        #endregion

        #region Light Control

        private void FadeLightsIn()
        {
            lightFadeTimer = 0f;
            isFadingLights = true;
        }

        private void FadeLightsOut()
        {
            lightFadeTimer = 1f;
            isFadingLights = true;
        }

        private void UpdateLightFade()
        {
            if (!isFadingLights)
                return;

            lightFadeTimer += Time.deltaTime / powerToggleDuration;

            if (isPowered)
            {
                // Fade in
                float t = Mathf.Clamp01(lightFadeTimer);
                EnableAllLights(t);

                if (t >= 1f)
                    isFadingLights = false;
            }
            else
            {
                // Fade out
                float t = Mathf.Clamp01(1f - lightFadeTimer);
                DisableAllLights(t);

                if (lightFadeTimer >= 1f)
                    isFadingLights = false;
            }
        }

        private void EnableAllLights(float intensity)
        {
            foreach (Light light in elevatorLights)
            {
                if (light != null)
                {
                    light.enabled = true;
                    //light.intensity = intensity * lightIntensityWhenPowered;
                }
            }
        }

        private void DisableAllLights(float intensity)
        {
            foreach (Light light in elevatorLights)
            {
                if (light != null)
                {
                    //light.intensity = intensity * lightIntensityWhenPowered;

                   // if (intensity <= 0f)
                        light.enabled = false;
                }
            }
        }

        #endregion

        #region Audio

        private void PlayAudioClip(SoundClip soundClip)
        {
            if (audioSource == null || soundClip.audioClip == null)
                return;

            audioSource.PlayOneShot(soundClip.audioClip, soundClip.volume);
        }

        #endregion

        #region ISaveable Implementation

        public StorableCollection OnSave()
        {
            return new StorableCollection()
            {
                { "powered", isPowered }
            };
        }

        public void OnLoad(JToken data)
        {
            isPowered = (bool)data["powered"];
            ApplyInitialPowerState();
        }

        #endregion

        #region Editor Validation

#if UNITY_EDITOR
        private void OnDrawGizmosSelected()
        {
            if (elevatorLights.Count == 0)
            {
                Debug.LogWarning($"[{nameof(ElevatorPowerControl)}] No hay luces asignadas en {gameObject.name}", gameObject);
            }

            if (elevatorSystem == null)
            {
                Debug.LogWarning($"[{nameof(ElevatorPowerControl)}] ElevatorSystem no asignado en {gameObject.name}", gameObject);
            }
        }
#endif

        #endregion
    }
}