using UnityEngine;
using System.Collections;

/// <summary>
/// Sistema de efectos visuales y sonoros para el elevador.
/// Crea atmósfera de horror con parpadeos, temblores y sonidos ambientales.
/// </summary>
public class ElevatorEffectsManager : MonoBehaviour
{
    [SerializeField] private ElevatorControllerEnhanced elevatorController;
    
    [Header("Efectos de Luces")]
    [SerializeField] private Light[] elevatorLights;
    [SerializeField] private Material[] lightEmissionMaterials;
    [SerializeField] private float lightFlickerDuration = 0.1f;
    [SerializeField] private AnimationCurve lightFlickerCurve = AnimationCurve.EaseInOut(0, 1, 1, 0);

    [Header("Efectos de Partículas")]
    [SerializeField] private ParticleSystem dustParticles;
    [SerializeField] private ParticleSystem sparkParticles;

    [Header("Sonidos Ambientales")]
    [SerializeField] private AudioSource ambientSound;
    [SerializeField] private float ambientVolume = 0.3f;
    [SerializeField] private AudioClip[] ambientClips;

    [Header("Efectos de Cámara")]
    [SerializeField] private Camera mainCamera;
    [SerializeField] private float cameraShakeIntensity = 0.1f;
    private Vector3 originalCameraPosition;

    [Header("Configuración de Horror")]
    [SerializeField] private bool enableHorrorEffects = true;
    [SerializeField] [Range(0, 1)] private float horrorIntensity = 0.5f;

    private Coroutine flickerCoroutine;
    private Coroutine shakeCoroutine;

    private void OnEnable()
    {
        if (elevatorController != null)
        {
            elevatorController.OnElevatorStartMoving += HandleElevatorStart;
            elevatorController.OnElevatorStopped += HandleElevatorStop;
            elevatorController.OnEmergencyStop += HandleEmergencyStop;
            elevatorController.OnShaking += HandleShaking;
            elevatorController.OnLightFlicker += HandleLightFlicker;
        }

        if (mainCamera == null)
        {
            mainCamera = Camera.main;
        }

        if (mainCamera != null)
        {
            originalCameraPosition = mainCamera.transform.localPosition;
        }

        if (ambientSound != null && ambientClips.Length > 0)
        {
            StartCoroutine(PlayAmbientSoundLoop());
        }
    }

    private void OnDisable()
    {
        if (elevatorController != null)
        {
            elevatorController.OnElevatorStartMoving -= HandleElevatorStart;
            elevatorController.OnElevatorStopped -= HandleElevatorStop;
            elevatorController.OnEmergencyStop -= HandleEmergencyStop;
            elevatorController.OnShaking -= HandleShaking;
            elevatorController.OnLightFlicker -= HandleLightFlicker;
        }
    }

    /// <summary>
    /// Maneja efectos cuando el elevador comienza a moverse
    /// </summary>
    private void HandleElevatorStart()
    {
        if (!enableHorrorEffects) return;

        // Inicia parpadeo de luces
        if (flickerCoroutine != null) StopCoroutine(flickerCoroutine);
        flickerCoroutine = StartCoroutine(FlickerLightsRoutine());

        // Activa partículas de polvo
        if (dustParticles != null && dustParticles.isStopped)
        {
            dustParticles.Play();
        }

        // Reproduce sonido de inicio
        PlayRandomAmbientSound();
    }

    /// <summary>
    /// Maneja efectos cuando el elevador se detiene
    /// </summary>
    private void HandleElevatorStop()
    {
        // Detiene parpadeo
        if (flickerCoroutine != null)
        {
            StopCoroutine(flickerCoroutine);
            flickerCoroutine = null;
        }

        // Restaura luces
        foreach (Light light in elevatorLights)
        {
            if (light != null) light.enabled = true;
        }

        // Detiene partículas de polvo
        if (dustParticles != null && dustParticles.isPlaying)
        {
            dustParticles.Stop();
        }
    }

    /// <summary>
    /// Maneja efectos de parada de emergencia
    /// </summary>
    private void HandleEmergencyStop()
    {
        if (!enableHorrorEffects) return;

        // Parpadeo rápido intenso
        StartCoroutine(IntenseFlickerRoutine());

        // Sparks particles
        if (sparkParticles != null)
        {
            sparkParticles.Play();
        }

        // Shake fuerte de cámara
        if (mainCamera != null)
        {
            if (shakeCoroutine != null) StopCoroutine(shakeCoroutine);
            shakeCoroutine = StartCoroutine(ShakeCameraRoutine(0.5f, 0.3f));
        }
    }

