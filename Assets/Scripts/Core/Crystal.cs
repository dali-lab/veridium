using UnityEngine;
using System;
using System.Collections.Generic;

namespace sib
{
    class Crystal {

        Dictionary<Vector3, Atom> atoms;

        public Crystal() {
            atoms = new Dictionary<Vector3, Atom>();
        }

    }
}