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

        foreach( string scene in scenes )
        {
            string modulePrefabPath = scene.Replace(".unity", ".prefab");

            if(File.Exists(modulePrefabPath) )
            {
                GameObject modulePrefab = AssetDatabase.LoadAssetAtPath<GameObject>(modulePrefabPath);
                VeridiumModule module = modulePrefab.GetComponent<VeridiumModule>();
                if( module != null )
                {
                    Debug.Log("Found module: " + module.displayName);
                    Debug.Log("Description: " + module.description);
                    
                    // instantiate the module
                    GameObject moduleInstance = Instantiate(modulePrefab);
                    moduleInstance.transform.position = transform.TransformPoint(Vector3.forward * 0.2f);
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
