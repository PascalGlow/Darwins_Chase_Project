using UnityEngine;
using Photon.Pun;
using System.Text;
using TMPro;
public class Scoreboard : MonoBehaviour
{   
    Canvas canvasObject;
    [SerializeField] TMP_Text playerCountText;
    [SerializeField] PointManager pointManager;
    StringBuilder playerNicknames = new StringBuilder();
    
    void Start()
    {
        canvasObject = GetComponent<Canvas>();
        pointManager = pointManager.GetComponent<PointManager>();
    }
    void Update()
    {
       // if (Input.GetKeyDown(KeyCode.Tab))
       // {   
            UpdateScoreBoard();
            canvasObject.enabled = true;
       // }

        //if (Input.GetKeyUp(KeyCode.Tab))
       // {
           // canvasObject.enabled = false;
        //}
    }

    void UpdateScoreBoard()
    {
        Photon.Realtime.Player[] playerList = PhotonNetwork.PlayerList; // Get all players connected to network

        playerNicknames.Clear(); // Reset String
        foreach (Photon.Realtime.Player player in playerList)
        {
            int pointCount = pointManager.GetPointsCount(player);
            playerNicknames.AppendLine(player.NickName + "\t Punkte: " + pointCount); // Build players string
        }
        string scoreboard = "Spieleranzahl: " + playerList.Length.ToString() + "\n" + playerNicknames; // Combine string
        UIManager.DrawText(scoreboard, playerCountText); // Display string
    }
}
