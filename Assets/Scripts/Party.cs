using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Party
{
    public string name;
    public List<CharacterSheet> characters;
    public Transform[] positions;
    public bool playerControlled;
    private int partyLimit = 4;

    public event EventHandler OnPartyListChanged;

    

    public void SetParty(List<CharacterSheet> characters) {
        this.characters = characters;
        OnPartyListChanged?.Invoke(this, EventArgs.Empty);
    }
    public void UpdatedParty() {
        OnPartyListChanged?.Invoke(this, EventArgs.Empty);
    }

    public void SetName(string partyName)
    {
        this.name = partyName;
    }
    public string GetName() { return name; }

    public void ChangePartyLimit(int partyLimitIn) {
        if (characters.Count > partyLimitIn)
            Debug.LogError("Error: Tried to set Party limit below existing character count");
        else
            this.partyLimit = partyLimitIn;
    }

    public void AddCharacter(CharacterSheet newChar) {
        if (characters.Count >= partyLimit) {
            Debug.LogError("Error: Tried Adding Character when Party is Full");
        } else {
            characters.Add(newChar);
            OnPartyListChanged?.Invoke(this, EventArgs.Empty);
        }
    }

    public void RemoveCharacter(CharacterSheet remChar) {
        characters.Remove(remChar);
        OnPartyListChanged?.Invoke(this, EventArgs.Empty);
    }

    

}
