using System;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;
using Syrus.Plugins.DFV2Client;
using UnityEngine.UI;
using System.Collections;
using TMPro;

public class DF2Client : MonoBehaviour
{
	public TMP_InputField  content;

	private DialogFlowV2Client client;

	private string languageCode = "de";
	public event Action<String> UserTextEvent;
	public event Action<String> AITextEvent;

	public int cheertimeInsec = 90;
	public List<string> CheerList = new List<string> { "cheer","cheer user"};
	// Start is called before the first frame update
	private void Start()
	{
		client = GetComponent<DialogFlowV2Client>();

		// Ändern Sie den Event-Handler, um die Coroutine zu starten
		client.ChatbotResponded += response => StartCoroutine(LogResponseTextCoroutine(response));
		client.DetectIntentError += LogError;

		StartCoroutine(CheerUser());
	}

	private void Update()
	{
		if(Input.GetKeyDown(KeyCode.Return))
		{
			if(content.text!="")
			{
				SendText();
			}
		}
	}

	IEnumerator CheerUser()
	{
		yield return new WaitForSeconds(cheertimeInsec);

		FindObjectOfType<MessageUI_Setting>().ChatBox.SetActive(true);
		SendTextToChatbot(CheerList[UnityEngine.Random.Range(0, CheerList.Count)]);
		StartCoroutine(CheerUser());	
    }

	private IEnumerator LogResponseTextCoroutine(DF2Response response)
	{
		Debug.Log(JsonConvert.SerializeObject(response, Formatting.Indented));
		Debug.Log(GetSessionName() + " said: \"" + response.queryResult.fulfillmentText + "\"");
		Debug.Log("Audio " + response.OutputAudio);
		string abc = response.queryResult.fulfillmentText;
		string[] myStringSplit = abc.Split(". ");
		if (myStringSplit.Length > 1)
		{
			
			for (int i = 0; i < myStringSplit.Length; i++)
			{
				if (myStringSplit[i] != "")
				{
					Debug.Log("STRINGSPLIT 0" + myStringSplit[0]);

					Debug.Log("RESPONSE FULL --> " + response.queryResult.fulfillmentText);
					float randomWaitTime = UnityEngine.Random.Range(2.0f, 3.0f);
					Debug.Log("RANDOMWAITTIME" + randomWaitTime);
					yield return new WaitForSeconds(randomWaitTime); // Verzögert die Ausführung um 1 Sekunde
					AITextEvent?.Invoke(myStringSplit[i]);
					// Generiert eine zufällige Wartezeit zwischen 3 und 6 Sekunden
					
				}
			}
		}
		else
		{
			Debug.Log("RESPONSE FULL --> " + response.queryResult.fulfillmentText);
			// Generiert eine zufällige Wartezeit zwischen 3 und 6 
			float randomWaitTime = UnityEngine.Random.Range(2.0f, 3.0f);
			Debug.Log("RANDOMWAITTIME 2222" + randomWaitTime);
			yield return new WaitForSeconds(randomWaitTime); // Verzögert die Ausführung um 1 Sekunde
			AITextEvent?.Invoke(myStringSplit[0]);
			Debug.Log("STRINGSPLIT 0" + myStringSplit[0]);


		}
	}


	private void LogError(DF2ErrorResponse errorResponse)
	{
		
		Debug.LogError(string.Format("Error {0}: {1}", errorResponse.error.code.ToString(),
			errorResponse.error.message));
	}
	public void MyTurn()
    {
			FindObjectOfType<MessageUI_Setting>().ChatBox.SetActive(true);
			if (FindObjectOfType<MessageUI_Setting>().ChatBox.activeSelf)
			{
				string sessionName = GetSessionName();
				client.DetectIntentFromText("Du musst wuerfeln", sessionName, languageCode);
		}
			
	}

	public void Introduction()
	{
		FindObjectOfType<MessageUI_Setting>().ChatBox.SetActive(true);
		if (FindObjectOfType<MessageUI_Setting>().ChatBox.activeSelf)
		{
			string sessionName = GetSessionName();
			client.DetectIntentFromText("introduction", sessionName, languageCode);
		}
	}

	public void almostReachedEnd()
	{
		FindObjectOfType<MessageUI_Setting>().ChatBox.SetActive(true);
		if (FindObjectOfType<MessageUI_Setting>().ChatBox.activeSelf)
		{
			string sessionName = GetSessionName();
			client.DetectIntentFromText("almostReachedEnd", sessionName, languageCode);
		}
	}

	public void FavoriteCard()
	{
		FindObjectOfType<MessageUI_Setting>().ChatBox.SetActive(true);
		if (FindObjectOfType<MessageUI_Setting>().ChatBox.activeSelf)
		{
			string sessionName = GetSessionName();
			client.DetectIntentFromText("favoriteCard", sessionName, languageCode);
		}
	}


	public void SendText()
	{
		if(content.text!="")
		{
            SendTextToChatbot(content.text);
            UserTextEvent?.Invoke(content.text);
            content.text = "";
        }
		
	}
	public void SendTextToChatbot(string context)
    {
		if (context!="")
		{
			string sessionName = GetSessionName();
			client.DetectIntentFromText(context, sessionName, languageCode);
		}
	}

	public void SendEvent()
	{
		client.DetectIntentFromEvent(content.text,
			new Dictionary<string, object>(), GetSessionName());
	}

	public void Clear()
	{
		client.ClearSession(GetSessionName());
	}


	private string GetSessionName(string defaultFallback = "DefaultSession")
	{
		string sessionName="";
		if (sessionName.Trim().Length == 0)
			sessionName = defaultFallback;
		Debug.Log(sessionName);
		return sessionName;
	}

	public void RocketField(int number)
	{
		FindObjectOfType<MessageUI_Setting>().ChatBox.SetActive(true);
		if (FindObjectOfType<MessageUI_Setting>().ChatBox.activeSelf)
		{
			string sessionName = GetSessionName();
			if (number == 0)
			{
				client.DetectIntentFromText("firstRocketField", sessionName, languageCode);
			}
			else if (number == 1)
			{
				client.DetectIntentFromText("secondRocketField", sessionName, languageCode);
			}
			else if (number == 2)
			{
				client.DetectIntentFromText("thirdRocketField", sessionName, languageCode);
			}
			else if (number == 3)
			{
				client.DetectIntentFromText("fourthRocketField", sessionName, languageCode);
			}
			else if (number == 4)
			{
				client.DetectIntentFromText("fifthRocketField", sessionName, languageCode);
			}
			else if (number == 5)
			{
				client.DetectIntentFromText("sixthRocketField", sessionName, languageCode);
			}
			else if (number == 6)
			{
				client.DetectIntentFromText("seventhRocketField", sessionName, languageCode);
			}
			else if (number == 7)
			{
				client.DetectIntentFromText("eighthRocketField", sessionName, languageCode);
			}
			else 
			{
				client.DetectIntentFromText("ninthRocketField", sessionName, languageCode);
			}
		}
	}


}