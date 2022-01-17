using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "Action", menuName = "Action", order = 1)]
public class CharacterAction : ScriptableObject
{
    /*
    - Name
- possible roll
- list of targets
- Action descriptor + timing
- Midway descriptor + timing
- Result descriptor + timing
- List of results on success, fail, critical, fumble
*/
    public string actionName;
    public string verb;
    public List<ParameteredAtomicFunction> alwaysDoThese;
    [HideInInspector]
    public bool useRoll;
    [Header("If Rolling...")]
    public int difficultyRating;
    d20 roll;
    public StatSelector chooseActorStatToTest;
    public StatSelector chooseTargetStatToTest;
    [HideInInspector]
    public bool needsTarget;

    Func<ParameteredAtomicFunction, String> startDescriptor;
    Func<ParameteredAtomicFunction, String> actionDescriptor;
    Func<ParameteredAtomicFunction, String> resultDescriptor;
    public List<ParameteredAtomicFunction> success;
    public List<ParameteredAtomicFunction> critical;
    public List<ParameteredAtomicFunction> failure;
    public List<ParameteredAtomicFunction> fumble;

    private void Awake()
    {
        needsTarget = NeedsTarget();
        useRoll = (success.Count + critical.Count + failure.Count + fumble.Count) > 0;
    }

    bool NeedsTarget()
    {
        return (
           NeedsTarget(success) ||
           NeedsTarget(critical) ||
           NeedsTarget(failure) ||
           NeedsTarget(fumble)
           );
    }

    bool NeedsTarget(List<ParameteredAtomicFunction> list)
    {
        foreach (ParameteredAtomicFunction paf in list)
        {
            switch (paf.atomicFunction)
            {
                case EnumeratedAtomicFunction.TargetAgility:
                case EnumeratedAtomicFunction.TargetBleeding:
                case EnumeratedAtomicFunction.TargetBlind:
                case EnumeratedAtomicFunction.TargetBludgeonResist:
                case EnumeratedAtomicFunction.TargetCutResist:
                case EnumeratedAtomicFunction.TargetDistracted:
                case EnumeratedAtomicFunction.TargetElectricResist:
                case EnumeratedAtomicFunction.TargetEquip:
                case EnumeratedAtomicFunction.TargetFireResist:
                case EnumeratedAtomicFunction.TargetHands:
                case EnumeratedAtomicFunction.TargetHP:
                case EnumeratedAtomicFunction.TargetInfection:
                case EnumeratedAtomicFunction.TargetLegs:
                case EnumeratedAtomicFunction.TargetMagicResist:
                case EnumeratedAtomicFunction.TargetMaxHP:
                case EnumeratedAtomicFunction.TargetPierceResist:
                case EnumeratedAtomicFunction.TargetPresence:
                case EnumeratedAtomicFunction.TargetStrength:
                case EnumeratedAtomicFunction.TargetToughness:
                    return true;
            }
        }

        return false;
    }
}