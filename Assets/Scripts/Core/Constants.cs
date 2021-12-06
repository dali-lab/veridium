/**
 * @author      Siddharth Hathi
 * @title       Unit Cells
 */

using System;
using System.Collections.Generic;
using UnityEngine;

namespace sib
{
    // Static class the wraps global constants pertinent to the unit cell classes
    public static class Constants {
        // The maximum number of vertices in a 6-sided unit cell
        public static int cell6Vertices = 15;

        // Maximum number of bonds in a 6-sided unit cell
        public static int cell6Bonds = 32;

        // The maximum number of vertices in an 8-sided unit cell
        public static int cell8Vertices = 15;

        // A list of the 15 points in a 6-sided cell - the first 
        // 8 are found in all 6-sided unit cells, the next 8 are found
        // in face-centered cells, and the last one is a center point
        // for body-centered cells.
        /*************************************
        * UNIT CELL ARRAY POSITION-INDEX REF:
        * (Assumes 1x1x1 cubic centered at origin)
        * 0: (-1, -1, -1),
        * 1: (-1, -1, 1),
        * 2: (-1, 1, -1),
        * 3: (1, -1, -1),
        * 4: (1, 1, -1),
        * 5: (1, -1, 1),
        * 6: (-1, 1, 1),
        * 7: (1, 1, 1),
        * 8: (0, 0, -1),
        * 9: (0, -1, 0),
        * 10: (-1, 0, 0),
        * 11: (0, 0, 1),
        * 12: (0, 1, 0),
        * 13: (1, 0, 0),
        * 14: (0, 0, 0)
        **************************************/
        public static Vector3[] cell6BasicPositions = new Vector3[] {
            // Basic Vertices
            new Vector3(-1, -1, -1),    // 0
            new Vector3(-1, -1, 1),     // 1
            new Vector3(-1, 1, -1),     // 2
            new Vector3(1, -1, -1),     // 3
            new Vector3(1, 1, -1),      // 4
            new Vector3(1, -1, 1),      // 5
            new Vector3(-1, 1, 1),      // 6
            new Vector3(1, 1, 1),       // 7
            // Face Centered Vertices
            new Vector3(0, 0, -1),      // 8
            new Vector3(0, 0, 1),       // 9
            new Vector3(0, -1, 0),      // 10
            new Vector3(0, 1, 0),       // 11
            new Vector3(-1, 0, 0),      // 12
            new Vector3(1, 0, 0),       // 13
            // Body centered vertex
            new Vector3(0, 0, 0)        // 14
        };

        public static Vector3[] cell8BasicPositions = new Vector3[] {
            // top half
            new Vector3(0, 0, 0.5f),                               // 0
            new Vector3(0, 1, 0.5f),                               // 1
            new Vector3((float)(Math.Sqrt(3)/2), 0.5f, 0.5f),       // 2
            new Vector3(-(float)(Math.Sqrt(3)/2), 0.5f, 0.5f),      // 3
            new Vector3(0, -1, 0.5f),                               // 4
            new Vector3((float)(Math.Sqrt(3)/2), -0.5f, 0.5f),      // 5
            new Vector3(-(float)(Math.Sqrt(3)/2), -0.5f, 0.5f),     // 6
            // bottom half
            new Vector3(0, 0, -0.5f),                              // 7
            new Vector3(0, 1, -0.5f),                              // 8
            new Vector3((float)(Math.Sqrt(3)/2), 0.5f, -0.5f),      // 9
            new Vector3(-(float)(Math.Sqrt(3)/2), 0.5f, -0.5f),     // 10
            new Vector3(0, -1, -0.5f),                               // 11
            new Vector3((float)(Math.Sqrt(3)/2), -0.5f, -0.5f),     // 12
            new Vector3(-(float)(Math.Sqrt(3)/2), -0.5f, -0.5f)     // 13
        };

        public static Dictionary<CellVariation, int[]> cell6VariationMap = new Dictionary<CellVariation, int[]> {
            { CellVariation.SIMPLE, new int[] { 0, 1, 2, 3, 4, 5, 6, 7 } },
            { CellVariation.BASE, new int[] { 0, 1 , 2, 3, 4, 5, 6, 7, 8, 9 } },
            { CellVariation.FACE, new int[] { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13 } },
            { CellVariation.BODY, new int[] { 0, 1, 2, 3, 4, 5, 6, 7, 14 } }
        };

        // A static dictionary reference that maps each unit cell type to the valid variations associated with it.
        public static Dictionary<CellType, CellVariation[]> validCells = new Dictionary<CellType, CellVariation[]> {
            { CellType.CUBIC, new CellVariation[] { CellVariation.SIMPLE, CellVariation.FACE, CellVariation.BODY } },
            { CellType.ORTHO, new CellVariation[] { CellVariation.SIMPLE, CellVariation.FACE, CellVariation.BODY, CellVariation.BASE } },
            { CellType.TETRA, new CellVariation[] { CellVariation.SIMPLE, CellVariation.BODY } },
            { CellType.MONO, new CellVariation[] { CellVariation.SIMPLE, CellVariation.BASE } },
            { CellType.TRI, new CellVariation[] { CellVariation.SIMPLE } },
            { CellType.RHOMBO, new CellVariation[] { CellVariation.SIMPLE } },
            { CellType.HEX, new CellVariation[] { CellVariation.SIMPLE } }
        };

        public static int[][] cell6BondMap = new int[][] {
            new int[] { 1, 2, 3 },
            new int[] { 0, 5, 6 },
            new int[] { 0, 4, 6 },
            new int[] { 0, 4, 5 },
            new int[] { 2, 3, 7 },
            new int[] { 1, 3, 7 },
            new int[] { 1, 2, 7 },
            new int[] { 4, 5, 6},
            new int[] { 0, 2, 3, 4 },
            new int[] { 1, 5, 6, 7 },
            new int[] { 0, 1, 3, 5 },
            new int[] { 2, 4, 6, 7 },
            new int[] { 0, 1, 2, 6 },
            new int[] { 3, 4, 5, 7 },
            new int[] { 0, 1, 2, 3, 4, 5, 6, 7 }
        };

        public static int[][] cell8BondMap = new int[][] {
            new int[] { 1, 2, 3, 4, 5, 6 , 7 },     // 0
            new int[] { 0, 2, 3, 8 },              // 1
            new int[] { 0, 1, 5, 9 },              // 2
            new int[] { 0, 1, 6, 10 },              // 3
            new int[] { 0, 5, 6, 11 },              // 4
            new int[] { 0, 2, 4, 12 },              // 5
            new int[] { 0, 3, 4, 13 },              // 6
            new int[] { 0, 8, 9, 10, 11, 12, 13 }, // 7
            new int[] { 1, 7, 9, 10 },             // 8
            new int[] { 2, 7, 8, 12 },             // 9
            new int[] { 3, 7, 8, 13 },             // 10
            new int[] { 4, 7, 12, 13 },            // 11
            new int[] { 5, 7, 9, 11 },              // 12
            new int[] { 6, 7, 10, 11 }              // 13
        };
    }
}