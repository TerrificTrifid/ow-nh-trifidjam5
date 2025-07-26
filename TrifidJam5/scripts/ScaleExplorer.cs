using NewHorizons.Handlers;
using OWML.Common;
using System.Collections.Generic;
using UnityEngine;

namespace TrifidJam5
{
    public class ScaleExplorer : MonoBehaviour
    {
        public static ScaleExplorer Instance;
        public static float InnerExponentThreshold = -2;
        public static float OuterExponentThreshold = 1;
        public static float InnerScaleThreshold = Mathf.Pow(10, InnerExponentThreshold);
        public static float OuterScaleThreshold = Mathf.Pow(10, OuterExponentThreshold);
        public static float MinExponent = -35;
        public static float MaxExponent = 26;

        public OWTriggerVolume Trigger;
        public ScaleLevel[] Levels;
        private bool _activated = false;
        public int TargetExponent = 0;
        private float _exponent = 0;
        public float Speed = 1;
        private bool _shifting = false;

        public TextMesh ExponentText;
        public TextMesh ScaleText;
        public Dictionary<int, string> OrdersOfMagnitude;
        public Dictionary<int, string> OrdersOfMagnitude2;

        public ScaleExplorerMusic Music;
        public OWAudioSource OneShotSource;
        public OWAudioSource LoopSource;
        public AudioClip ActivateSound;
        public AudioClip DeactivateSound;
        public AudioClip AscendSound;
        public AudioClip DescendSound;
        public AudioClip LevelSound;

        public IInputCommands AscendKey = InputLibrary.toolOptionUp;
        public IInputCommands DescendKey = InputLibrary.toolOptionDown;
        private ScreenPrompt _ascendPrompt;
        private ScreenPrompt _descendPrompt;

        public void Awake()
        {
            Instance = this;
            Trigger.OnEntry += OnEntry;
            Trigger.OnExit += OnExit;
        }

        public void Start()
        {
            _ascendPrompt = new ScreenPrompt(AscendKey, TranslationHandler.GetTranslation("ScaleExplorer_Ascend", TranslationHandler.TextType.UI) + "   <CMD>");
            _descendPrompt = new ScreenPrompt(DescendKey, TranslationHandler.GetTranslation("ScaleExplorer_Descend", TranslationHandler.TextType.UI) + "   <CMD>");

            Levels = gameObject.GetComponentsInChildren<ScaleLevel>(true);
            foreach (ScaleLevel level in Levels)
            {
                level.gameObject.SetActive(true);
            }

            OrdersOfMagnitude = new Dictionary<int, string>
            {
                { -30, " quectometers" },
                { -27, " rontometers" },
                { -24, " yoctometers" },
                { -21, " zeptometers" },
                { -18, " attometers" },
                { -15, " femtometers" },
                { -12, " picometers" },
                { -10, " angstroms" },
                { -9, " nanometers" },
                //{ -8, " beard-seconds" },
                { -6, " micrometers" },
                { -3, " millimeters" },
                { -2, " centimeters" },
                { 0, " meters" },
                { 3, " kilometers" },
                //{ 6, " megameters" },
                { 9, " gigameters" },
                { 12, " terameters" },
                { 15, " petameters" },
                { 18, " exameters" },
                { 21, " zettameters" },
                { 24, " yottameters" },
                { 27, " ronnameters" },
                { 30, " quettameters" }
            };
            OrdersOfMagnitude2 = new Dictionary<int, string>
            {
                { -5, " twips" },
                { -4, " thou" },
                { -3, " barleycorns" },
                { -2, " inches" },
                { -1, " hands" },
                { 0, " feet" },
                { 1, " yards" },
                { 2, " chains" },
                { 3, " furlongs" },
                { 4, " miles" },
                { 5, " leagues" },
            };
        }

        public void Update()
        {
            if (_activated)
            {
                float rate = 0;
                if (OWInput.IsPressed(AscendKey, InputMode.Character) && _exponent < MaxExponent)
                {
                    rate += Speed;
                }
                if (OWInput.IsPressed(DescendKey, InputMode.Character) && _exponent > MinExponent)
                {
                    rate -= Speed;
                }

                // replace with smoothing
                if (_shifting && rate == 0)
                {
                    _shifting = false;
                    foreach (ScaleLevel level in Levels)
                    {
                        level.ApplyExponent(_exponent);
                    }
                    LoopSource.FadeOut(0.25f);
                }
                else if (!_shifting && rate != 0)
                {
                    _shifting = true;
                    OneShotSource.PlayOneShot(rate > 0 ? AscendSound : DescendSound);
                    LoopSource.FadeIn(0.25f);
                }

                if (_shifting)
                {
                    _exponent += rate * Time.deltaTime;
                    _exponent = Mathf.Max(_exponent, MinExponent);
                    _exponent = Mathf.Min(_exponent, MaxExponent);
                    foreach (ScaleLevel level in Levels)
                    {
                        level.ApplyExponent(_exponent);
                    }
                }

                ExponentText.text = _exponent.ToString("0.0");

                float n = LengthInUnits(_exponent, out string unit, false);
                if (string.IsNullOrEmpty(unit))
                {
                    ScaleText.text = "Out of metric scale";
                }
                else
                {
                    ScaleText.text = n.ToString("0.0") + unit;
                }
            }
        }

        private void OnEntry(GameObject hitobj)
        {
            var body = hitobj.GetAttachedOWRigidbody();
            if (!body.CompareTag("Player")) return;

            if (!_activated)
            {
                _activated = true;
                ShowPrompts(true);
                OneShotSource.PlayOneShot(ActivateSound);
                foreach (ScaleLevel level in Levels)
                {
                    level.ApplyExponent(_exponent);
                }
            }
        }

        private void OnExit(GameObject hitobj)
        {
            var body = hitobj.GetAttachedOWRigidbody();
            if (!body.CompareTag("Player")) return;

            if (_activated)
            {
                _activated = false;
                ShowPrompts(false);
                OneShotSource.PlayOneShot(DeactivateSound);
                foreach (ScaleLevel level in Levels)
                {
                    level.FadeOut();
                }
            }
        }

        public void ShowPrompts(bool show)
        {
            if (show)
            {
                Locator.GetPromptManager().AddScreenPrompt(_ascendPrompt, PromptPosition.UpperRight, true);
                Locator.GetPromptManager().AddScreenPrompt(_descendPrompt, PromptPosition.UpperRight, true);
            }
            else
            {
                Locator.GetPromptManager().RemoveScreenPrompt(_ascendPrompt, PromptPosition.UpperRight);
                Locator.GetPromptManager().RemoveScreenPrompt(_descendPrompt, PromptPosition.UpperRight);
            }
        }

        public float LengthInUnits(float exponent, out string unit, bool imperial = false)
        {
            float n = 0;
            unit = ""; 
            if (imperial)
            {

            }
            else
            {
                int exp = Mathf.FloorToInt(exponent);
                while (exp >= MinExponent && !OrdersOfMagnitude.ContainsKey(exp))
                {
                    exp--;
                }
                if (OrdersOfMagnitude.TryGetValue(exp, out unit))
                {
                    n = Mathf.Pow(10, exponent - exp);
                }
            }
            return n;
        }

        public enum LengthUnits
        {
            Metric,
            Imperial,
            Natural
        }
    }
}