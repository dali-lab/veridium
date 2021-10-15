using UnityEngine;
using System;
using System.Collections.Generic;

namespace sib
{
    class Crystal {

        Dictionary<Vector3, Atom> atoms;
        Dictionary<Vector3, UnitCell6> unitCells;

        public Crystal() {
            atoms = new Dictionary<Vector3, Atom>();
            unitCells = new Dictionary<Vector3, UnitCell6>();
        }

    }
}