using System.Collections.Generic;
using System.Collections;
using Photon.Pun;
using UnityEngine;
public class PointManager : MonoBehaviourPun
{
    public Dictionary<Photon.Realtime.Player, int> players = new Dictionary<Photon.Realtime.Player, int>();
    UIManager UIManager;
    List<int> AllScores = new List<int>();

    void Start()
    {
        UIManager = GameObject.FindGameObjectWithTag("UIManager").GetComponent<UIManager>();
        
    }

    private void Update()
    {
        
    }
    [PunRPC]
    public void SetPoints(Photon.Realtime.Player player, int points)
    {
        if (!players.ContainsKey(player)) Debug.LogError("Can't set points of unknown player");
        StartCoroutine(PointNotification(player, points));
    }

    IEnumerator PointNotification(Photon.Realtime.Player player, int points)
    {
        yield return new WaitForSeconds(2);
        // UI Notification
        if (player == PhotonNetwork.LocalPlayer)
        {
            int addedPoints = points - players[player]; // Difference of prev and new points
            if (points % 2 == 0)
            {
                if (addedPoints > 0)
                {
                    UIManager.GenerateDissolvingTextPopup("+" + addedPoints, 2f, "black");
                    FindObjectOfType<MessageUI_Setting>().ChatBox.SetActive(true);
                    FindObjectOfType<DF2Client>().SendTextToChatbot("gain point");

                }
                else
                {
                    UIManager.GenerateDissolvingTextPopup(addedPoints.ToString(), 2f, "black");
                    FindObjectOfType<MessageUI_Setting>().ChatBox.SetActive(true);
                    FindObjectOfType<DF2Client>().SendTextToChatbot("lose point");
                }
            }
                
        }
        
        players[player] = points;
        AllScores.Clear();
        foreach (var entry in players)
        {
            AllScores.Add(entry.Value);
        }
        AllScores.Sort();

        if(players[player]==AllScores[0])
        {
			FindObjectOfType<MessageUI_Setting>().ChatBox.SetActive(true);
			FindObjectOfType<DF2Client>().SendTextToChatbot("oben");
		}
        else if(players[player] == AllScores[AllScores.Count-1])
        {
			FindObjectOfType<MessageUI_Setting>().ChatBox.SetActive(true);
			FindObjectOfType<DF2Client>().SendTextToChatbot("unten");
		}
        else
        {
			FindObjectOfType<MessageUI_Setting>().ChatBox.SetActive(true);
			FindObjectOfType<DF2Client>().SendTextToChatbot("mitte");
		}
    }
    public void AddPlayer(Photon.Realtime.Player player)
    {
        players.Add(player, 0);
    }

    public int GetPointsCount(Photon.Realtime.Player player)
    {
        return players[player];
    }
}