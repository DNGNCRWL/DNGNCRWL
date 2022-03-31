using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct PlayerInfo {
    private int numHealthPots;
    private int highestWeaponLvl;
    private int lowestWeaponLvl;
    private int ammo;
    private int maxLvl;
    private int minLvl;

    public PlayerInfo(bool doesNothing) {
        numHealthPots = 0;
        highestWeaponLvl = 0;
        lowestWeaponLvl = 0;
        ammo = 0;
        maxLvl = 0;
        minLvl = 0;
    }

    public int getNumHealthPots() { return numHealthPots; }
    public int getHighestWeaponLvl() { return highestWeaponLvl; }
    public int getLowestWeaponLvl() { return lowestWeaponLvl; }
    public int getAmmo() { return ammo; }
    public int getMaxLvl() { return maxLvl; }
    public int getMinLvl() { return minLvl; }

    public void setNumHealthPots(int n) { numHealthPots = n; }
    public void setHighestWeaponLvl(int n) { highestWeaponLvl = n; }
    public void setLowestWeaponLvl(int n) { lowestWeaponLvl = n; }
    public void setAmmo(int n) { ammo = n; }
    public void setMaxLvl(int n) { maxLvl = n; }
    public void setMinLvl(int n) { minLvl = n; }
}
