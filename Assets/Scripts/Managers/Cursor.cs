using System.Collections;
using UnityEngine;


[RequireComponent(typeof(SpriteRenderer))]
public class Cursor : Singleton<Cursor>
{
    //  activate the ability to edit the grid

    private MatchableGrid grid;


    private Blastable[] selected;

    protected override void Init()
    {
        selected = new Blastable[2];
    }
    private void Start()
    {
        grid = (MatchableGrid)MatchableGrid.Instance;
    }

    public void ItemTapped(Blastable tapped)
    {
        StartCoroutine(grid.TryBlast(tapped));
    }

    public void SelectFirst(Blastable toSelect)
    {
        selected[0] = toSelect;
    }


}