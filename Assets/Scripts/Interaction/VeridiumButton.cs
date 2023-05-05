using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class VeridiumButton : MonoBehaviour
{
    public static VeridiumButton Instance {get; private set; }

    public UnityEvent onInteracted;
    private bool pressable = false;
    private ButtonType bt = ButtonType.CONTINUE;
    public string buttonText = "Continue";
    public AudioClip buttonAudio;

    public enum ButtonType
    {
        CONTINUE,
        SUBMIT
    }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this);
        }
        else
        {
            Instance = this;
        }
    }

    public void Enable()
    {
        // Call glow effect
        pressable = true;
    }

    public void Disable()
    {
        // Stop glow effect
        pressable = false;
        onInteracted.RemoveAllListeners();
    }

    public void Pressed()
    {
        if (!pressable) return;
        // Debug.Log("Pressed!");
        onInteracted.Invoke();
        if (bt == ButtonType.CONTINUE) Disable();
    }

    // Switches the button between the different button types
    public void SwitchType(ButtonType newBT)
    {
        Debug.Log("Switched button type to: " + newBT.ToString());
        this.bt = newBT;
        buttonText = newBT.ToString();
    }


}
