using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using MyBox;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Playables;
using UnityEngine.UI;
using XNode;
using XNodeEditor;
using Debug = UnityEngine.Debug;
using Random = UnityEngine.Random;

namespace ConversationMatrixTool
{
    public class ConversationMatrixGraphPlayer : MonoBehaviour
    {
        #region VARIABLES

        private Conversation currentConversation;
        private ConversationMatrixGraph currentGraph;
        private BaseNode currentNode;
        private CharacterHook currentCharacter;
        private bool isReady;
        [Tooltip("A pool of conversations")] public List<Conversation> conversations;

        [Tooltip("A pool of characters in the scene")]
        public List<CharacterHook> characters;

        [Space] [Header("UI Parameters")] [Tooltip("Parent of statement text")]
        public GameObject statementPanel;

        [Tooltip("Parent of UI buttonsfor answers")]
        public GameObject answerPanel;

        [Tooltip("TextMeshPro component to show statement text")]
        public TextMeshProUGUI statement;

        [Tooltip("TextMeshPro array of answer buttons")]
        public TextMeshProUGUI[] answers;

        [Space]
        [Header("Debug Parameters")]
        [Tooltip("set this to true to run a conversation with the given name at start")]
        public bool debugMode = true;

        [Tooltip("Title of the conversation to be started in debug mode")]
        public string debugConversationTitle;

        [Space] [Header("Input Parameters")] [Tooltip("Setting this to false will hide statement text or vice versa")]
        public bool showSubtitles;

        [Tooltip("Shortcut to proceed to the next node at runtime")]
        public KeyCode skipKey;

        private Coroutine cr_WaitNode;
        private WaitForSeconds waitForSeconds;
        private bool isSkippableWaiting;
        private AudioSource currentlyPlaying;
        private Text continueButtonTitle;
        [HideInInspector] public int[] currentAnswerMatrix;
        private PlayableDirector sequenceDirector;

        #endregion

        #region UNITY_METHODS

        private void Awake()
        {
            ScanForCharactersInScene();
            if (statementPanel) statementPanel.SetActive(false);
            if (answerPanel) answerPanel.SetActive(false);
        }

        private void Start()
        {
            if (debugMode)
                StartConversation(debugConversationTitle);
        }

        private void Update()
        {
            if (!isReady) return;

            if (currentNode == null) return;
            //check if the skip key is pressed and skip to the next node unless current node is an unskippable wait node
            if (Input.GetKeyDown(skipKey))
                if (currentNode.type != NodeType.Wait)
                    NextNode();
                else if (isSkippableWaiting)
                    NextNode();
        }

        #endregion

        #region HELPER_METHODS

        //This method scans for all the characters in the scene and places them in a pool for runtime functionality
        public void ScanForCharactersInScene()
        {
            var temp = FindObjectsOfType<CharacterHook>();
            characters = new List<CharacterHook>();
            foreach (var c in temp)
            {
                characters.Add(c.ReturnCharacter());
                //adjust the audio source position and look at point for mouth of the character
                c.TryGetMouthPosition();
            }
        }

        //Starts a conversation if it is present in the conversation pool. Uses conversation title as the search criteria
        public void StartConversation(string conversationTitle)
        {
            //Grab conversation index via its name.
            int conversationIndex = CheckConversationIndex(conversationTitle);
            //if index is smaller than 0, it means no conversation with the given name was found in the conversation pool
            if (conversationIndex < 0)
            {
                Debug.LogError("Conversation does not exist in the pool! (Based on fullName search)");
                return;
            }

            //if index is equals to or higher than 0, start conversation via index
            StartConversation(conversationIndex);
        }

