using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace Veridium.Modules.AminoAcids
{
    public class AminoAcidBuildButton : MonoBehaviour
    {
        public bool isBuilding = false;
        private Image image;
        private AminoAcidSheet sheet;
        private TMP_Text text;
        [SerializeField]
        private GameObject lockAminoAcidsPlane;

        void Start()
        {
            image = GetComponent<Image>();
            sheet = GetComponentInParent<AminoAcidSheet>();
            text = GetComponentInChildren<TMP_Text>();
            GetComponentInParent<Canvas>().gameObject.SetActive(false);
        }

        public void Toggle()
        {
            isBuilding = !isBuilding;
            text.text = isBuilding ? "Cancel" : "Build";
            image.color = isBuilding ? Color.red : Color.green;
            lockAminoAcidsPlane.SetActive(isBuilding);
        }
    }
}