using System;
using UnityEngine;
using System.Collections.Generic;

namespace sib
{
    class Program
    {
        static void Main(string[] args)
        {
            UnitCell6 cell = new UnitCell6(CellType.CUBIC, CellVariation.SIMPLE, 
                new Vector3(0, 0, 0), 10, 10, 10, 90, 90, 90);
            cell.AddVertices(new Dictionary<Vector3, Atom>(), 0, null);

            string cellDebug = cell.Debug();
            Console.WriteLine(cellDebug);

            Console.WriteLine("Hello World!");
        }
    }
}