        //Starts a conversation if it is present in the conversation pool. Uses index to locate conversation
        public void StartConversation(int index)
        {
            //another set of checks, in case someone tries to run a conversation directly with an invalid index.
            if (index < 0)
            {
                Debug.LogError("Conversation outputIndex cannot be a negative integer!");
                return;
            }

            if (index > conversations.Count)
            {
                Debug.LogError("There aren't that many conversations in the pool!");
                return;
            }

            //set current conversation and graph for ease of access in the following sections of the code
            currentConversation = conversations[index];
            currentGraph = currentConversation.graph;
            //regular check
            if (currentGraph == null)
            {
                Debug.LogError("Conversation's graph is null!");
                return;
            }

            //set the player for the graph.
            currentGraph.player = this;
            //prime the graph to make it ready for showtime
            currentGraph.Initialize();
            //check if the initialization has been successful. the current node of the graph should not be null.
            //if null, probably missing a start node.
            if (currentGraph.currentNode == null)
            {
                Debug.LogWarning("Current node of graph for Conversation " + index +
                                 " is null! This is an error. Panic and run!");
                return;
            }

            //find start node
            currentNode = currentGraph.currentNode;
            //initialization should have set the start node as the current node but let us check 
            if (currentNode.type != NodeType.Start) //if the answer is no than try to find it
                for (int i = 0; i < currentGraph.nodes.Count; i++)
                    if (currentNode.type != NodeType.Start &&
                        (currentGraph.nodes[i] as BaseNode)?.type == NodeType.Start)
                        currentGraph.currentNode = currentGraph.nodes[i] as BaseNode;

            //final check. the current node of the graph should not be null.
            //if null, definitely missing a start node.
            if (currentGraph.currentNode == null || currentNode.type != NodeType.Start)
            {
                Debug.LogWarning("Current node of graph for Conversation " + index +
                                 " is null! This is an error. Possibility of a missing start node: 99%");
                return;
            }


            if (currentGraph.currentNode.type != NodeType.Start)
            {
                Debug.LogWarning(
                    "This graph does not contain a start node. This is unacceptable! Possibility of a missing start node: 100%");
                return;
            }

            Debug.Log("Graph of Conversation " + index + " has been loaded.");
            //let everyone know that the player is ready
            isReady = true;
            //process first node
            ProcessNode(
                currentGraph
                    .currentNode); //start node needs to run the assign method to go to the next function - start of a chain reaction
        }

        //This method return the index of a conversation in the pool, using its name as the search criteria.
        //Returns -1 if the conversation does not exist in the pool.
        private int CheckConversationIndex(string conversationTitle)
        {
            for (int i = 0; i < conversations.Count; i++)
            {
                if (conversations[i].title == conversationTitle)
                {
                    Debug.Log("Conversation '" + conversationTitle + "' was FOUND in the pool!");
                    return i;
                }
            }

            Debug.LogWarning("Conversation '" + conversationTitle + "' was NOT found in the pool!");
            return -1;
        }

        //a helper script to display and interact with pool questions
        public QuestionPoolDisplayer qPD;

        //following method finds the displayer and returns it. displayer's arrangement varies from scene to scene.
        private QuestionPoolDisplayer GetQPD()
        {
            if (qPD == null) qPD = GetComponent<QuestionPoolDisplayer>();
            if (qPD == null) qPD = GetComponentInChildren<QuestionPoolDisplayer>();
            if (qPD == null) qPD = FindObjectOfType<QuestionPoolDisplayer>();
            return qPD;
        }

        //this method checks if the character of the current node exists in the character pool.
        //if the character exists in the pool, it is assigned as the current character for ease of access.
        public void CharacterTest()
        {
            var index = -1;

            //in order to retrieve character in the node, we need to know the type of the node
            var type = currentNode.type;
            if (type == NodeType.Statement)
                index = CheckCharacter(((StatementNode)currentNode).character);
            else if (type == NodeType.Animation)
                index = CheckCharacter(((AnimationNodeBase)currentNode).character);

            //if index is equal to or greater than 0, it means the character has been found in the pool
            if (index >= 0)
                currentCharacter = ReturnCharacter(index);
            //else it means the character was not found in the pool
            else
                currentCharacter = null;
        }

        //this method returns a character's controller (character hook) from the pool, using the given index
        private CharacterHook ReturnCharacter(int index)
        {
            if (characters == null) return null;
            if (index < 0 || index > characters.Count) return null;
            return characters[index];
        }

        //this method returns the index of a character in the pool based on its scriptable object. returns -1 if character is not found.
        private int CheckCharacter(Character charSo)
        {
            var _temp = -1;
            if (characters == null) return _temp;
            if (characters.Count == 0) return _temp;
            if (charSo == null) return _temp;
            for (int i = 0; i < characters.Count; i++)
            {
                if (characters[i] == null) break;
                if (characters[i].guid == charSo.guid)
                    _temp = i;
            }

            return _temp;
        }

