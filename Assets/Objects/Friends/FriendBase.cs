using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FriendBase : MonoBehaviour
{
    public string friendName;
    public string friendDescription;


    public bool openDialogue;
    public FriendDialogue friendDialogue;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnInteract()
    {
        if (openDialogue)
        {
            friendDialogue.OpenDialogue();
        }
    }
}
