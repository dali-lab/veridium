using System;
using System.IO;
using UnityEditor;
using UnityEngine;

public class FindModulesInBuild : MonoBehaviour
{
    public string moduleFolder = "Assets/Modules";
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
                    moduleInstance.transform.position = transform.TransformPoint(Vector3.forward * 0.3f * j);
                    moduleInstance.transform.rotation = transform.rotation;
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
}