        //this method returns the runtime animator controller of a character based on its scriptable object.
        public RuntimeAnimatorController GetCharAnimator(Character charSo)
        {
            var index = CheckCharacter(charSo);
            if (index >= 0)
                return characters[index].animator.runtimeAnimatorController;
            return null;
        }

        //this method searches for an event in the pool with the matching key and invokes it if found
        public void InvokeEvent(string key)
        {
            foreach (var e in currentConversation.events)
                if (e.key == key)
                {
                    e._event.Invoke();
                    //Debug.Log("Event invoked: " + key);
                }
        }

        //this method checks if a condition is met and returns true or false accordingly
        public bool CheckCondition(string key)
        {
            foreach (var c in currentConversation.conditions)
                if (c.key == key)
                {
                    return c.condition.Invoke();
                }

            return false; //change this to desired default
        }

        #endregion

        #region NODE_PROCESSING_METHODS

        //this method directs operation to relevant methods, based on the type of the node being processed
        public void ProcessNode(BaseNode node)
        {
            //hide statement ui
            statementPanel.SetActive(false);

            //disable current node
            if (currentNode != null)
                currentNode.isCurrent = false;

            //update current node
            currentNode = node;

            //enable current node
            if (currentNode != null)
                currentNode.isCurrent = true;

            //this part only works in the editor and highlights current in the graph window
#if UNITY_EDITOR
            if (NodeEditorWindow.current)
            {
                NodeEditorWindow.current.SelectNode(currentNode, false);
                NodeEditorWindow.current.Repaint();
            }
#endif
            if (currentNode == null) return;

            //following method checks if the node's assigned character is in the character pool
            CharacterTest();

            //runs the relevant method, based on the type of the node being processed
            switch (currentNode.type)
            {
                case NodeType.Start:
                    currentNode.NextNode();
                    break;
                case NodeType.Statement:
                    ProcessStatementData();
                    break;
                case NodeType.Answer:
                    ProcessAnswerData();
                    break;
                case NodeType.End:
                    EndGraph();
                    break;
                case NodeType.Animation:
                    break;
                case NodeType.Wait:
                    ProcessWaitData();
                    break;
                case NodeType.LookAt:
                    ProcessLookAtData();
                    break;
                case NodeType.QuestionPool:
                    ProcessQuestionPoolData();
                    break;
                case NodeType.Sequence:
                    ProcessSequenceData();
                    break;
                default:
                    Debug.Log("Processing Default Node Action for " + currentNode.type);
                    break;
            }
        }

        private void ProcessSequenceData()
        {
            var node = currentNode as SequenceNode;
            if (node.sequence == null) return;
            var _UID = node.sequence.UID;
            var sequences = FindObjectsOfType<SequenceGO>();
            foreach (var sqnc in sequences)
            {
                if (sqnc.UID == _UID)
                {
                    sequenceDirector = sqnc.GetComponent<PlayableDirector>();
                    if (!sequenceDirector)
                    {
                        Debug.LogError("No director found on the sequence game object!");
                        return;
                    }
                    statementPanel.SetActive(true);
                    answerPanel.SetActive(false);
                    statement.SetText("");
                    sequenceDirector.stopped += SequenceEnd;    
                    sequenceDirector.Play();
                }
            }
        }

        public void SequenceEnd(PlayableDirector playableDirector)
        {
            if (playableDirector != sequenceDirector) return;
            sequenceDirector.stopped -= SequenceEnd;
            currentNode.NextNode();
        }

        //method to process question pool nodes. Displays questions in the pool.
        private void ProcessQuestionPoolData()
        {
            //Debug.Log("Processing answer data!");
            var node = currentNode as QuestionPoolNode;
            var q = node?.output;
            GetQPD().DisplayQuestions(q);
        }

