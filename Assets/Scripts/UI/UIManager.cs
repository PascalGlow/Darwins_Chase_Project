using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Photon.Pun;

public class UIManager : MonoBehaviourPun
{
    [SerializeField] Canvas TextPopup;
    [SerializeField] Canvas InputPopup;
    [SerializeField] Canvas VotePopup;
    [SerializeField] Canvas TimerPopup;
    [SerializeField] Canvas GMPopup;
    [SerializeField] Canvas EndingPopup;
    PointManager PointManager;
    public const float waitingTime = 15f;
    
    public static void ToggleTransparancy(GameObject target, Player playerScript)
    {
        if (playerScript.gameObject.GetComponent<PhotonView>().IsMine) //Only toggle on target owner view
        {
            SpriteRenderer sprite = target.GetComponent<SpriteRenderer>();
            sprite.color = (sprite.color.a < 1) ? new Color(1, 1, 1, 1) : new Color(1, 1, 1, 0.5f);
            target.GetComponent<BoxCollider2D>().enabled = !target.GetComponent<BoxCollider2D>().enabled;
		}  
    }

    public static void DrawText(string text, TMP_Text textField) {
        textField.text = text;
    }

    [PunRPC]
    public Canvas GenerateTextPopup(string text, WindowPosition pos, WindowSize height, WindowSize width, float waitingTime, string color) {
        Canvas go = Instantiate(TextPopup, new Vector3(0,(int)pos,0), Quaternion.identity);
        Vector2 size = new Vector2((int)width, (int)height);
        go.GetComponent<RectTransform>().sizeDelta = size;
        //go.transform.GetChild(0).GetChild(0).gameObject.GetComponent<RectTransform>().sizeDelta = size;
        go.GetComponentInChildren<TMP_Text>().text = text;

        switch(color)
        {
            case "orange":
            go.GetComponentInChildren<Image>().color = new Color(1f, 0.3018998f, 0.2037079f); // Orange color
            break;
            case "blue":
            go.GetComponentInChildren<Image>().color = new Color(0.25f, 0.56f, 0.62f);
            break;
            case "grey":
            go.GetComponentInChildren<Image>().color = new Color(0.51f, 0.59f, 0.58f);
            break;
            case "red":
            go.GetComponentInChildren<Image>().color = new Color(0.75f, 0.02f, 0.29f);
            break;
            default:
            go.GetComponentInChildren<Image>().color = new Color(0f, 0f, 0f, 0.8f);
            break;
        }

        Destroy(go.gameObject, waitingTime); // Destroy after x seconds
        return go;
    }

    public Canvas GenerateDissolvingTextPopup(string text, float hovertime, string color) {
        float lifetime = hovertime + 3f;
        Canvas go = Instantiate(TextPopup, new Vector3(0,(int)WindowPosition.Top,0), Quaternion.identity);
        Vector2 size = new Vector2((int)WindowSize.Width/1.3f, (int)WindowSize.One_Third_Height/1.2f);
        go.GetComponent<RectTransform>().sizeDelta = size;
        //go.transform.GetChild(0).GetChild(0).gameObject.GetComponent<RectTransform>().sizeDelta = size;
        go.GetComponentInChildren<Image>().color = new Color(1f, 0.3018998f, 0.2037079f, .9f); // Orange color
        go.GetComponentInChildren<TMP_Text>().text = text;

        switch(color)
        {
            case "orange":
            go.GetComponentInChildren<Image>().color = new Color(1f, 0.3018998f, 0.2037079f); // Orange color
            break;
            case "blue":
            go.GetComponentInChildren<Image>().color = new Color(0.25f, 0.56f, 0.62f);
            break;
            case "grey":
            go.GetComponentInChildren<Image>().color = new Color(0.51f, 0.59f, 0.58f);
            break;
            case "red":
            go.GetComponentInChildren<Image>().color = new Color(0.75f, 0.02f, 0.29f);
            break;
            default:
            go.GetComponentInChildren<Image>().color = new Color(0f, 0f, 0f, 0.8f);
            break;
        }

        Destroy(go.gameObject, lifetime); // Destroy after x seconds
        StartCoroutine(DissolveTextPopup(go, lifetime, hovertime));
        return go;
    }

