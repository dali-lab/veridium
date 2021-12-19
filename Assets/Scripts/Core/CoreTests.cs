using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using sib;

namespace sib
{
    public static class Tests
    {
        public static void TestRhombohedral(GameObject atomPrefab, GameObject linePrefab, GameObject builder) {
            string debugString = "TestRhombo called \n";
            (GameObject.FindWithTag("DebugText").GetComponent<TMPro.TextMeshPro>()).text = debugString;

            UnitCell test = new UnitCell6(0, CellType.RHOMBO, CellVariation.SIMPLE, builder.transform.position, 0.3f, 0.3f, 0.3f, 60, 60, 60);

            debugString += "Unit Cell Initialized \n";
            (GameObject.FindWithTag("DebugText").GetComponent<TMPro.TextMeshPro>()).text = debugString;

            test.AddVertices(new Dictionary<Vector3, Atom>());

            debugString += "Vertices added \n";
            (GameObject.FindWithTag("DebugText").GetComponent<TMPro.TextMeshPro>()).text = debugString;

            test.AddBonds(new Dictionary<Vector3, Bond>());

            debugString += test.Debug();
            (GameObject.FindWithTag("DebugText").GetComponent<TMPro.TextMeshPro>()).text = debugString;

            test.Draw(atomPrefab, linePrefab, builder);
        }

        public static void TestMonoclinic(GameObject atomPrefab, GameObject linePrefab, GameObject builder) {
            UnitCell test = new UnitCell6(0, CellType.MONO, CellVariation.BASE, builder.transform.position, 0.3f, 0.3f, 0.3f, 90, 90, 120);
            test.AddVertices(new Dictionary<Vector3, Atom>());
            test.AddBonds(new Dictionary<Vector3, Bond>());
            test.Draw(atomPrefab, linePrefab, builder);
        }

        public static void TestTriclinic(GameObject atomPrefab, GameObject linePrefab, GameObject builder) {
            UnitCell test = new UnitCell6(0, CellType.TRI, CellVariation.SIMPLE, builder.transform.position, 0.5f, 0.3f, 0.3f, 110, 70, 120);
            test.AddVertices(new Dictionary<Vector3, Atom>());
            test.AddBonds(new Dictionary<Vector3, Bond>());
            test.Draw(atomPrefab, linePrefab, builder);
        }

        public static void TestHex(GameObject atomPrefab, GameObject linePrefab, GameObject builder) {
            (GameObject.FindWithTag("DebugText").GetComponent<TMPro.TextMeshPro>()).text = "Builing Hex"; 
            UnitCell test = new UnitCell8(0, builder.transform.position, 0.2f, 0.2f, true);
            (GameObject.FindWithTag("DebugText").GetComponent<TMPro.TextMeshPro>()).text = "Hex Initialized"; 
            test.AddVertices(new Dictionary<Vector3, Atom>());
            (GameObject.FindWithTag("DebugText").GetComponent<TMPro.TextMeshPro>()).text = "Vertices Added"; 
            test.AddBonds(new Dictionary<Vector3, Bond>());
            (GameObject.FindWithTag("DebugText").GetComponent<TMPro.TextMeshPro>()).text = "Bonds Added"; 
            test.Draw(atomPrefab, linePrefab, builder);
            (GameObject.FindWithTag("DebugText").GetComponent<TMPro.TextMeshPro>()).text = "Hex drawn"; 
        }

        public static void TestOrthoCrystal(GameObject atomPrefab, GameObject linePrefab, GameObject builder) {
            Crystal test = new Crystal(builder.transform.position);
            test.SetState(CrystalState.INFINITE);
            test.Construct(CellType.ORTHO, CellVariation.BODY, 0.3f, 0.4f, 0.5f, 90, 90, 90, 0, 2);
            test.Draw(atomPrefab, linePrefab, builder);
        }

        public static void TestTetraCrystal(GameObject atomPrefab, GameObject linePrefab, GameObject builder) {
            Crystal test = new Crystal(builder.transform.position);
            test.SetState(CrystalState.INFINITE);
            test.Construct(CellType.TETRA, CellVariation.BODY, 0.3f, 0.3f, 0.6f, 90, 90, 90, 0, 2);
            test.Draw(atomPrefab, linePrefab, builder);
        }

        public static void TestRhomboCrystal(GameObject atomPrefab, GameObject linePrefab, GameObject builder) {
            Crystal test = new Crystal(builder.transform.position);
            test.SetState(CrystalState.INFINITE);
            test.Construct(CellType.RHOMBO, CellVariation.SIMPLE, 0.3f, 0.3f, 0.3f, 60, 60, 60, 0, 2);
            test.Draw(atomPrefab, linePrefab, builder);
        }