        //this method processes different types of lookat information.
        private void ProcessLookAtData()
        {
            //grab the character whose IK we will be manipulated 
            var entity = ((LookAtNode)currentNode).character;
            if (entity == null)
            {
                //throw an error and end process if there is no character to manipulate
                Debug.Log("Entity Null!");
                return;
            }

            //grab the pool index of the character
            var index = CheckCharacter(entity);
            //grab the character hook of the character from the pool.
            //character hook serves as a character controlling layer between the CMT and the character 
            var character = ReturnCharacter(index);
            if (character == null)
            {
                //throw an error and end process if there is no character hook to control
                Debug.Log("Character Null!");
                return;
            }

            //cast current node as a lookat node for ease of access
            var node = (LookAtNode)currentNode;
            if (node == null)
            {
                //return if it is not a lookat node;
                Debug.Log("Node Null!");
                return;
            }

            //grab target character for the node
            var target = ReturnCharacter(CheckCharacter(node.target));

            //if resetLook is set to true in the inspector (graph window), reset previous lookat settings
            if (node.resetLook)
                character.iKController.AssignLookAtTarget(false);
            //else if lookAtPlayerCamera is set to true, the transform of the main camera is assigned as the new lookat target
            else if (node.lookAtPlayerCamera)
                character.iKController.AssignLookAtTarget(true, Camera.main.transform);
            //else it means there is a target transform and if the mouth position of the target character is not null, assign it as the new lookat target 
            else if (target.mouthPosition != null)
                character.iKController.AssignLookAtTarget(true,
                    target.mouthPosition);
            //continue to next node as we are done processing this lookat node
            NextNode();
        }

        //this method processes wait nodes and start necessary coroutines for the wait
        private void ProcessWaitData()
        {
            //hide statement UI
            statementPanel.SetActive(false);

            //set this to true if the current node is cancellable;
            isSkippableWaiting = ((WaitNode)currentNode).cancellable;

            //start the coroutine for the wait
            if (cr_WaitNode != null) StopCoroutine(cr_WaitNode);
            //the duration of the wait is set in the wait node as a variable called 'seconds'
            cr_WaitNode = StartCoroutine(CR_Wait(((WaitNode)currentNode).seconds));
        }

        //this coroutine simply waits for the preset amount of seconds and then continues to the next node
        public IEnumerator CR_Wait(float delay)
        {
            waitForSeconds = new WaitForSeconds(delay);
            yield return waitForSeconds;
            NextNode();
            cr_WaitNode = null;
        }

        //this method ensures that all running processes from the current node are finalized before going to the next node.
        public void NextNode()
        {
            if (currentNode == null) return;
            if (autoSkip != null) StopCoroutine(autoSkip);
            if (backToIdle != null) StopCoroutine(backToIdle);
            if (cr_WaitNode != null) StopCoroutine(cr_WaitNode);
            if (currentlyPlaying != null) currentlyPlaying.Stop();
            currentNode.NextNode();
        }

        //when an answer node or a question pool is being displayed, this method is used to process user input
        public void Answer(int i)
        {
            if (currentNode == null) return;
            switch (currentNode.type)
            {
                case NodeType.Answer:
                    Debug.Log("Answer: " + i);
                    (currentNode as AnswerNode).AnswerQuestion(currentAnswerMatrix[i]);
                    break;
                case NodeType.QuestionPool:
                    Debug.Log("Question: " + i);
                    (currentNode as QuestionPoolNode).AskQuestion(i);
                    break;
                default:
                    return;
            }
        }

        private Coroutine backToIdle, autoSkip;
        private AudioClip statementAudio;

        //this method displays the statement text and plays its audio
        private void ProcessStatementData()
        {
            //grab current node and cast it as a statement node
            var node = currentNode as StatementNode;

            //grab statement audio clip
            statementAudio = node?.audio;

            //calculate the length of the statement in seconds based on the number of letters in the statement, if there is no audio clip
            float len;
            if (statementAudio != null)
                len = statementAudio.length;
            else
                len = node.text.Length / 8f;

            //start a coroutine if autoSkip of this statement node is set to true
            if (node.autoSkip)
            {
                if (autoSkip != null) StopCoroutine(autoSkip);
                autoSkip = StartCoroutine(CR_AutoSkip(len));
            }

            //show statement UI if subtitles are turned on
            if (statementPanel) statementPanel.SetActive(showSubtitles);

            //hide answers UI
            if (answerPanel) answerPanel.SetActive(false);

            //load statement text
            statement.SetText(node.text);

            //check if character is available as the rest of the process requires a character. return if current character is null
            if (currentCharacter == null) return;

            //grab the audio source of the character
            var aS = currentCharacter.audioSource;

            //assign the audio source as currently playing, so that we can stop it once the statement is over
            if (currentlyPlaying != null) currentlyPlaying.Stop();
            if (aS != null) currentlyPlaying = aS;
            else Debug.Log("Audio source of " + currentCharacter.characterName + " is null! Cannot play voice!");
            if (currentlyPlaying != null) currentlyPlaying.Stop();

            //speechBlend controls the lip animation on the character, based on the audio clip
            var sB = currentCharacter.speechBlend;
            if (sB != null)
                if (sB.gameObject.activeInHierarchy)
                    sB.PlaySound(statementAudio, currentlyPlaying);
        }

