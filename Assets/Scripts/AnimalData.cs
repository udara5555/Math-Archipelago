using UnityEngine;

/// <summary>
/// Simple data container for an animal. Not a MonoBehaviour — 
/// used as a serializable field inside SetsGameplay.
/// </summary>
[System.Serializable]
public class AnimalData
{
    public string animalName;   // e.g. "Lion"
    public Sprite icon;         // the sliced sprite from animals.png
    public string category;     // "Mammal", "Bird", or "Fish" (Level 1)

    [Header("Venn Diagram Attributes (Level 2)")]
    public bool haveFur;
    public bool canFly;
    public bool produceMilk;
    public bool haveFins;
}
