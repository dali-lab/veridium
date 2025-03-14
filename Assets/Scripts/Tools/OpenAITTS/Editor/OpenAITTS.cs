using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using System.Text.RegularExpressions;
using UnityEngine.Networking;
using Codice.Utils;
using System;
using System.IO;

namespace Veridium.Tools
{
    public class OpenAITTS : EditorWindow
    {
        string text = "";
        string prefix = "in";
        string suffix = "_DE";
        string secretKey = "";

        [MenuItem("Tools/Text to Speech (OpenAI)")]
        public static void ShowExample()
        {
            OpenAITTS wnd = GetWindow<OpenAITTS>();
            wnd.titleContent = new GUIContent("Text to Speech (OpenAI)");
        }

        public void CreateGUI()
        {
            // Each editor window contains a root VisualElement object
            VisualElement root = rootVisualElement;


            // try to load secret key from file
            string secretKeyPath = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile) + Path.DirectorySeparatorChar + ".openaikey";
            if (File.Exists(secretKeyPath))
            {
                secretKey = File.ReadAllText(secretKeyPath).Trim();
                Debug.Log("Secret key found: " + secretKey);
            } else
            {
                Label label = new Label("Secret key not found. Please create a file at " + secretKeyPath + " with your OpenAI secret key.");
                label.style.whiteSpace = WhiteSpace.Normal;
                root.Add(label);

                return;
            }
            
            Label explanation = new Label("Enter the text you want to convert to speech. Use [CUT] to split the text into multiple clips. Things in <angle brackets> will be ignored. Output is stored in Assets/TTSOutput.");
            explanation.style.whiteSpace = WhiteSpace.Normal;
            root.Add(explanation);

            ScrollView scrollView = new ScrollView();
            root.Add(scrollView);

            TextField textField = new TextField();
            textField.value = text;
            textField.RegisterValueChangedCallback(evt => text = evt.newValue);
            textField.label = "";
            textField.multiline = true;
            textField.style.whiteSpace = WhiteSpace.Normal;
            scrollView.Add(textField);

            TextField prefixField = new TextField("Prefix");
            prefixField.value = prefix;
            prefixField.RegisterValueChangedCallback(evt => prefix = evt.newValue);
            root.Add(prefixField);

            TextField suffixField = new TextField("Suffix");
            suffixField.value = suffix;
            suffixField.RegisterValueChangedCallback(evt => suffix = evt.newValue);
            root.Add(suffixField);
            
            Button button = new Button();
            button.name = "generate";
            button.text = "Generate speech";
            button.clickable.clicked += GenerateSpeech;
            root.Add(button);
        }

        public void GenerateSpeech()
        {
            string cleanText = Regex.Replace(text, @"<[^>]*>", "").Replace("\n", " ");

            // split text by [CUT]
            string[] splitText = cleanText.Replace("[CUT]", "\n").Split('\n');

            for (int i = 0; i < splitText.Length; i++)
            {
                string s = splitText[i];

                GenerateClip(s, Application.dataPath + "/TTSOutput/" + prefix + (i+1) + suffix + ".mp3");
            }
        }

        public void GenerateClip(string text, string outputFilePath) {
            text = HttpUtility.JavaScriptStringEncode(text);
            string requestBody = "{\n    \"model\": \"tts-1\",\n    \"input\": \"" + text + "\",\n    \"voice\": \"alloy\"\n}";

            // send request to OpenAI
            var request = UnityWebRequest.Put("https://api.openai.com/v1/audio/speech", requestBody);

            request.SetRequestHeader("Authorization", "Bearer "+ secretKey);
            request.SetRequestHeader("Content-Type", "application/json");

            // create via PUT and change to POST later because UnityWebRequest.Post is broken
            request.method = "POST";

            var res = request.SendWebRequest();

            res.completed += (op) =>
            {
                if (request.result == UnityWebRequest.Result.Success)
                {
                    byte[] audio = request.downloadHandler.data;
                    File.WriteAllBytes(outputFilePath, audio);
                    Debug.Log("Audio saved to " + outputFilePath);

                    AssetDatabase.Refresh();
                }
                else
                {
                    Debug.Log(request.error);
                }
            };
        }
    }
}