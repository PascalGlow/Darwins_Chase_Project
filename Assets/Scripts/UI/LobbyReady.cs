using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Photon.Pun;
using System.Collections.Generic;
using System.Collections;
public class LobbyReady : MonoBehaviourPunCallbacks
{   
    [SerializeField] TMP_InputField inputField;
    [SerializeField] Button startButton;
    void Start()
    {
        if (PhotonNetwork.LocalPlayer.NickName == "") PhotonNetwork.LocalPlayer.NickName = "Darwin " + Random.Range(0,10000).ToString();
        if (!PhotonNetwork.IsMasterClient) startButton.interactable = false;
        StartCoroutine(RefreshPlayerList());
    }

    IEnumerator RefreshPlayerList()
    {   
        inputField.text = "";
        foreach(KeyValuePair<int, Photon.Realtime.Player> p in PhotonNetwork.CurrentRoom.Players)
        {
            yield return new WaitUntil(() => p.Value.NickName != "");
            inputField.text += p.Value.NickName + "\n";
        }
    }
    public void Start_Click()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            PhotonNetwork.CurrentRoom.IsOpen = false; // No one can join now
            PhotonNetwork.LoadLevel("Board");
        } else {
            Debug.Log("Only the host can start the game");
        }
    }

    public void Leave_Click()
    {
        PhotonNetwork.LeaveRoom();
        PhotonNetwork.LoadLevel("Lobby");
    }

    public override void OnPlayerEnteredRoom(Photon.Realtime.Player newPlayer)
    {
        StartCoroutine(RefreshPlayerList());
        base.OnPlayerEnteredRoom(newPlayer);
    }

    public override void OnPlayerLeftRoom(Photon.Realtime.Player leavingPlayer)
    {
        StartCoroutine(RefreshPlayerList());
    }
}
