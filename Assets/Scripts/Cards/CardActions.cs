using UnityEngine;
using TMPro;
using Photon.Pun;
using UnityEngine.UI;
using UnityEngine.Events;
using System.Collections;
using System.Collections.Generic;
using System;
public class CardActions : MonoBehaviourPun
{
    UIManager UIManager;
    HandleVoteClick HandleVoteClick;
    Canvas currentQuestionCanvas;
    Canvas currentAnswerCanvas;
    Player Player;
    PointManager PointManager;
    GameManager GameManager;
    NoteSpawning NoteSpawning;
    Dictionary<Photon.Realtime.Player, string> playerAnswers;
    
    void Start()
    {
        UIManager = GameObject.FindGameObjectWithTag("UIManager").GetComponent<UIManager>();
        GameManager = GameObject.FindGameObjectWithTag("GameManager").GetComponent<GameManager>();
        Player = GetComponent<Player>();
        PointManager = GameObject.FindGameObjectWithTag("PointManager").GetComponent<PointManager>();
        HandleVoteClick = GameObject.FindGameObjectWithTag("VotingManager").GetComponent<HandleVoteClick>();
        NoteSpawning = GameObject.FindGameObjectWithTag("DC").GetComponent<NoteSpawning>();
    }

    public void gameEndAction()
    {
        photonView.RPC("SubtractPoints", RpcTarget.All);
        PhotonNetwork.LocalPlayer.CustomProperties.Clear();
    }
    

    public IEnumerator AskForInputQuiz(int points, string correctAnswer, float waitingTime)
    {   
        currentAnswerCanvas.GetComponentInChildren<Button>().onClick.AddListener(StopAskForInputQuiz(points, correctAnswer, waitingTime));
        Destroy(GameObject.FindGameObjectWithTag("Timer"));
        UIManager.GenerateTimer(waitingTime);
        yield return new WaitForSeconds(waitingTime); // Wait for user solution input
        StopAskForInputQuiz(points, correctAnswer, waitingTime)();
    }

    public IEnumerator AskForInputCreativity(int points, float readingTime)
    {   
        currentAnswerCanvas.GetComponentInChildren<Button>().onClick.AddListener(() => {
            // Save answer and destroy input fields if submitted, still wait after it tho
            string answer = currentAnswerCanvas.GetComponentInChildren<TMP_Text>().text;
            if (currentAnswerCanvas != null) photonView.RPC("SetPlayerAnswer", RpcTarget.All, PhotonNetwork.LocalPlayer, answer);
            Destroy(currentAnswerCanvas.gameObject);
            if (currentQuestionCanvas != null) Destroy(currentQuestionCanvas.gameObject);
        });

        Destroy(GameObject.FindGameObjectWithTag("Timer"));
        UIManager.GenerateTimer(readingTime);
        yield return new WaitForSeconds(readingTime); // Wait for answer to be read + to reply to it
        
        try {
            foreach (Photon.Realtime.Player p in playerAnswers.Keys)
            {
                // Show answer for each player
                string answerText = "Antwort von " + p.NickName + ": " + playerAnswers[p];
                UIManager.GenerateTextPopup(answerText, WindowPosition.One_Third, WindowSize.Two_Third_Height, WindowSize.Width, (answerText.Split(' ').Length * 60 / 200) + 4f, "red");
                if (p.NickName != PhotonNetwork.LocalPlayer.NickName) // Only vote if not your own answer
                {
                    HandleVoteClick.StartVoting(points, p, false);
                } else StartCoroutine(HandleVoteClick.WaitForVotes(points, p, false)); // Wait for others to vote
                yield return new WaitForSeconds(UIManager.waitingTime); // Wait for votes and stuff to happen before going to next player
            }
        } finally {
            GameManager.nextTurn();
        }
    }

