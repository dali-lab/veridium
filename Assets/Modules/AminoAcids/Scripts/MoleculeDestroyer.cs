using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.Events;

namespace Veridium.Modules.AminoAcids
{
    public class MoleculeDestroyer : MonoBehaviour
    {
        public Material neutralMat;
        public Material destroyMat;
        private new Renderer renderer;
        private int collidingInteractors = 0;
        public UnityEvent<Molecule> OnDestroyMolecule;

        void Start()
        {
            renderer = GetComponent<Renderer>();
            OnDestroyMolecule = new UnityEvent<Molecule>();
        }

        public void OnTriggerEnter(Collider other)
        {
            if (!other.TryGetComponent(out IXRSelectInteractor interactor)) return;

            interactor.selectExited.AddListener(OnSelectExited);
            renderer.material = destroyMat;
            collidingInteractors++;
        }

        public void OnTriggerExit(Collider other)
        {
            if (!other.TryGetComponent(out IXRSelectInteractor interactor)) return;

            interactor.selectExited.RemoveListener(OnSelectExited);
            collidingInteractors--;
            if (collidingInteractors > 0) return;
            renderer.material = neutralMat;
        }

        private void OnSelectExited(SelectExitEventArgs args)
        {
            if (!args.interactableObject.transform.TryGetComponent(out Molecule molecule)) return;
            if (args.interactableObject.interactorsSelecting.Count > 0) return;

            Destroy(molecule.gameObject);
            OnDestroyMolecule.Invoke(molecule);
        }
    }
}