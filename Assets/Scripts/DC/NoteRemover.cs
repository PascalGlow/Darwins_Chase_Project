using UnityEngine;
using TMPro;
using Photon.Pun;
public class NoteRemover : MonoBehaviour
{
    public void DestroyNote()
    {
        // Remove note from player notes list
        PlayerDCNotes PlayerDCNotes = GetComponentInParent<PlayerDCNotes>();
        string text = GetComponentInParent<TMP_InputField>().text;
        PlayerDCNotes.RemoveNote(text); 
        Destroy(transform.parent.gameObject);
    }
}