    /// <summary>
    /// Maneja el temblor del elevador
    /// </summary>
    private void HandleShaking(float magnitude)
    {
        if (!enableHorrorEffects) return;

        // Podrías añadir efectos adicionales aquí según la magnitud
        if (magnitude > 0.5f)
        {
            if (sparkParticles != null && Random.value > 0.7f)
            {
                sparkParticles.Emit(1);
            }
        }
    }

    /// <summary>
    /// Maneja el parpadeo de luces
    /// </summary>
    private void HandleLightFlicker(bool lightsOn)
    {
        foreach (Light light in elevatorLights)
        {
            if (light != null) light.enabled = lightsOn;
        }

        foreach (Material mat in lightEmissionMaterials)
        {
            if (mat != null)
            {
                mat.SetColor("_EmissionColor", lightsOn ? Color.white : Color.black);
            }
        }
    }

    /// <summary>
    /// Corrutina para parpadeo de luces suave
    /// </summary>
    private IEnumerator FlickerLightsRoutine()
    {
        while (true)
        {
            yield return new WaitForSeconds(Random.Range(1f, 3f));

            if (Random.value > 0.7f)
            {
                for (int i = 0; i < Random.Range(1, 4); i++)
                {
                    foreach (Light light in elevatorLights)
                    {
                        if (light != null) light.enabled = false;
                    }
                    yield return new WaitForSeconds(0.05f);

                    foreach (Light light in elevatorLights)
                    {
                        if (light != null) light.enabled = true;
                    }
                    yield return new WaitForSeconds(0.1f);
                }
            }
        }
    }

    /// <summary>
    /// Corrutina para parpadeo intenso (emergencia)
    /// </summary>
    private IEnumerator IntenseFlickerRoutine()
    {
        for (int i = 0; i < 10; i++)
        {
            foreach (Light light in elevatorLights)
            {
                if (light != null) light.enabled = !light.enabled;
            }
            yield return new WaitForSeconds(0.05f);
        }

        // Restaura al final
        foreach (Light light in elevatorLights)
        {
            if (light != null) light.enabled = true;
        }
    }

    /// <summary>
    /// Corrutina para shake de cámara
    /// </summary>
    private IEnumerator ShakeCameraRoutine(float duration, float intensity)
    {
        float elapsed = 0;

        while (elapsed < duration)
        {
            float randomX = Random.Range(-intensity, intensity);
            float randomY = Random.Range(-intensity, intensity);
            float randomZ = Random.Range(-intensity, intensity);

            mainCamera.transform.localPosition = originalCameraPosition + new Vector3(randomX, randomY, randomZ);

            elapsed += Time.deltaTime;
            yield return null;
        }

        mainCamera.transform.localPosition = originalCameraPosition;
    }

    /// <summary>
    /// Reproduce un sonido ambiente aleatorio
    /// </summary>
    private void PlayRandomAmbientSound()
    {
        if (ambientClips.Length == 0) return;

        AudioClip clip = ambientClips[Random.Range(0, ambientClips.Length)];
        if (ambientSound != null)
        {
            ambientSound.clip = clip;
            ambientSound.volume = ambientVolume * horrorIntensity;
            ambientSound.Play();
        }
    }

    /// <summary>
    /// Loop de sonido ambiente
    /// </summary>
    private IEnumerator PlayAmbientSoundLoop()
    {
        while (true)
        {
            yield return new WaitForSeconds(Random.Range(15f, 30f));
            PlayRandomAmbientSound();
        }
    }

    /// <summary>
    /// Método público para reproducir efecto visual personalizado
    /// </summary>
    public void PlayCustomEffect(string effectName)
    {
        switch (effectName)
        {
            case "LightFlicker":
                StartCoroutine(FlickerLightsRoutine());
                break;
            case "CameraShake":
                StartCoroutine(ShakeCameraRoutine(0.3f, 0.2f));
                break;
            case "Sparks":
                if (sparkParticles != null) sparkParticles.Play();
                break;
            case "Dust":
                if (dustParticles != null) dustParticles.Play();
                break;
        }
    }

    /// <summary>
    /// Configura la intensidad de los efectos de horror
    /// </summary>
    public void SetHorrorIntensity(float intensity)
    {
        horrorIntensity = Mathf.Clamp01(intensity);
        if (ambientSound != null)
        {
            ambientSound.volume = ambientVolume * horrorIntensity;
        }
    }
}
