using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;
using System.Linq;

namespace Veridium.Modules.AminoAcids
{
    public class HelpVolume : MonoBehaviour
    {
        [SerializeField] TMP_Text explanationField;
        private Dictionary<Collider, Component> explainableObjects;
        [SerializeField] List<LanguageText> EmptyExplanation;
        [SerializeField] List<LanguageText> TooManyObjectsExplanation;
        [SerializeField] List<LanguageText> MoleculeExplanation;
        [SerializeField] List<LanguageText> LaserPointerExplanation;
        [SerializeField] List<LanguageText> BondManipulatorExplanation;
        [SerializeField] List<LanguageText> MirrorExplanation;
        private Dictionary<Type, List<LanguageText>> types;

        void Start()
        {
            explainableObjects = new Dictionary<Collider, Component>();

            types = new Dictionary<Type, List<LanguageText>>()
            {
                { typeof(Molecule), MoleculeExplanation },
                { typeof(LaserPointer), LaserPointerExplanation },
                { typeof(BondManipulator), BondManipulatorExplanation },
                { typeof(MoleculeMirror), MirrorExplanation }
            };
        }

        void Update()
        {
            transform.LookAt(Camera.main.transform, Vector3.up);
        }

        void OnTriggerEnter(Collider other)
        {
            foreach (KeyValuePair<Type, List<LanguageText>> kvp in types)
            {
                if (!other.TryGetComponentInParent(kvp.Key, out Component component)) continue;

                explainableObjects.Add(other, component);
                TryUpdateText(kvp.Value);
                return;
            }
        }

        void OnTriggerExit(Collider other)
        {
            explainableObjects.Remove(other);
            TryUpdateText(null);
        }

        private void TryUpdateText(List<LanguageText> localizedExplanation)
        {
            if (explainableObjects.Count == 0)
            {
                SetText(EmptyExplanation);
                return;
            }

            if (explainableObjects.Any(kvp => kvp.Value.GetType() != explainableObjects.First().Value.GetType()))
            {
                SetText(TooManyObjectsExplanation);
                return;
            }

            SetText(localizedExplanation ?? GetExplanationForAllObjects());
        }

        private void SetText(List<LanguageText> localizedExplanation)
        {
            explanationField.text = localizedExplanation.GetTextForCurrentLanguage();
        }
        
        private List<LanguageText> GetExplanationForAllObjects()
        {
            return types.TryGetValue(explainableObjects.First().Value.GetType(), out var localizedExplanation) ? localizedExplanation : EmptyExplanation;
        }
    }
}
