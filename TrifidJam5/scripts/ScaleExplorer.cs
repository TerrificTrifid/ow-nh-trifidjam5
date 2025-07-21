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
                { -30, " quectometer" },
                { -27, " rontometer" },
                { -24, " yoctometer" },
                { -21, " zeptometer" },
                { -18, " attometer" },
                { -15, " femtometer" },
                { -12, " picometer" },
                { -10, " angstrom" },
                { -9, " nanometer" },
                //{ -8, " beard-second" },
                { -6, " micrometer" },
                { -3, " millimeter" },
                { -2, " centimeter" },
                { 0, " meter" },
                { 3, " kilometer" },
                //{ 6, " megameter" },
                { 9, " gigameter" },
                { 12, " terameter" },
                { 15, " petameter" },
                { 18, " exameter" },
                { 21, " zettameter" },
                { 24, " yottameter" },
                { 27, " ronnameter" },
                { 30, " quettameter" }
            };
        }

        public void Update()
        {
            if (_activated)
            {
                float rate = 0;
                if (OWInput.IsPressed(AscendKey, InputMode.Character))
                {
                    rate += Speed;
                }
                if (OWInput.IsPressed(DescendKey, InputMode.Character))
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
                    foreach (ScaleLevel level in Levels)
                    {
                        level.ApplyExponent(_exponent);
                    }
                }

                ExponentText.text = _exponent.ToString("0.0");
                int exp = Mathf.FloorToInt(_exponent);
                while (exp > -30 && !OrdersOfMagnitude.ContainsKey(exp))
                {
                    exp--;
                }
                if (OrdersOfMagnitude.TryGetValue(exp, out string unit))
                {
                    ScaleText.text = Mathf.Pow(10, _exponent - exp).ToString("0.0") + unit + "s";
                }
                else
                {
                    ScaleText.text = "0";
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
    }
}