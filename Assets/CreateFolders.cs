using UnityEditor;
using UnityEngine;
using System.Collections.Generic;
using System.IO;
using UnityEngine.WSA;

public class CreateFolders : EditorWindow
{
    private static string _projectName = "PROJECT_NAME";

    [MenuItem("Assets / Create Default Folders")]
    private static void SetUpFolders()
    {
        CreateFolders window = ScriptableObject.CreateInstance<CreateFolders>();
        window.position = new Rect(Screen.width / 2, Screen.height / 2, 400, 150);
        window.ShowPopup();
    }

    private static void CreateAllFolders()
    {
        //Main Folders
        List<string> mainFolders = new()
        {
            "Art",
            "Audio",
            "Coding",
            "Docs",
            "Levels",
            "Settings",
            "ThirdParty"
        };

        //Art Subfolders
        List<string> artFolders = new()
        {
            "Animations",
            "Assets",
            "Fonts",
            "Icons",
            "Materials",
            "Meshes"
        };

        //Audio Subfolders
        List<string> audioFolders = new()
        {
            "BGM",
            "Music",
            "SFX"
        };

        //Programming Subfolders
        List<string> codingFolders = new()
        {
            "Editor",
            "Scripts",
            "Shaders",
            "VFX",
        };

        //Levels Subfolders
        List<string> levelsFolders = new()
        {
            "Prefabs",
            "Scenes",
            "UI"
        };

        foreach (string folder in mainFolders)
        {
            if (!Directory.Exists("Assets/" + folder))
                Directory.CreateDirectory("Assets/" + _projectName + "/" + folder);
        }

        CreateSubFolder("Art", artFolders);
        CreateSubFolder("Audio", audioFolders);
        CreateSubFolder("Coding", codingFolders);
        CreateSubFolder("Levels", levelsFolders);

        AssetDatabase.Refresh();
    }

    private static void CreateSubFolder(string mainFolder, List<string> subFolders)
    {
        if (!Directory.Exists("Assets/" + mainFolder))
        {
            foreach (string subfolder in subFolders)
            {
                if (!Directory.Exists("Assets/" + _projectName + "/" + mainFolder + "/" + subfolder))
                    Directory.CreateDirectory("Assets/" + _projectName + "/" + mainFolder + "/" + subfolder);
            }
        }
    }

    private void OnGUI()
    {
        EditorGUILayout.LabelField("Insert the Project name used as the root folder");
        _projectName = EditorGUILayout.TextField("Project Name: ", _projectName);
        this.Repaint();
        GUILayout.Space(70);
        if (GUILayout.Button("Generate!"))
        {
            CreateAllFolders();
            this.Close();
        }
    }
}