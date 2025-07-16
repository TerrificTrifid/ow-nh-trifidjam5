using NewHorizons.Handlers;
using OWML.Common;
using System.Collections.Generic;
using UnityEngine;

namespace TrifidJam5
{
    public class ScaleExplorer : MonoBehaviour
    {
        public static ScaleExplorer Instance;

        public OWTriggerVolume Trigger;
        public Dictionary<int, ScaleLevel> Levels;
        public bool Activated = false;
        public int TargetLevel = 0;

        private float _currentLevel = 0;

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

            

            foreach (ScaleLevel level in gameObject.GetComponentsInChildren<ScaleLevel>(true))
            {
                if (!Levels.TryAdd(level.N, level)) TrifidJam5.Instance.ModHelper.Console.WriteLine(level.name + " didnt work", MessageType.Error);
            }
        }

        public void Update()
        {


            //Locator.GetPromptManager().AddScreenPrompt(_reelInPrompt, PromptPosition.UpperRight, true);
        }

        private void OnEntry(GameObject hitobj)
        {
            var body = hitobj.GetAttachedOWRigidbody();
            if (!body.CompareTag("Player")) return;


        }

        private void OnExit(GameObject hitobj)
        {
            var body = hitobj.GetAttachedOWRigidbody();
            if (!body.CompareTag("Player")) return;


        }
    }
}