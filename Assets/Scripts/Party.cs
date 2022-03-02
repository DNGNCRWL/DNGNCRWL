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

    public event EventHandler OnPartyListChanged;

    public void UpdatedParty() {
        OnPartyListChanged?.Invoke(this, EventArgs.Empty);
    }
}