        //this coroutine simply waits for the preset amount of seconds and then continues to the next node of a statement node automatically
        IEnumerator CR_AutoSkip(float delay)
        {
            yield return new WaitForSeconds(delay);
            if (currentNode != null)
                if (currentNode.type == NodeType.Statement)
                    NextNode();
            yield return null;
        }

        //this coroutine helps keep track of ongoing animations. it is invoked when an animation clip is over.
        IEnumerator CR_BackToIdle(float delay)
        {
            yield return new WaitForSeconds(delay);
            backToIdle = null;
            yield return null;
        }

        //this public method turns subtitles on or off. Subtitles are the text statement shown on screen while a statement is being made.
        public void ToggleSubtitles(bool state)
        {
            showSubtitles = state;
        }

        //this method displays a question and its possible answers on the screen as clickable options.
        private void ProcessAnswerData()
        {
            //grab current node and cast it as an Answer Node
            var node = currentNode as AnswerNode;

            //grab the question text on the answer node
            var q = node?.question;
            if (q.NotNullOrEmpty())
            {
                //show question text if subtitles are on
                statementPanel.SetActive(showSubtitles);

                //load question text
                statement.SetText(q);
            }
            else
                //hide statement panel if there is no text in the answer node's question section
                statementPanel.SetActive(false);

            //clear the text that was left from the previous answer node on each option
            foreach (var t in answers)
                t.SetText("");

            //load new options
            //start by counting to see how many options there is.
            var length = node.output.Count;

            //create an answer matrix so that we can randomize options on demand
            currentAnswerMatrix = new int[length];

            //if randomize is set to true
            if (node.randomize)
            {
                //create a temp list of options
                var temp = new List<int>();

                //populate the list
                for (int i = 0; i < length; i++)
                    temp.Add(i);

                //randomly select an option and add it to the answer matrix
                for (int i = 0; i < length; i++)
                {
                    var selected = temp[Random.Range(0, temp.Count)];
                    currentAnswerMatrix[i] = selected;

                    //remove the option from the pool so we do not re-select it by accident
                    temp.Remove(selected);
                }
            }
            else //if randomize is not set to true, just populate answer matrix in ascending order.
                for (int i = 0; i < length; i++)
                    currentAnswerMatrix[i] = i;

            //activate/deactivate options and update their text
            for (var i = 0; i < answers.Length; i++)
            {
                //grab option for ease of access
                var option = answers[i];

                //activate/deactivate options based on the number of options in the node
                option.transform.parent.gameObject.SetActive(i < length);
                if (i < length)
                {
                    //this is where randomization kicks in, if it is set to true, else nothing changes
                    var index = currentAnswerMatrix[i];

                    //grab the interactive component of the option
                    var temp = option.transform.parent.GetComponent<UIItem>();
                    if (temp)

                        //enable/disable interactive component. Options can still be active and displaying, even if they are set a not-interactive
                        temp.enabled = node.output[index].isActive;

                    //update option text
                    answers[i].SetText(node.output[index].text);
                }
            }

            answerPanel.SetActive(true);
        }

        //this method carries out necessary operations to end a graph and is invoked by the completion node
        public void EndGraph()
        {
            //hide UI elements
            if (statementPanel != null)
                statementPanel.SetActive(false);
            if (answerPanel != null)
                answerPanel.SetActive(false);

            //Invoke Final Event
            InvokeEvent((currentNode as CompletionNode)?.finalEventKey);

            //clear internal cache
            currentGraph = null;
            currentNode = null;
            isReady = false;
        }

        #endregion

        #region ANIMATION RELAY METHODS

        //Following methods control the runtime animator controller of current character
        //by manipulating its parameters based on different types of animation nodes
        public void TriggerAnimation(string trigger)
        {
            SetExitTime(-1f);
            if (currentCharacter == null) return;
            if (currentCharacter.animator == null)
                return;
            if (trigger != null)
                if (trigger.Length > 0)
                    currentCharacter.animator.SetTrigger(trigger);
        }

        public void TriggerAnimation(string trigger, float exitTime)
        {
            SetExitTime(exitTime);
            TriggerAnimation(trigger);
        }

