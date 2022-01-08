/**
 * @author Siddharth Hathi
 * @title UnitCell
 */

using System;
using System.Collections.Generic;
using UnityEngine;

namespace sib
{
    /**
     * @class UnitCell
     * Defines the UnitCell abstract class interface structure. The abstract
     * class defines a set of functions shared by all unit cell class 
     * implementations
     */
    public abstract class UnitCell
    {
        // Retreive a list of atoms falling on a miller plane for the cell
        public abstract List<Atom> GetMillerAtoms(int h, int k, int l);

        // Creates the cell's vertices and adds them to the cell
        public abstract void AddVertices(Dictionary<Vector3, Atom> crystalAtoms);

        // Adds bonds to the unit cell
        public abstract void AddBonds(Dictionary<Vector3, Bond> crystalBonds);

        // Gets an array of the cells vertex atoms
        public abstract Atom[] GetVertices();

        // Gets a list of the cells bonds
        public abstract List<Bond> GetBonds();

        // Draws the cell
        public abstract void Draw(GameObject atomPrefab, GameObject linePrefab, GameObject builder);

        // Generates the cells neighbors
        public abstract void GenerateNeighbors(Dictionary<Vector3, Atom> crystalAtoms, Dictionary<Vector3, Bond> crystalBonds, Dictionary<Vector3, UnitCell> crystalCells);

        // Returns a debugging string
        public abstract string Debug();
    }
}