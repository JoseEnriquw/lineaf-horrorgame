using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using ThunderWire.Attributes;
using TMPro;

namespace UHFPS.Runtime
{
    [InspectorHeader("Options Slider")]
    public class OptionsSlider : OptionBehaviour
    {
        public enum SliderTypeEnum { FloatSlider, IntegerSlider }

        // Tipo de volumen que controla este slider
        public enum VolumeCategory
        {
            Master,   // volumen general
            Music     // sólo música (menú + ambiente)
        }

        public TMP_Text SliderText;
        public Slider Slider;

        [Header("Slider Settings")]
        public SliderTypeEnum SliderType = SliderTypeEnum.FloatSlider;
        public MinMax SliderLimits = new(0, 1);
        public float SliderValue = 1f;

        [Header("Snap Settings")]
        public bool UseSnapping;
        public float SnapValue = 0.05f;

        [Header("Audio Targets")]
        // Para este slider: las fuentes que va a controlar
        public AudioSource[] TargetAudioSources;

        [Header("Volume Category")]
        public VolumeCategory Category = VolumeCategory.Master;

        // Factores globales
        private static float MasterVolume = 1f;
        private static float MusicVolume = 1f;

        // Para poder actualizar todos los sliders cuando cambia uno
        private static readonly List<OptionsSlider> AllSliders = new();

        private void Awake()
        {
            AllSliders.Add(this);
        }

        private void OnDestroy()
        {
            AllSliders.Remove(this);
        }

        private void Start()
        {
            Slider.minValue = SliderLimits.RealMin;
            Slider.maxValue = SliderLimits.RealMax;

            ApplyCategoryValue(SliderValue);

            Slider.value = SliderValue;
            Slider.onValueChanged.AddListener(SetSliderValue);

            UpdateAllVolumes();
            SliderText.text = SliderValue.ToString("0.00");
        }

        public override void SetOptionValue(object value)
        {
            SetSliderValue((float)value);
            Slider.value = SliderValue;
            IsChanged = false;
        }

        public override object GetOptionValue()
        {
            return SliderType switch
            {
                SliderTypeEnum.FloatSlider => SliderValue,
                SliderTypeEnum.IntegerSlider => Mathf.RoundToInt(SliderValue),
                _ => SliderValue
            };
        }

        public override void SetOptionData(StorableCollection data)
        {
            if (data.TryGetValue("settings", out object[] settings))
            {
                SliderType = (SliderTypeEnum)settings[0];
                SliderLimits = (MinMax)settings[1];
                UseSnapping = (bool)settings[2];
                SnapValue = (float)settings[3];

                Slider.minValue = SliderLimits.RealMin;
                Slider.maxValue = SliderLimits.RealMax;
            }

            if (data.TryGetValue("defaultValue", out object value))
            {
                SliderValue = (float)value;
            }

            Slider.wholeNumbers = SliderType == SliderTypeEnum.IntegerSlider;

            ApplyCategoryValue(SliderValue);
            Slider.value = SliderValue;
            UpdateAllVolumes();

            SliderText.text = SliderValue.ToString("0.00");
        }

        public void SetSliderValue(float value)
        {
            if (SliderType == SliderTypeEnum.FloatSlider)
                SliderValue = (float)Math.Round(value, 2);
            else if (SliderType == SliderTypeEnum.IntegerSlider)
                SliderValue = Mathf.RoundToInt(value);

            if (UseSnapping)
                SliderValue = SnapTo(SliderValue, SnapValue);

            ApplyCategoryValue(SliderValue);
            UpdateAllVolumes();

            SliderText.text = SliderValue.ToString("0.00");
            IsChanged = true;
        }

        /// <summary>
        /// Asigna el valor del slider a la categoría que le toque (Master o Music)
        /// </summary>
        private void ApplyCategoryValue(float value)
        {
            switch (Category)
            {
                case VolumeCategory.Master:
                    MasterVolume = value;
                    break;
                case VolumeCategory.Music:
                    MusicVolume = value;
                    break;
            }
        }

        /// <summary>
        /// Recalcula el volumen de todos los sliders registrados.
        /// </summary>
        private static void UpdateAllVolumes()
        {
            foreach (var slider in AllSliders)
            {
                if (slider != null)
                    slider.UpdateAudioVolume();
            }
        }

        /// <summary>
        /// Aplica el volumen efectivo a los AudioSource que controla ESTE slider.
        /// </summary>
        private void UpdateAudioVolume()
        {
            if (TargetAudioSources == null) return;

            float factor = 1f;

            // Master afecta todo. Music afecta sólo música pero también respeta el Master.
            switch (Category)
            {
                case VolumeCategory.Master:
                    factor = MasterVolume;
                    break;
                case VolumeCategory.Music:
                    factor = MasterVolume * MusicVolume;
                    break;
            }

            foreach (var src in TargetAudioSources)
            {
                if (src != null)
                    src.volume = factor;   // 0–1
            }
        }

        private float SnapTo(float value, float multiple)
        {
            return Mathf.Round(value / multiple) * multiple;
        }
    }
}