        private void SetExitTime(float exitTime)
        {
            if (backToIdle != null) StopCoroutine(backToIdle);
            if (exitTime >= 0) backToIdle = StartCoroutine(CR_BackToIdle(exitTime));
        }

        public void BoolAnimation(string id, bool state)
        {
            SetExitTime(-1f);
            if (currentCharacter.animator == null)
            {
                //Debug.LogWarning("No animator controller present on the current character!");
                return;
            }

            currentCharacter.animator.SetBool(id, state);
        }

        public void BoolAnimation(string id, bool state, float exitTime)
        {
            SetExitTime(exitTime);
            if (currentCharacter.animator == null)
            {
                //Debug.LogWarning("No animator controller present on the current character!");
                return;
            }

            currentCharacter.animator.SetBool(id, state);
        }

        public void FloatAnimation(string id, float floatValue)
        {
            SetExitTime(-1f);
            if (currentCharacter.animator == null)
            {
                //Debug.LogWarning("No animator controller present on the current character!");
                return;
            }

            currentCharacter.animator.SetFloat(id, floatValue);
        }

        public void FloatAnimation(string id, float floatValue, float exitTime)
        {
            SetExitTime(exitTime);
            if (currentCharacter.animator == null)
            {
                //Debug.LogWarning("No animator controller present on the current character!");
                return;
            }

            currentCharacter.animator.SetFloat(id, floatValue);
        }

        public void IntegerAnimation(string id, int intValue)
        {
            SetExitTime(-1f);
            if (currentCharacter.animator == null)
            {
                //Debug.LogWarning("No animator controller present on the current character!");
                return;
            }

            currentCharacter.animator.SetInteger(id, intValue);
        }

        public void IntegerAnimation(string id, int intValue, float exitTime)
        {
            SetExitTime(exitTime);
            if (currentCharacter.animator == null)
            {
                //Debug.LogWarning("No animator controller present on the current character!");
                return;
            }

            currentCharacter.animator.SetInteger(id, intValue);
        }

        #endregion

        #region CMT 2.0

        //Export conversation as text file
        public int convoToTextIndex;

        //this method writes a conversation to disk. change convoToTextIndex variable to pick a conversation from the pool 
        public void WriteConvoToDisk()
        {
            WriteToFile(conversations[convoToTextIndex]);
        }

        //this method writes a specified conversation to disk
        public void WriteToFile(Conversation convo)
        {
            //start a coroutine to parse through the convo to collect statement nodes
            if (cR_ParseConvoToDisk == null) cR_ParseConvoToDisk = StartCoroutine(CR_ParseConvoToDisk(convo));
        }

        private Coroutine cR_ParseConvoToDisk;
        private StatementNode[] statementsArray;
        [HideInInspector] public List<StatementNode> statements;

