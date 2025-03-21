using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Veridium.Modules.AminoAcids {
    public class BondDestroyer : MonoBehaviour
    {
        void OnTriggerEnter(Collider other)
        {
            Bond bond = other.GetComponentInParent<Bond>();
            if (!bond) return;
            bond.Destroy();
        }
    }
}
