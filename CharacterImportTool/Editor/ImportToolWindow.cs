using UnityEngine;
using UnityEditor;
using ReadyPlayerMe;

public class ImportToolWindow : EditorWindow
{
    [MenuItem("XpertVR/Character Import Tool")]
    public static void ShowWindow()
    {
        EditorWindow window = GetWindow<ImportToolWindow>("Character Import Tool");
        window.maxSize = new Vector2(500f, 500f);
        window.minSize = window.maxSize;
    }

    public CharData[] characters;
    public WindowData windowData;
    private string dataPath = "Assets/ConversationMatrixTool/CharacterImportTool/Data.asset";
    private bool running;
    private string currentAvatarName;
    AvatarLoader avatarLoader;
    private float progress;
    private int index;

    private void Awake()
    {
        //Create the scriptable object if it does not exist
        if (!EditorGUIUtility.Load(dataPath))
            AssetDatabase.CreateAsset(CreateInstance<WindowData>(), dataPath);

        //Load the scriptable object and apply its data to the window
        windowData = (WindowData)EditorGUIUtility.Load(dataPath) as WindowData;
        //instantiate array
        characters = windowData.characterCodes;
    }

    private void AvatarLoaderOnOnProgressChanged(object sender, ProgressChangeEventArgs e)
    {
        progress = e.Progress;
    }

    private void OnGUI()
    {
        if (avatarLoader == null)
        {
            avatarLoader = new AvatarLoader();
            avatarLoader.OnProgressChanged += AvatarLoaderOnOnProgressChanged;
            avatarLoader.OnFailed += AvatarLoaderOnOnFailed;
            avatarLoader.OnCompleted += AvatarLoaderOnOnCompleted;
        }

        //Create new GUIstyle that will wrap text correctly
        GUIStyle wrappedText = new GUIStyle(GUI.skin.GetStyle("label"))
        {
            wordWrap = true
        };
        GUILayout.Space(10);

        //Warning Message
        GUILayout.Label(
            "This tool requires that the Conversation Matrix Tool and the Ready Player Me SDK are installed in the project. Please ensure that they are installed before using this!",
            wrappedText);
        GUILayout.Space(10);

        //Character Codes Array
        //ScriptableObject target = this;
        SerializedObject so = new SerializedObject(this);
        SerializedProperty stringsProperty = so.FindProperty("characters");
        EditorGUILayout.PropertyField(stringsProperty, true); // True means show children
        so.ApplyModifiedProperties(); // Remember to apply modified properties

        
        //Import Characters Button if there is something in the array
        if (GUILayout.Button("Import Characters"))
        {
            var len = characters.Length;
            //Check if character code array is null
            if (len > 0 && running == false)
            {
                //Once started, don't allow the user to click the button again until it is finished
                running = true;
                index = 0;
                //Import the characters here
                Debug.Log("Importing " + len + " characters");

                //Begin loading the avatars
                LoadAvatars();
            }
        }

        //Display progress bar if there are characters importing
        if (running)
        {
            GUI.enabled = true;
            EditorGUI.ProgressBar(
                new Rect(20, 150 + 200 * (characters.Length), position.width - 40, 20),
                progress,
                "Characters Loaded (" + index + "/" + characters.Length + ")");
        }
    }

    private void AvatarLoaderOnOnCompleted(object sender, CompletionEventArgs e)
    {
        Debug.Log("Avatar " + index + " has been imported!");

        var avatar = e.Avatar;

        if (avatar == null) return;

        var charHook = avatar.AddComponent<ConversationMatrixTool.CharacterHook>();

        avatar.transform.name = currentAvatarName + "_Avatar";

        charHook.characterName = currentAvatarName;

        charHook.GenerateCharacterScriptableObject();

        index++;
        LoadAvatars();
    }

    private void AvatarLoaderOnOnFailed(object sender, FailureEventArgs e)
    {
        Debug.LogError(e.Message);
    }

    void OnDestroy()
    {
        //Save the array data when the window is closed
        windowData.characterCodes = characters;
    }

    private void LoadAvatars()
    {
        //If not all the characters are loaded, load the next character. Otherwise reset the window.
        if (index < characters.Length)
        {
            LoadAvatar(index);
        }
        else
        {
            Debug.Log("Characters are loaded!");
            running = false;
        }
    }

    private void LoadAvatar(int index)
    {
        avatarLoader.LoadAvatar(characters[index].URL);
        currentAvatarName = characters[index].charName;
    }

    //This will return the path of the new GLB file that was created if given the original url 
    private string GetGLBPath(string url)
    {
        return "Assets/Resources/Avatars/" + url.Substring(38);
    }
}