        //this coroutine goes through a given conversation and parses its information to text format
        private IEnumerator CR_ParseConvoToDisk(Conversation convo)
        {
            var wait = new WaitForEndOfFrame();

            //create file with the convo name
            StreamWriter sw = new StreamWriter(Application.persistentDataPath + "/" + convo.title + ".txt");

            //add constants to the script
            var start = "┌";
            var node = "├";
            var end = "└";

            string[] letters =
            {
                "a", "b", "c", "d", "e", "f", "g", "h", "i", "j", "k", "l", "m", "n", "o", "p", "q", "r", "s", "t", "u",
                "w", "v", "x", "y", "z"
            };


            //define a list for statement nodes that we will go look for in the graph
            statements = new List<StatementNode>();

            //grab graph
            var graph = convo.graph;

            //define a node variable for current node
            BaseNode currentNode = null;

            //find start node of the graph
            foreach (var graphNode in graph.nodes)
                if (((BaseNode)graphNode).type == NodeType.Start && currentNode == null)
                    currentNode = (BaseNode)graphNode;

            //see if we have a start node at hand, and bail if we don't.
            if (currentNode == null)
                Debug.LogError("The convo to be parsed contains no start node. Terminating process.");
            else
            {
                //parse through the convo to get the statements
                while (currentNode != null && currentNode.type != NodeType.End)
                {
                    currentNode = GetNextNode(currentNode);
                    if (currentNode != null && currentNode.type == NodeType.Statement)
                    {
                        statements.Add((StatementNode)currentNode);
                        Debug.Log(currentNode.UID);
                    }

                    yield return wait;
                }

                //convert list to array to lock indexes
                statementsArray = statements.ToArray();

                //add title to text
                sw.WriteLine(convo.title.ToUpper());

                //draw a line
                sw.WriteLine("");
                sw.WriteLine("-------------------------------------------------------------");
                sw.WriteLine("");

                //write each statement and answer to text

                for (int i = 0; i < statementsArray.Length; i++)
                {
                    //grab statement
                    var statement = statementsArray[i];
                    //create 1st line - which includes the statement - or the question
                    var line = (i < 10 ? 0.ToString() : "") + i + start + statement.text;

                    //write line
                    sw.WriteLine(line);

                    //grab the node that is connected to the output of the current node
                    var outputNode = GetNextNode(statement);

                    //check if the output is connected to an answer node or another statement node
                    if (outputNode.type ==
                        NodeType
                            .Statement) //if it is a statement node, just add the outputIndex number of that node as a link for the 2nd line
                    {
                        //get outputIndex of output node
                        var index = GetIndex(outputNode);

                        //create 2nd line
                        line = (i < 10 ? 0.ToString() : "") + i + end + "--[" + (index < 10 ? 0.ToString() : "") +
                               index + "]";

                        //write line
                        sw.WriteLine(line);
                    }
                    else if
                        (outputNode.type ==
                         NodeType.Answer) //if it is an answer node, add a line for each possible answer and add the outputIndex number link in [outputIndex] format to the end of relevant ones.
                    {
                        //get output count
                        var count = ((AnswerNode)outputNode).output.Count;
                        //write lines recursively
                        for (var j = 0; j < count; j++)
                        {
                            //grab answer
                            var answer = ((AnswerNode)outputNode).output[j];

                            //create line - check if it is last to draw a different line for the final unit
                            if (j == count - 1) //final line
                                line = (i < 10 ? 0.ToString() : "") + i + end + "-" + letters[j] + "." + answer.text;
                            else //interim line
                                line = (i < 10 ? 0.ToString() : "") + i + node + "-" + letters[j] + "." + answer.text;

                            //write line
                            sw.WriteLine(line);
                        }
                    }

                    //draw a line
                    sw.WriteLine("");
                    sw.WriteLine("-------------------------------------------------------------");
                    sw.WriteLine("");
                }

                //jump one line
                sw.WriteLine("");

                //end text
                sw.WriteLine("------THE END------");

                //close file
                sw.Close();
            }

            cR_ParseConvoToDisk = null;
            yield return null;
        }

        //this method returns the index number of a statement node in the pool, based on its unique identity value
        private int GetIndex(string UID)
        {
            for (int i = 0; i < statementsArray.Length; i++)
                if (statementsArray[i].UID == UID)
                    return i;
            return -1;
        }

        //this method returns the index number of a statement node in the pool
        private int GetIndex(BaseNode node)
        {
            return GetIndex(node.UID);
        }

        //this method returns the next node, connected to the output of the given node.
        //an output index can be passed to the method for nodes with multiple outputs. default is 0
        private BaseNode GetNextNode(BaseNode node, int outputIndex = 0)
        {
            if (node == null) return null;
            var port = node.GetOutputPort("output");
            if (port == null)
                return null;
            if (port.ConnectionCount > 0)
                return port.GetConnection(outputIndex).node as BaseNode;
            return null;
        }

        //this method returns the next node, connected to the given output port.
        private BaseNode GetConnectedNode(NodePort port)
        {
            if (port == null)
                return null;
            if (port.ConnectionCount > 0)
                return port.GetConnection(0).node as BaseNode;
            return null;
        }

        #endregion
    }

    #region CLASSES_STRUCTS

    [Serializable]
    public class Conversation
    {
        public string title;
        public ConversationMatrixGraph graph;
        public CMTEvent[] events;
        public CMTCondition[] conditions;
    }

    [Serializable]
    public struct CMTEvent
    {
        public string key;
        public UnityEvent _event;

        public CMTEvent(string _key)
        {
            key = _key;
            _event = new UnityEvent();
        }
    }

    [Serializable]
    public struct CMTCondition
    {
        public string key;
        public Condition condition;

        public CMTCondition(string _key)
        {
            key = _key;
            condition = new Condition();
        }
    }

    #endregion
}