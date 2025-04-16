using System;
using System.Collections.Generic;
using UnityEngine;
public class SetupCards : MonoBehaviour
{
    public List<Card> quiz_knowledge = new List<Card>(), pattern_recognize = new List<Card>(), environment = new List<Card>(), creativity = new List<Card>(), rocket = new List<Card>();
    public List<string> patterns = new List<string>();
    Cards allCards;
    private void Start()
    {
        string CardsJSON = Resources.Load<TextAsset>("GameBoard/Cards").text;
        allCards = JsonUtility.FromJson<Cards>(CardsJSON);

        CreateCardsInList(allCards.Quiz_Wissen_Karten, ref quiz_knowledge);
        CreateCardsInList(allCards.Muster_Erkennen_Karten, ref pattern_recognize);
        CreateCardsInList(allCards.Umwelt_Karten, ref environment);
        CreateCardsInList(allCards.Kreativ_Karten, ref creativity);
        CreateCardsInList(allCards.Raketen_Karten, ref rocket);
        patterns = new List<string>(allCards.Muster_Karten);
    }

    static void CreateCardsInList(CardInformation[] cards, ref List<Card> ListToInsertIn)
    {
        foreach (CardInformation card in cards)
        {
            if (card != null && card.Frage.Length > 0) ListToInsertIn.Add(new Card(card.Frage, card.Anwort, card.Notiz ?? "", card.Punkte ?? "0", card.Extra ?? ""));
        }
    }

    [Serializable]
    public class Cards
    {
        public CardInformation[] Quiz_Wissen_Karten;
        public CardInformation[] Muster_Erkennen_Karten;
        public CardInformation[] Umwelt_Karten;
        public CardInformation[] Kreativ_Karten;
        public CardInformation[] Raketen_Karten;
        public string[] Muster_Karten;
    }

    [Serializable]
    public class CardInformation
    {
        public string Frage;
        public string Anwort;
        public string Punkte;

        public string Notiz;
        public string Extra;
    }
}