        public static void TestTriclinicCrystal(GameObject atomPrefab, GameObject linePrefab, GameObject builder) {
            Crystal test = new Crystal(builder.transform.position);
            test.SetState(CrystalState.INFINITE);
            test.Construct(CellType.TRI, CellVariation.SIMPLE, 0.3f, 0.4f, 0.5f, 100, 70, 120, 0, 2);
            test.Draw(atomPrefab, linePrefab, builder);
        }

        public static void TestMonoClinicCrystal(GameObject atomPrefab, GameObject linePrefab, GameObject builder) {
            Crystal test = new Crystal(builder.transform.position);
            test.SetState(CrystalState.INFINITE);
            test.Construct(CellType.MONO, CellVariation.BASE, 0.3f, 0.4f, 0.5f, 90, 90, 120, 0, 2);
            test.Draw(atomPrefab, linePrefab, builder);
        }

        public static void TestHexCrystal(GameObject atomPrefab, GameObject linePrefab, GameObject builder) {
            Crystal test = new Crystal(builder.transform.position);
            test.SetState(CrystalState.INFINITE);
            test.Construct(CellType.HEX, CellVariation.SIMPLE, 0.2f, 0.4f, 0, 0, 0, 0, 0, 1);
            test.Draw(atomPrefab, linePrefab, builder);
        }

        public static void TestUnit6Millers(GameObject atomPrefab, GameObject linePrefab, GameObject builder) {
            string debugString = "";
            UnitCell6 test = new UnitCell6(5, CellType.CUBIC, CellVariation.BODY, builder.transform.position, 0.3f, 0.3f, 0.3f, 0, 0, 0);
            test.AddVertices(new Dictionary<Vector3, Atom>());
            test.AddBonds(new Dictionary<Vector3, Bond>());
            test.Draw(atomPrefab, linePrefab, builder);
            List<Atom> millerAtoms = test.GetMillerAtoms(2, 0, 0);
            if (millerAtoms.Count > 0) {
                debugString += "GetMillerAtoms returned atoms" + "\n";
                (GameObject.FindWithTag("DebugText").GetComponent<TMPro.TextMeshPro>()).text = debugString; 
            } else {
                debugString += "GetMillerAtoms didn't crash but didn't return atoms" + "\n"; 
                (GameObject.FindWithTag("DebugText").GetComponent<TMPro.TextMeshPro>()).text = debugString; 
            }
            foreach (Atom atom in millerAtoms) {
                debugString += atom.Debug();
            }
            (GameObject.FindWithTag("DebugText").GetComponent<TMPro.TextMeshPro>()).text = debugString; 
        }

        public static void TestUnit8Millers(GameObject atomPrefab, GameObject linePrefab, GameObject builder) {
            string debugString = "";
            UnitCell8 test = new UnitCell8(0, builder.transform.position, 0.2f, 0.4f, false);
            test.AddVertices(new Dictionary<Vector3, Atom>());
            test.AddBonds(new Dictionary<Vector3, Bond>());
            test.Draw(atomPrefab, linePrefab, builder);
            List<Atom> millerAtoms = test.GetMillerAtoms(-1, 0, 0);
            if (millerAtoms.Count > 0) {
                debugString += "GetMillerAtoms returned atoms" + "\n";
                (GameObject.FindWithTag("DebugText").GetComponent<TMPro.TextMeshPro>()).text = debugString; 
            } else {
                debugString += "GetMillerAtoms didn't crash but didn't return atoms" + "\n"; 
                (GameObject.FindWithTag("DebugText").GetComponent<TMPro.TextMeshPro>()).text = debugString; 
            }
            foreach (Atom atom in millerAtoms) {
                debugString += atom.Debug();
            }
            (GameObject.FindWithTag("DebugText").GetComponent<TMPro.TextMeshPro>()).text = debugString; 
        }

        public static void TestMillerCrystal(GameObject atomPrefab, GameObject linePrefab, GameObject builder) {
            string debugString = "";
            Crystal test = new Crystal(builder.transform.position);
            test.SetState(CrystalState.INFINITE);
            test.Construct(CellType.HEX, CellVariation.SIMPLE, 0.2f, 0.4f, 0, 0, 0, 0, 0, 1);
            test.Draw(atomPrefab, linePrefab, builder);
            HashSet<Atom> crystalMillerAtoms = test.GetMillerAtoms(-1, 0, 0);
            if (crystalMillerAtoms.Count > 0) {
                debugString += "GetMillerAtoms returned atoms" + "\n";
                (GameObject.FindWithTag("DebugText").GetComponent<TMPro.TextMeshPro>()).text = debugString; 
            } else {
                debugString += "GetMillerAtoms didn't crash but didn't return atoms" + "\n"; 
                (GameObject.FindWithTag("DebugText").GetComponent<TMPro.TextMeshPro>()).text = debugString; 
            }
            foreach (Atom atom in crystalMillerAtoms) {
                debugString += atom.Debug();
            }
            (GameObject.FindWithTag("DebugText").GetComponent<TMPro.TextMeshPro>()).text = debugString; 
        }

