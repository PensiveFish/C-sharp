using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;
using System.Collections.Generic;

public class DialogueSystem : MonoBehaviour {

    [Header("UI Dialogue Stuff")]
    public Text[] textUI; //0 for player, 1 for npc
    public float displayTime = 3f; //how long text on screen

    #region PrivateVariables

    Queue<string> npcSentences = new Queue<string>();
    Queue<string> plySentences = new Queue<string>();
    bool npcIsFirst;
    Coroutine lastCorout;

    #endregion

    private void Start()
    {
        if (textUI.Length < 2)
            Debug.LogError("Unassigned variable type of Text for NPC/Player on " + gameObject.name.ToUpper());
    }

    void Update () {
        if (Input.GetKeyDown(KeyCode.Return))
        {
            NextSentence(true); //forcing next sentence of dialogue
        }
    }

    //Main function for displaying sentences, boolean used to see if we are skipping phrases
    void NextSentence(bool isForced)
    {
        if (npcIsFirst)
        {
            if (isForced)
            {
                StopCoroutine(lastCorout);
                textUI[1].gameObject.SetActive(false);
            }

            if (npcSentences.Count == 0) //if npc queue is empty then we are done with npc and no longer want to be able to interact with him
            {
                if (plySentences.Count == 0) //before that we are checking if player queue also empty
                    Debug.Log("End of dialogue");
                else
                    Debug.LogWarning("Npc sentences is ended but player queue still not empty, check textAssets of " + gameObject.name.ToUpper());

                enabled = false; //disable this script
                return;
            }


            if (npcSentences.Peek() == "") //if next string is empty string
            {
                npcSentences.Dequeue(); //remove empty string
                npcIsFirst = !npcIsFirst; //change bool to switch talking side
                SwitchTextUI(0);
                NextSentence(false); //call this function again
                return;
            }
            else
            {
                textUI[1].text = npcSentences.Dequeue();
                lastCorout = StartCoroutine("DisplayText", 1);
            }
        }
        else
        {
            if (isForced)
            {
                StopCoroutine(lastCorout);
                textUI[0].gameObject.SetActive(false);
            }

            if (plySentences.Count == 0) //if player queue is empty then we are done with npc and no longer want to be able to interact with him
            {
                if (npcSentences.Count == 0) //before that we are checking if npc queue also empty
                    Debug.Log("End of dialogue");
                else
                    Debug.LogWarning("Player sentences is ended but npc queue still not empty, check textAssets of " + gameObject.name.ToUpper());

                enabled = false; //disable this script
                return;
            }


            if (plySentences.Peek() == "") //if next string is empty string
            {
                plySentences.Dequeue(); //remove empty string
                npcIsFirst = !npcIsFirst; //change bool to switch talking side
                SwitchTextUI(1);
                NextSentence(false); //call this function again
                return;
            }
            else
            {
                textUI[0].text = plySentences.Dequeue();
                lastCorout = StartCoroutine("DisplayText", 0);
            }
        }
    }

    //Function to transform TextAssets into Queue
    public void AssetToQueue(TextAsset npc, TextAsset player, bool npcIsFirst_)
    {
        npcIsFirst = npcIsFirst_;

        npcSentences.Clear(); //clear queues in case we have something there from previous dialogue
        plySentences.Clear();

        //I'm using StringSplitOptions.None because by empty line our script will understand that it's time to switch person who is talking
        string[] textArray = player.text.Split(new string[] { "\r\n", "\r" }, StringSplitOptions.None);
        int length = textArray.Length;

        for (int i = 0; i < length; i++)
            plySentences.Enqueue(textArray[i]);

        textArray = npc.text.Split(new string[] { "\r\n", "\r" }, StringSplitOptions.None);
        length = textArray.Length;

        for (int i = 0; i < length; i++)
            npcSentences.Enqueue(textArray[i]);

        //Enabling proper textUI
        if (npcIsFirst)
            textUI[1].gameObject.SetActive(true);
        else
            textUI[0].gameObject.SetActive(true);

        NextSentence(false);
    }

    //Function for switching enabled text
    void SwitchTextUI(int index)
    {
        textUI[1 - index].gameObject.SetActive(false);
        textUI[index].gameObject.SetActive(true);
    }

    //Show text
    IEnumerator DisplayText(int index)
    {
        textUI[index].gameObject.SetActive(true);
        yield return new WaitForSeconds(displayTime);
        textUI[index].gameObject.SetActive(false);
        NextSentence(false);
    }
}
