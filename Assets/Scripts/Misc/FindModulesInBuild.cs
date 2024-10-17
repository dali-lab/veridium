using System;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using Veridium_Interaction;

namespace Veridium.Modules
{
    public class FindModulesInBuild : MonoBehaviour
    {
        public string moduleFolder = "Assets/Modules";

        public List<VeridiumModule> modules = new List<VeridiumModule>();

        public GameObject moduleSocket;

        // Start is called before the first frame update
        void Start()
        {
            int sceneCount = UnityEngine.SceneManagement.SceneManager.sceneCountInBuildSettings;     
            string[] scenes = new string[sceneCount];
            for(int i = 0; i < sceneCount; i++)
            {
                scenes[i] = UnityEngine.SceneManagement.SceneUtility.GetScenePathByBuildIndex( i );
            }

            int j = 0;
            foreach( string scene in scenes )
            {
                string modulePrefabPath = Path.GetFileName(scene).Replace(".unity", "");
                GameObject modulePrefab = Resources.Load<GameObject>(modulePrefabPath);

                if(modulePrefab != null)
                {
                    VeridiumModule module = modulePrefab.GetComponent<VeridiumModule>();
                    if( module != null )
                    {
                        Debug.Log("Found module: " + module.displayName);
                        Debug.Log("Description: " + module.description);
                        
                        // instantiate the module
                        GameObject moduleInstance = Instantiate(modulePrefab);

                        GameObject socket = Instantiate(moduleSocket);
                        socket.transform.position = transform.TransformPoint(Vector3.right * 0.3f * j);
                        socket.transform.rotation = transform.rotation;

                        moduleInstance.transform.parent = socket.transform;
                        moduleInstance.transform.localPosition = Vector3.zero;
                        moduleInstance.transform.localRotation = Quaternion.identity;
                        moduleInstance.GetComponent<VeridiumModule>().SetScenePath(scene);

                        modules.Add(moduleInstance.GetComponent<VeridiumModule>());
                        j++;
                    }
                }
            }
        }

        // Update is called once per frame
        void Update()
        {
            
        }

        public static string NormalizePath(string path)
        {
            return Path.GetFullPath(new Uri(path).LocalPath)
                    .TrimEnd(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar)
                    .ToUpperInvariant();
        }

        public void HideAllButOne(string visibleModule)
        {
            foreach(VeridiumModule module in modules)
            {
                if(module.displayName != visibleModule)
                {
                    module.gameObject.SetActive(false);
                    module.GetComponent<ReturnHomeAfter>().Interacted();
                }
            }
        }

        public void LockModule(string lockedModule)
        {
            foreach(VeridiumModule module in modules)
            {
                if(module.displayName == lockedModule)
                {
                    module.GetComponent<HandDistanceGrabbable>().Lock();
                }
            }
        }

        public void UnlockModule(string unlockedModule)
        {
            foreach(VeridiumModule module in modules)
            {
                if(module.displayName == unlockedModule)
                {
                    module.GetComponent<HandDistanceGrabbable>().Unlock();
                }
            }
        }
    }
}