    [PunRPC]
    public Canvas GenerateEndingPopup()
    {
        PointManager = GameObject.FindGameObjectWithTag("PointManager").GetComponent<PointManager>();
        Canvas go = Instantiate(EndingPopup);
        TMP_Text textField = go.transform.GetChild(0).GetComponentInChildren<TMP_Text>();
        go.transform.GetChild(0).GetComponentInChildren<Button>().onClick.AddListener(() => {
            PhotonNetwork.LeaveRoom();
            PhotonNetwork.LoadLevel("Lobby");
        });
        StringBuilder scoreboardString = new StringBuilder();
        List<Photon.Realtime.Player> players = new List<Photon.Realtime.Player>(PhotonNetwork.PlayerList);
        List<Photon.Realtime.Player> playersSorted = players.OrderByDescending(p => p.CustomProperties["points"]).ToList();

        for(int i = 0; i < playersSorted.Count; i++)
        {
            scoreboardString.AppendLine((i+1) + ". " + playersSorted[i].NickName + ": " + playersSorted[i].CustomProperties["points"] + " Points");
        }
        textField.text = scoreboardString.ToString();
        return go;
    }

    IEnumerator DissolveTextPopup(Canvas go, float lifetime, float hovertime) {
        yield return new WaitForSeconds(hovertime); // Wait to be read
        float startTime = Time.time;
        Vector2 dest = new Vector2(0, 1500);
        // Slowly move up for 5s
        while((Time.time - startTime) < lifetime)
        {
            if (go == null) break;
            go.gameObject.transform.position = Vector2.MoveTowards(go.gameObject.transform.position, dest, 1000 * Time.smoothDeltaTime); // Move field
            
            // Fade background color, border and text
            Image panelBackground = go.GetComponentInChildren<Image>();
            panelBackground.color = new Color(panelBackground.color.r, panelBackground.color.g, panelBackground.color.b, panelBackground.color.a * .9f);
            Image border = go.transform.GetChild(2).GetComponent<Image>();
            border.color = new Color (border.color.r, border.color.g, border.color.b, border.color.a * .9f);
            TMP_Text text = go.GetComponentInChildren<TMP_Text>();
            text.color = new Color(text.color.r, text.color.g, text.color.b, text.color.a * .9f);

            yield return new WaitForSeconds(.05f);
        }
    }
    public Canvas InputTextPopup(string color) {
        Canvas go = Instantiate(InputPopup, new Vector3(0,(int)WindowPosition.Three_Third,0), Quaternion.identity);
        switch(color)
        {
            case "orange":
            go.GetComponentInChildren<Image>().color = new Color(1f, 0.3018998f, 0.2037079f); // Orange color
            break;
            case "blue":
            go.GetComponentInChildren<Image>().color = new Color(0.25f, 0.56f, 0.62f);
            break;
            case "grey":
            go.GetComponentInChildren<Image>().color = new Color(0.51f, 0.59f, 0.58f);
            break;
            case "red":
            go.GetComponentInChildren<Image>().color = new Color(0.75f, 0.02f, 0.29f);
            break;
            default:
            go.GetComponentInChildren<Image>().color = new Color(0f, 0f, 0f, 0.8f);
            break;
        }
        return go;
    }

    public Canvas GenerateVotePopup(string color) {
        Canvas go = Instantiate(VotePopup, new Vector3(0,(int)WindowPosition.Three_Third,0), Quaternion.identity);
        go.GetComponent<RectTransform>().sizeDelta = new Vector2((int)WindowSize.Width, (int)WindowSize.One_Third_Height);
        
        switch(color)
        {
            case "orange":
            go.GetComponentInChildren<Image>().color = new Color(1f, 0.3018998f, 0.2037079f); // Orange color
            break;
            case "blue":
            go.GetComponentInChildren<Image>().color = new Color(0.25f, 0.56f, 0.62f);
            break;
            case "grey":
            go.GetComponentInChildren<Image>().color = new Color(0.51f, 0.59f, 0.58f);
            break;
            case "red":
            go.GetComponentInChildren<Image>().color = new Color(0.75f, 0.02f, 0.29f);
            break;
            default:
            go.GetComponentInChildren<Image>().color = new Color(0f, 0f, 0f, 0.8f);
            break;
        }
        
        Destroy(go.gameObject, waitingTime); // Destroy after x seconds
        return go;
    }

    public Canvas GenerateTimer(float seconds) {
        Canvas go = Instantiate(TimerPopup);
        go.GetComponentInChildren<Timer>().duration = seconds;
        go.GetComponentInChildren<Timer>().Begin(seconds);
        return go;
    }

    public Canvas GenerateGMView(float waitingTime)
    {
        Canvas go = Instantiate(GMPopup);
        Destroy(go.gameObject, waitingTime);
        return go;
    }
}

public enum WindowSize : int {
    Three_Third_Height = 2200,
    Two_Third_Height = 1500,
    One_Third_Height = 800,
    Width = 4800
}

public enum WindowPosition : int {
    Top = 1200,
    One_Third = 800,
    Two_Third = -100,
    Three_Third = -1000
}