        public static void TestMillerLists(GameObject atomPrefab, GameObject linePrefab, GameObject builder) {
            UnitCell8 testHex = new UnitCell8(0, builder.transform.position, 0.2f, 0.4f, false);
            UnitCell6 testSimple = new UnitCell6(0, CellType.CUBIC, CellVariation.SIMPLE, builder.transform.position, 0.2f, 0.2f, 0.2f, 90, 90, 90);
            UnitCell6 testBody = new UnitCell6(0, CellType.CUBIC, CellVariation.BODY, builder.transform.position, 0.2f, 0.2f, 0.2f, 90, 90, 90);
            UnitCell6 testFace = new UnitCell6(0, CellType.CUBIC, CellVariation.FACE, builder.transform.position, 0.2f, 0.2f, 0.2f, 90, 90, 90);

            testHex.AddVertices(new Dictionary<Vector3, Atom>());
            testSimple.AddVertices(new Dictionary<Vector3, Atom>());
            testBody.AddVertices(new Dictionary<Vector3, Atom>());
            testFace.AddVertices(new Dictionary<Vector3, Atom>());
            
            testHex.AddBonds(new Dictionary<Vector3, Bond>());
            testSimple.AddBonds(new Dictionary<Vector3, Bond>());
            testBody.AddBonds(new Dictionary<Vector3, Bond>());
            testFace.AddBonds(new Dictionary<Vector3, Bond>());

            List<Vector3> hexMillers = Miller.GetMillerIndecesForCell(CellType.HEX, CellVariation.SIMPLE);
            List<Vector3> simpleMillers = Miller.GetMillerIndecesForCell(CellType.CUBIC, CellVariation.SIMPLE);
            List<Vector3> bodyMillers = Miller.GetMillerIndecesForCell(CellType.CUBIC, CellVariation.BODY);
            List<Vector3> faceMillers = Miller.GetMillerIndecesForCell(CellType.CUBIC, CellVariation.FACE);

            int testsFailed = 0;
            string debugString = "";

            debugString += "Hexagonals contains " + hexMillers.Count.ToString() + "vectors";
            foreach (Vector3 millerVec in hexMillers) {
                int  h, k, l;
                h = (int)millerVec.x;
                k = (int)millerVec.y;
                l = (int)millerVec.z;

                List<Atom> millerAtoms = testHex.GetMillerAtoms(h, k, l);
                if (millerAtoms.Count == 0) {
                    testsFailed ++;
                    debugString += "Hex test failed for miller indices " + millerVec.ToString() + "\n";
                }
            }

            debugString += "Simple contains " + simpleMillers.Count.ToString() + "vectors";
            foreach (Vector3 millerVec in simpleMillers) {
                int  h, k, l;
                h = (int)millerVec.x;
                k = (int)millerVec.y;
                l = (int)millerVec.z;

                List<Atom> millerAtoms = testSimple.GetMillerAtoms(h, k, l);
                if (millerAtoms.Count == 0) {
                    testsFailed ++;
                    debugString += "Simple test failed for miller indices " + millerVec.ToString() + "\n";
                }
            }

            debugString += "Body contains " + bodyMillers.Count.ToString() + "vectors";
            foreach (Vector3 millerVec in bodyMillers) {
                int  h, k, l;
                h = (int)millerVec.x;
                k = (int)millerVec.y;
                l = (int)millerVec.z;

                List<Atom> millerAtoms = testBody.GetMillerAtoms(h, k, l);
                if (millerAtoms.Count == 0) {
                    testsFailed ++;
                    debugString += "Body test failed for miller indices " + millerVec.ToString() + "\n";
                }
            }

            debugString += "Face contains " + faceMillers.Count.ToString() + "vectors";
            foreach (Vector3 millerVec in faceMillers) {
                int  h, k, l;
                h = (int)millerVec.x;
                k = (int)millerVec.y;
                l = (int)millerVec.z;

                List<Atom> millerAtoms = testFace.GetMillerAtoms(h, k, l);
                if (millerAtoms.Count == 0) {
                    testsFailed ++;
                    debugString += "Face test failed for miller indices " + millerVec.ToString() + "\n";
                }
            }

            if (testsFailed == 0) {
                debugString += "All tests passed!";
            } else {
                debugString += testsFailed.ToString() + " tests failed\n";
            }
            (GameObject.FindWithTag("DebugText").GetComponent<TMPro.TextMeshPro>()).text = debugString; 
        }
    }
}