using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Coloration
{
    
    private static readonly Dictionary<string, string> Colors
        = new Dictionary<string, string>
        {
            {"Hydrogen", "ffffff"},
            {"Helium", "d9ffff"},
            {"Lithium", "cc80ff"},
            {"Beryllium", "c2ff00"},
            {"Boron", "ffb5b5"},
            {"Carbon", "909090"},
            {"Nitrogen", "3050f8"},
            {"Oxygen", "ff0d0d"},
            {"Fluorine", "90e050"},
            {"Neon", "b3e3f5"},
            {"Sodium", "ab5cf2"},
            {"Magnesium", "8aff00"},
            {"Aluminum", "bfa6a6"},
            {"Silicon", "f0c8a0"},
            {"Phosphorus", "ff8000"},
            {"Sulfur", "ffff30"},
            {"Chlorine", "1ff01f"},
            {"Argon", "80d1e3"},
            {"Potassium", "8f4004"},
            {"Calcium", "3d5500"},
            {"Scandium", "e6e6e6"},
            {"Titanium", "bfc2c7"},
            {"Vanadium", "a6a6ab"},
            {"Chromium", "8a99c7"},
            {"Manganese", "9c7ac7"},
            {"Iron", "e06633"},
            {"Cobalt", "f090a0"},
            {"Nickel", "50d050"},
            {"Copper", "c88033"},
            {"Zinc", "7d80b0"},
            {"Gallium", "c28f8f"},
            {"Germanium", "668f8f"},
            {"Arsenic", "bd80e3"},
            {"Selenium", "ffa100"},
            {"Bromine", "a62929"},
            {"Krypton", "5cb8d1"},
            {"Rubidium", "702eb0"},
            {"Strontium", "00ff00"},
            {"Yttrium", "94ffff"},
            {"Zirconium", "94e0e0"},
            {"Niobium", "73c2c9"},
            {"Molybdenum", "54b5b5"},
            {"Technetium", "3b9e9e"},
            {"Ruthenium", "248f8f"},
            {"Rhodium", "0a7d8c"},
            {"Palladium", "006985"},
            {"Silver", "c0c0c0"},
            {"Cadmium", "ffd98f"},
            {"Indium", "a67573"},
            {"Tin", "668080"},
            {"Antimony", "9e6385"},
            {"Tellurium", "d47a00"},
            {"Iodine", "940094"},
            {"Xenon", "429e80"},
            {"Caesium", "57178f"},
            {"Barium", "00c900"},
            {"Lanthanum", "70d4ff"},
            {"Cerium", "ffffc7"},
            {"Praseodymium", "d9ffc7"},
            {"Neodymium", "c7ffc7"},
            {"Promethium", "a3ffc7"},
            {"Samarium", "8fffc7"},
            {"Europium", "61ffc7"},
            {"Gadolinium", "45ffc7"},
            {"Terbium", "30ffc7"},
            {"Dysprosium", "1fffc7"},
            {"Holmium", "00ff9c"},
            {"Erbium", "00e675"},
            {"Thulium", "00d452"},
            {"Ytterbium", "00bf38"},
            {"Lutetium", "00ab24"},
            {"Hafnium", "4dc2ff"},
            {"Tantalum", "4da6ff"},
            {"Rhenium", "267dab"},
            {"Osmium", "266696"},
            {"Iridium", "175487"},
            {"Platinum", "d0d0e0"},
            {"Gold", "ffd123"},
            {"Mercury", "b8b8d0"},
            {"Thallium", "a6544d"},
            {"Lead", "575961"},
            {"Bismuth", "9e4fb5"},
            {"Polonium", "abfc00"},
            {"Astatine", "754f45"},
            {"Radon", "428296"},
            {"Francium", "420056"},
            {"Radium", "007d00"},
            {"Actinium", "70abfa"},
            {"Thorium", "00baff"},
            {"Protactinium", "00a1ff"},
            {"Uranium", "008fff"},
            {"Neptunium", "0080ff"},
            {"Plutonium", "006bff"},
            {"Americium", "545cf2"},
            {"Curium", "785ce3"},
            {"Berkelium", "8a4fe3"},
            {"Californium", "a136d4"},
            {"Einsteinium", "b31fd4"},
            {"Fermium", "b31fba"},
            {"Mendelevium", "b30da6"},
            {"Nobelium", "bd0d87"},
            {"Lawrencium", "c70066"},
            {"Rutherfordium", "cc0059"},
            {"Dubnium", "d1004f"},
            {"Seaborgium", "d90045"},
            {"Bohrium", "e00038"},
            {"Hassium", "e6002e"},
            {"Meitnerium", "eb0026"},
            {"Darmstadtium", "ffffff"},
            {"Roentgenium", "ffffff"},
            {"Copernicium", "ffffff"},
            {"Nihonium", "ffffff"},
            {"Flerovium", "ffffff"},
            {"Moscovium", "ffffff"},
            {"Livermorium", "ffffff"},
            {"Tennessine", "ffffff"},
            {"Oganesson", "ffffff"},
        };

    public static Color GetColor(string element){

        Color color = new Color();

        string hex = "#" + Colors[element];

        (GameObject.FindWithTag("DebugText").GetComponent<TMPro.TextMeshPro>()).text = hex;

        if (ColorUtility.TryParseHtmlString(hex, out color)){
            return color;
        } else {
            return new Color();
        }

    }

    public static Color GetColorByNumber(int atomicNumber){

        List<string> keyList = new List<string>(Colors.Keys);

        return (GetColor(keyList[atomicNumber-1]));

    }
}