    public UnityAction StopAskForInputQuiz(int points, string correctAnswer, float waitingTime)
    {
        return new UnityAction(() => {
            Destroy(GameObject.FindGameObjectWithTag("Timer")); // Destroy timer since already clicked or time over
            if (currentAnswerCanvas != null)
            {
                StopCoroutine("AskForInputQuiz"); // In case of button click the ongoing Coroutine has to be terminated
                string answer = currentAnswerCanvas.GetComponentInChildren<TMP_Text>().text;
                if (currentQuestionCanvas != null) Destroy(currentQuestionCanvas.gameObject);
                if (currentAnswerCanvas != null) Destroy(currentAnswerCanvas.gameObject);
                UIManager.photonView.RPC("GenerateTextPopup", RpcTarget.Others, "Gegebene Antwort: " + answer, WindowPosition.Top, WindowSize.One_Third_Height, WindowSize.Width, waitingTime/3, "blue"); // Show others the answers
                UIManager.photonView.RPC("GenerateTextPopup", RpcTarget.All, "Musterlösung: " + correctAnswer, WindowPosition.Two_Third, WindowSize.One_Third_Height, WindowSize.Width, waitingTime/3, "blue");
                HandleVoteClick.photonView.RPC("StartVoting", RpcTarget.Others, points, GameManager.currentPlayer.photonView.Controller, true);
                StartCoroutine(HandleVoteClick.WaitForVotes(points, GameManager.currentPlayer.photonView.Controller, true));
                Destroy(currentAnswerCanvas.gameObject);
                if (currentQuestionCanvas != null) Destroy(currentQuestionCanvas.gameObject);
            }
        });
    }
    public void quizKnowledgeAction(Card c)
    {
        float readingTime = 2 * ((c.question.Split(' ').Length * 60 / 200) + 3f);
        float writeTime = (c.answer.Split(' ').Length * 60 / 40) + 3f;
        float waitingTime = readingTime + writeTime;
        currentQuestionCanvas = UIManager.GenerateTextPopup("Frage: \n" + c.question, WindowPosition.One_Third, WindowSize.Two_Third_Height, WindowSize.Width, waitingTime, "blue");
        UIManager.photonView.RPC("GenerateTextPopup", RpcTarget.Others, "Frage: \n" + c.question, WindowPosition.One_Third, WindowSize.Two_Third_Height, WindowSize.Width, waitingTime, "blue"); // Show others the answer
        currentAnswerCanvas = UIManager.InputTextPopup("blue"); // Show answer input field
        StartCoroutine(AskForInputQuiz(c.points, c.answer, waitingTime));
    }

    [PunRPC]
    public void patternRecognizeAction(string question, string answer, int points, string note)
    {
        float readingTime = 2 * ((question.Split(' ').Length * 60 / 200) + 15f);

        string randomPattern = Player.cards.patterns[UnityEngine.Random.Range(0, Player.cards.patterns.Count)]; // Get random pattern
        string cardText = "<u><b>Deine Musterkarte: " + randomPattern + "</b></u> \n" + "Frage:" + question + "\n\n" + note;
        
        // Show text and wait with timer
        currentQuestionCanvas = UIManager.GenerateTextPopup(cardText, WindowPosition.One_Third, WindowSize.Two_Third_Height, WindowSize.Width, readingTime, "orange");
        Destroy(GameObject.FindGameObjectWithTag("Timer"));
        UIManager.GenerateTimer(UIManager.waitingTime);
        StartCoroutine(PatternVote(randomPattern, answer, points, readingTime));
    }

    [PunRPC]
    public void rocketAction(string note, string extra) {
        float readingTime = 2 * ((note.Insert(note.Length, extra).Split(' ').Length * 60 / 200) + 3f);
        string stepByStep = "1) Identifiziert das Feld auf dem Digital Canvas und liest den Infotext \n 2) Überlegt euch mögliche Inhalte und deren Relevanz für euer Geschäftsmodell\n 3) Notiert Kernaspekte per Doppelklick auf eurem Digital Canvas";
        Canvas go = UIManager.GenerateDissolvingTextPopup(stepByStep, readingTime, "black");
        Canvas go1 = UIManager.GenerateTextPopup(note + "\n" + extra, WindowPosition.Two_Third, WindowSize.Two_Third_Height, WindowSize.Width, readingTime, "black");
        StartCoroutine(EditDC(readingTime));
    }

