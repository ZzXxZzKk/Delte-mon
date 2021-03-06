﻿using System.Collections;
using System.Collections.Generic;
using BattleDelts.UI;
using UnityEngine;
using UnityEngine.UI;

namespace BattleDelts
{
    public class NameChanger : MonoBehaviour
    {

        public GameObject priceScreen;
        public GameObject NameChangeScreen;
        public Transform PosseView;
        public GameObject nameChangeOverview;

        UIManager UIMan;
        bool hasTriggered;
        int overviewIndex;
        string newName;

        // Initialize variables
        void Start()
        {
            hasTriggered = false;
            UIMan = UIManager.Inst;
            newName = "";
        }

        // Player steps into sight of Name Changer
        void OnTriggerEnter2D(Collider2D player)
        {
            if (!hasTriggered)
            {
                hasTriggered = true;

                PlayerMovement.Inst.StopMoving();

                UIMan.StartNPCMessage("I am the name changer.", "Name Changer");

                if (GameManager.Inst.coins < 50)
                {
                    UIMan.StartNPCMessage("You are too poor to change a name.");
                    UIMan.StartNPCMessage("Leave this place, mortal.");
                    PlayerMovement.Inst.ResumeMoving();
                }
                else
                {
                    UIMan.StartNPCMessage("To continue you must first pay the price.");
                    UIMan.StartMessage(null, null, () => priceScreen.SetActive(true));
                }
            }
        }

        // Player presses continue button
        public void continueButtonPress()
        {

            GameManager.Inst.coins -= 50;
            SoundEffectManager.Inst.PlaySoundImmediate("coinDing");

            // Set all buttons in Posse view
            for (int i = 0; i < 6; i++)
            {
                Transform view = PosseView.GetChild(i);
                if (i < GameManager.Inst.deltPosse.Count)
                {
                    view.GetChild(0).GetComponent<Text>().text = GameManager.Inst.deltPosse[i].nickname;
                    view.GetChild(1).GetComponent<Image>().sprite = GameManager.Inst.deltPosse[i].deltdex.frontImage;

                    view.gameObject.SetActive(true);
                }
                else
                {
                    view.gameObject.SetActive(false);
                }
            }
            priceScreen.SetActive(false);
            NameChangeScreen.SetActive(true);
            nameChangeOverview.SetActive(false);
        }

        // User selects one of their posse to rename
        public void selectDelt(int index)
        {
            if (!nameChangeOverview.activeInHierarchy)
            {
                nameChangeOverview.SetActive(true);
            }
            // Set overview image
            nameChangeOverview.transform.GetChild(0).GetComponent<Image>().sprite = GameManager.Inst.deltPosse[index].deltdex.frontImage;
            overviewIndex = index;
        }

        // User clicks the change name button after entering a new name
        public void changeName()
        {
            if (newName != "")
            {
                GameManager.Inst.deltPosse[overviewIndex].nickname = newName;
                NameChangeScreen.SetActive(false);
                DeltemonClass changed = GameManager.Inst.deltPosse[overviewIndex];
                UIMan.StartNPCMessage("Your " + changed.deltdex.nickname + "'s name is now " + newName, "Name Changer");
                PlayerMovement.Inst.ResumeMoving();
            }
            else
            {
                StartCoroutine(flashInput(nameChangeOverview.transform.GetChild(1).GetComponent<Image>()));
            }
        }

        // When user finishes editing a new name string
        public void setNewNameString(string input)
        {
            newName = input;
        }

        // Flash red on text input if there is no name
        IEnumerator flashInput(Image inputBackground)
        {
            for (int i = 0; i < 3; i++)
            {
                inputBackground.color = Color.red;
                yield return new WaitForSeconds(0.25f);
                inputBackground.color = Color.white;
                yield return new WaitForSeconds(0.25f);
            }
        }

        // User decides not to change a Delt's name
        public void backButtonPress()
        {
            priceScreen.SetActive(false);
            NameChangeScreen.SetActive(false);
            UIMan.StartNPCMessage("Leave this place, human.", "Name Changer");
            UIMan.StartMessage(null, null, () => UIMan.EndNPCMessage());
            PlayerMovement.Inst.ResumeMoving();
        }

        void OnTriggerExit2D(Collider2D player)
        {
            hasTriggered = false;
            UIMan.EndNPCMessage();
        }
    }
}