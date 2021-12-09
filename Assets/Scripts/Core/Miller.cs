using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using UnityEngine;
using sib;

namespace sib
{
    public static class Miller
    {
        public static List<Vector3> GetMillerIndecesForCell(CellType type, CellVariation variation) {
            if (type == CellType.HEX) {
                return GetMillerIndicesForHexagonal();
            }
            List<Vector3> millerIndices = new List<Vector3>();
            switch (variation) {
                case CellVariation.SIMPLE:
                    for ( int h = 0; h <= 1; h ++ ) {
                        for ( int k = 0; k <= 1; k ++ ) {
                            for ( int l = 0; l <= 1; l ++ ) {
                                if (!(h == 0 && k == 0 && l == 0)) {
                                    Vector3 millerIndex = new Vector3(h, k, l);
                                    millerIndices.Add(millerIndex);
                                }
                            }
                        }
                    }
                    break;
                case CellVariation.BODY:
                    for ( int h = 0; h <= 1; h ++ ) {
                        for ( int k = 0; k <= 1; k ++ ) {
                            for ( int l = 0; l <= 1; l ++ ) {
                                if (!(h == 0 && k == 0 && l == 0)) {
                                    Vector3 millerIndex = new Vector3(h, k, l);
                                    millerIndices.Add(millerIndex);
                                }
                            }
                        }
                    }
                    millerIndices.Add(new Vector3(2, 0, 0));
                    millerIndices.Add(new Vector3(0, 2, 0));
                    millerIndices.Add(new Vector3(0, 0, 2));
                    break;
                default:
                    for ( int h = 0; h <= 2; h ++ ) {
                        for ( int k = 0; k <= 2; k ++ ) {
                            for ( int l = 0; l <= 2; l ++ ) {
                                if (!(h == 0 && k == 0 && l == 0) && !(h == 2 && k == 2 && l == 2) && !(h == 2 && k == 1 && l == 1)) {
                                    Vector3 millerIndex = new Vector3(h, k, l);
                                    millerIndices.Add(millerIndex);
                                }
                            }
                        }
                    }
                    break;
            }
            return millerIndices;
        }

        public static List<Vector3> GetMillerIndicesForHexagonal() {
            UnitCell8 hex = new UnitCell8(0, new Vector3(0, 0, 0), 0.2f, 0.4f, false);
            hex.AddVertices(new Dictionary<Vector3, Atom>());
            hex.AddBonds(new Dictionary<Vector3, Bond>());
            List<Vector3> hexagonalIndices = new List<Vector3>();
            for ( int h = -2; h <= 2; h ++ ) {
                for ( int k = -2; k <= 2; k ++ ) {
                    for ( int l = -2; l <= 2; l ++ ) {
                        if (!(h == 0 && k == 0 && l == 0)) {
                            if (hex.GetMillerAtoms(h, k, l).Count > 0) {
                                Vector3 millerIndex = new Vector3(h, k, l);
                                hexagonalIndices.Add(millerIndex);
                            }
                        }
                    }
                }
            }
            return hexagonalIndices;
        }

        public static bool PointInMillerPlane(Vector3 point, int h, int k, int l, Vector3 origin, Vector3 a1, Vector3 a2, Vector3 a3, float planarSeparation) {

            Vector3 b1, b2, b3;
            b1 = a1/((float)h);
            b2 = a2/((float)k);
            b3 = a3/((float)l);

            List<Vector3> parallelVectors = new List<Vector3>();
            List<Vector3> pointsOnPlane = new List<Vector3>();

            if (h == 0) {
                parallelVectors.Add(a1);
            } else {
                pointsOnPlane.Add(origin + b1);
            }

            if (k == 0) {
                parallelVectors.Add(a2);
            } else {
                pointsOnPlane.Add(origin + b2);
            }

            if (l == 0) {
                parallelVectors.Add(a3);
            } else {
                pointsOnPlane.Add(origin + b3);
            }

            if (pointsOnPlane.Count == 0) {
                return false;
            } else {
                while (pointsOnPlane.Count < 3) {
                    if (parallelVectors.Count < 1) {
                        return false;
                    }
                    pointsOnPlane.Add(pointsOnPlane[0] + parallelVectors[0]);
                    parallelVectors.RemoveAt(0);
                }
                
                Vector3 startPoint, lhs, rhs;
                startPoint = pointsOnPlane[0];
                lhs = pointsOnPlane[1] - startPoint;
                rhs = pointsOnPlane[2] - startPoint;
                Vector3 normal = Vector3.Cross(lhs, rhs);

                Plane millerPlane = new Plane(normal, startPoint);
                if (millerPlane.GetDistanceToPoint(point) == 0) {
                    return true;
                }
            }
            return false;
        }
    }
}