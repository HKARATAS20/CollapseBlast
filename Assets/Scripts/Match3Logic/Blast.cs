using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public enum BlastType
{
    invalid,
    aIcon,
    bIcon,
    cIcon
}
public class Blast
{
    private int unlisted;

    private List<Blastable> blastables;
    public List<Blastable> Blastables
    {
        get { return blastables; }
    }
    public int Count
    {
        get { return blastables.Count + unlisted; }
    }

    public bool Contains(Blastable toCompare)
    {
        return blastables.Contains(toCompare);
    }

    public Blast()
    {
        blastables = new List<Blastable>(10);
    }

    public Blast(Blastable original) : this()
    {
        AddBlastable(original);
    }
    public void AddBlastable(Blastable toAdd)
    {
        blastables.Add(toAdd);
    }

    public void AddUnlisted()
    {
        unlisted++;
    }
    public void Merge(Blast toMerge)
    {
        blastables.AddRange(toMerge.blastables);
    }
}
