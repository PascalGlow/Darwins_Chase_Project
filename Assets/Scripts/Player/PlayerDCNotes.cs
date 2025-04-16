using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class PlayerDCNotes : MonoBehaviour
{
    public void AddNote(string text, int type)
    {
        //Add note to local player list
        List<string> notes;
        if (PhotonNetwork.LocalPlayer.CustomProperties.ContainsKey(type.ToString()))
        {
            string[] notesString = (string[]) PhotonNetwork.LocalPlayer.CustomProperties[type.ToString()];
            notes = new List<string>(notesString);
        }
        else {
            notes = new List<string>();
        }
        notes.Add(text);

        ExitGames.Client.Photon.Hashtable playerNotes = PhotonNetwork.LocalPlayer.CustomProperties;
        playerNotes[type.ToString()] = notes.ToArray();
        PhotonNetwork.LocalPlayer.SetCustomProperties(playerNotes);
        Debug.Log("Added new note for: " + PhotonNetwork.LocalPlayer.NickName + "\n Text: " + text + ", Type: " + type);
    }
    public void RemoveNote(string text)
    {
        foreach (DictionaryEntry dcFieldNotes in PhotonNetwork.LocalPlayer.CustomProperties)
        {
            if (dcFieldNotes.Value != null)
            {
                IEnumerable<string> stringArray = dcFieldNotes.Value as IEnumerable<string>;
                if(stringArray != null)
                {
                    List<string> stringList = new List<string>(stringArray);
                    foreach (string note in stringArray)
                    {
                        if (note.Equals(text))
                        {
                            stringList.Remove(text);
                            ExitGames.Client.Photon.Hashtable playerNotes = PhotonNetwork.LocalPlayer.CustomProperties;
                            playerNotes[dcFieldNotes.Key] = stringList.ToArray();
                            PhotonNetwork.LocalPlayer.SetCustomProperties(playerNotes);
                            Debug.Log("Removed note: " + "\n Text: " + text);
                        }
                    }
                }
            }
        }
    }
}