using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Veridium.Animation;

namespace Veridium.Modules.AminoAcids
{
    public class AnimSequenceController : MonoBehaviour
    {
        [SerializeField] private List<AnimSequence> sequences;
        private AnimSequence currentSequence;
        [SerializeField] private Transform buttonPodium;
        [SerializeField] private Vector3 podiumRetractedPosition;
        [SerializeField] private Vector3 podiumExtendedPosition;

        public void PlaySequence(int index)
        {
            if (index < 0 || index >= sequences.Count)
            {
                Debug.LogWarning("Animation index out of range: " + index);
                return;
            }

            if (currentSequence)
            {
                currentSequence.ResetSequence();
            }

            currentSequence = sequences[index];
            currentSequence.PlaySequence();
            RetractPodium();
        }

        public void AbortSequence()
        {
            if (currentSequence)
            {
                currentSequence.ResetSequence();
                currentSequence = null;
            }

            foreach (Molecule molecule in FindObjectsOfType<Molecule>())
            {
                if (molecule.GetComponentInParent<MoleculeSpawner>() != null) continue;
                Destroy(molecule);
            }

            ExtendPodium();
        }

        [ContextMenu("Reset Podium Position")]
        public void RetractPodium()
        {
            StartCoroutine(MovePodium(false));
        }

        [ContextMenu("Extend Podium Position")]
        public void ExtendPodium()
        {
            StartCoroutine(MovePodium(true));
        }

        private IEnumerator MovePodium(bool extend)
        {
            Vector3 startPos = buttonPodium.localPosition;
            Vector3 endPos = extend ? podiumExtendedPosition : podiumRetractedPosition;
            float duration = 0.5f;
            float elapsed = 0f;

            while (elapsed < duration)
            {
                elapsed += Time.deltaTime;
                float t = Mathf.Clamp01(elapsed / duration).EaseOut();
                buttonPodium.localPosition = Vector3.Lerp(startPos, endPos, t);
                yield return null;
            }

            buttonPodium.localPosition = endPos;
        }
    }
}