    [PunRPC]
    public void creativityAction(string question, int points, string note)
    {
        float readingTime = 4 * ((question.Split(' ').Length * 60 / 200) + 15f);
        //Initialize player responses to question
        playerAnswers = new Dictionary<Photon.Realtime.Player, string>();
        foreach (Photon.Realtime.Player player in PhotonNetwork.PlayerList) playerAnswers.Add(player, "");

        //Show Question and input field
        currentQuestionCanvas = UIManager.GenerateTextPopup("Frage: " + question + "\n" + note, WindowPosition.One_Third, WindowSize.Two_Third_Height, WindowSize.Width, readingTime, "red");
        currentAnswerCanvas = UIManager.InputTextPopup("red"); // Show answer input field
        StartCoroutine(AskForInputCreativity(points, readingTime));
    }
    public void environmentAction(string question, string answer, int points, string note)
    {
        float readingTime = 2 * ((question.Split(' ').Length * 60 / 200) + 3f);
        string questionText = "<b>Frage: </b>" + question + "\n" + note;
        UIManager.photonView.RPC("GenerateTextPopup", RpcTarget.All, questionText, WindowPosition.One_Third, WindowSize.Two_Third_Height, WindowSize.Width, readingTime, "grey");
        photonView.RPC("EnvironmentActionRPC", RpcTarget.Others, answer, points, PhotonNetwork.LocalPlayer, readingTime); // Make others vote after seeing gm
        StartCoroutine(HandleVoteClick.WaitForVotesDelayed(points, PhotonNetwork.LocalPlayer, true)); // Make selected player see votibg
    }

    [PunRPC]
    public void EnvironmentActionRPC(string answer, int points, Photon.Realtime.Player playerTarget, float readingTime){
        StartCoroutine(EnvironmentAction(answer, points, playerTarget, readingTime));
    }

    [PunRPC]
    IEnumerator EnvironmentAction(string answer, int points, Photon.Realtime.Player playerTarget, float readingTime)
    {
        yield return new WaitForSeconds(readingTime); // Wait for question to be read by all
        Canvas gmCanvas = UIManager.GenerateGMView(40f);
        ExitGames.Client.Photon.Hashtable props = playerTarget.CustomProperties;

        // Fill GM text fields with correct player bm
        GameObject gmContent = GameObject.Find("BusinessModelContent");
        for (int i = 0; i < gmContent.transform.childCount; i++)
        {
            TMP_Text textField = gmContent.transform.GetChild(i).GetChild(0).GetComponentInChildren<TMP_Text>();
            if (props.ContainsKey("GM" + i)) textField.text = props["GM" + i].ToString();
        }
        
        // Fill DC with corresponding notes on each field
        GameObject dcContent = GameObject.Find("DCContent");
        for (int i = 1; i < dcContent.transform.childCount + 1; i++) // Iterate over each field
        {
            TMP_Text textField = dcContent.transform.GetChild(i - 1).GetChild(0).GetComponentInChildren<TMP_Text>();
            
            // Only insert notes if they exist
            if (playerTarget.CustomProperties.ContainsKey(i.ToString()))
            {
                IEnumerable<string> notes = playerTarget.CustomProperties[i.ToString()] as IEnumerable<string>;
                string merged = "";
                foreach (string note in notes)
                {
                    merged += note + "\n";
                }
                textField.text = merged;
            }
        }

        yield return new WaitForSeconds(40f); // Wait for other players to check bm and dc
        HandleVoteClick.StartVoting(points, playerTarget, true);
        yield return new WaitForSeconds(UIManager.waitingTime); // Wait for vote to finish
        
        //Subtract Point
        if(HandleVoteClick.votesAccept < HandleVoteClick.votesReject)
        {
            int currentPoints = PointManager.GetPointsCount(playerTarget);
            PointManager.SetPoints(playerTarget, currentPoints - 1);
        }
    }

