using System;
using System.Collections;
using System.Collections.Generic;
using Oculus.Platform.Models;
using UnityEngine;

namespace Veridium.Modules
{
    public class VeridiumModule : MonoBehaviour
    {
        public string displayName;
        public string description;
        private string scenePath = "";

        // Start is called before the first frame update
        void Start()
        {
            
        }

        // Update is called once per frame
        void Update()
        {
            
        }

        public void SetScenePath(string path) {
            scenePath = path;
        }

        public string GetScenePath() {
            if (scenePath == "") throw new Exception("Uninitialized module");
            return scenePath;
        }
    }
}
