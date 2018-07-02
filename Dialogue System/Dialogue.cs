using UnityEngine;

//We need collider for our OnMouseDown function if it's a 3D game object
[RequireComponent(typeof(Collider))]
public class Dialogue : MonoBehaviour {

    public TextAsset npcText; //npc textAsset
    public TextAsset plyText; //player textAsset;
    public bool npcIsFirst = true; //bool to check who starts conversation and to controll who are talking now
    public DialogueSystem dialSystem;

    private void Start()
    {
        if (dialSystem == null)
            Debug.LogError("Unassigned variable type of DialogueSystem on " + gameObject.name.ToUpper());
    }

    //Function is called when left mouse button pressed down over object
    private void OnMouseDown()
    {
        dialSystem.enabled = true;
        dialSystem.AssetToQueue(npcText, plyText, npcIsFirst);
        enabled = false;
    }

}