    [PunRPC]
    public void SetPlayerAnswer(Photon.Realtime.Player player, string answer)
    {
        playerAnswers[player] = answer;
    }
    IEnumerator EditDC(float readingTime)
    {
        Destroy(GameObject.FindGameObjectWithTag("Timer"));
        UIManager.GenerateTimer(readingTime);
        yield return new WaitForSeconds(readingTime); // Wait for text to be read
        GameObject.FindGameObjectWithTag("DCPfeil").GetComponent<ToggleDC>().toggleDC(); // show DC
        NoteSpawning.editingDC = true;
        
        Destroy(GameObject.FindGameObjectWithTag("Timer"));
        UIManager.GenerateTimer(100f);
        yield return new WaitForSeconds(100f); // Wait for notes to be made
        GameObject.FindGameObjectWithTag("DCPfeil").GetComponent<ToggleDC>().toggleDC(); // hide DC
        NoteSpawning.StopEditing();
        GameManager.nextTurn();
    }

    IEnumerator PatternVote(string playerPattern, string answer, int points, float readingTime)
    {
        // Setup voting
        bool voted = false;
        Canvas voteCanvas = UIManager.GenerateVotePopup("orange");
        voteCanvas.GetComponentsInChildren<Button>()[0].onClick.AddListener(() => {
            voted = true;
            if (voteCanvas != null) Destroy(voteCanvas.gameObject); // Destroy buttons if clicked
            if (currentQuestionCanvas != null) Destroy(currentQuestionCanvas.gameObject);
            Destroy(GameObject.FindGameObjectWithTag("Timer")); // Destroy timer if clicked

            int currentPoints = PointManager.GetPointsCount(PhotonNetwork.LocalPlayer);
            if (playerPattern.Equals(answer)) // Correct choice
            {
                PointManager.photonView.RPC("SetPoints", RpcTarget.All, PhotonNetwork.LocalPlayer, currentPoints + 1);
            } else { // Wrong choice
                PointManager.photonView.RPC("SetPoints", RpcTarget.All, PhotonNetwork.LocalPlayer, currentPoints - 1);
            }
            GameManager.nextTurn();
        });
        voteCanvas.GetComponentsInChildren<Button>()[1].onClick.AddListener(() => {
            voted = true;
            if (voteCanvas != null) Destroy(voteCanvas.gameObject);
            if (currentQuestionCanvas != null) Destroy(currentQuestionCanvas.gameObject);
            Destroy(GameObject.FindGameObjectWithTag("Timer"));

            int currentPoints = PointManager.GetPointsCount(PhotonNetwork.LocalPlayer);
            if (!playerPattern.Equals(answer)) // Correct choice
            {
                PointManager.photonView.RPC("SetPoints", RpcTarget.All, PhotonNetwork.LocalPlayer, currentPoints + 1);
            } else { // Wrong choice
                PointManager.photonView.RPC("SetPoints", RpcTarget.All, PhotonNetwork.LocalPlayer, currentPoints - 1);
            }
            GameManager.nextTurn();
        });
        yield return new WaitForSeconds(readingTime); // Zeit zum Voten
        if(!voted) NotVotedOnTime();
    }

    void NotVotedOnTime()
    {
        Destroy(GameObject.FindGameObjectWithTag("Timer"));
        int currentPoints = PointManager.GetPointsCount(PhotonNetwork.LocalPlayer);
        PointManager.photonView.RPC("SetPoints", RpcTarget.All, PhotonNetwork.LocalPlayer, currentPoints - 1);
        GameManager.nextTurn();
    }
}
