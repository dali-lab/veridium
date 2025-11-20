using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Veridium.Modules.AminoAcids;
using UnityEngine.UI;
using UnityEngine.Events;
using TMPro;

public class AminoAcidSheet : MonoBehaviour
{
    public PeekableStructuralFormula selectedAminoAcid;
    public Image aminoAcidImage;
    public TextMeshProUGUI aminoAcidName;
    public GameObject peekCanvas;
    public UnityEvent<PeekableStructuralFormula> aminoAcidSelected;
    private AminoAcidBuildButton buildButton;

    void Start()
    {
        buildButton = GetComponentInChildren<AminoAcidBuildButton>();
    }

    public void ToggleAminoAcid(PeekableStructuralFormula aminoAcid)
    {
        if (buildButton.isBuilding) return;

        if (selectedAminoAcid == aminoAcid)
        {
            selectedAminoAcid = null;
            peekCanvas.SetActive(false);
            aminoAcidSelected.Invoke(null);
            return;
        }

        if (!selectedAminoAcid) peekCanvas.SetActive(true);

        selectedAminoAcid = aminoAcid;
        aminoAcidImage.sprite = selectedAminoAcid.peekMaterial;
        aminoAcidName.text = selectedAminoAcid.peekMaterial.name;

        aminoAcidSelected.Invoke(selectedAminoAcid);
    }
}
