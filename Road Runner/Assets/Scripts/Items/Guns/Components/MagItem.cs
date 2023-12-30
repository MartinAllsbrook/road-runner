using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MagItem : UseableItem
{
    private Magazine magazine;
    public Magazine Magazine
    {
        get { return magazine; }
        private set { magazine = value; }
    }

    protected void Awake()
    {
        magazine = new Magazine(10);
        magazine.ConsumeRound();
        magazine.ConsumeRound();
        magazine.ConsumeRound();
        magazine.ConsumeRound();
    }

    public void TryAddRound()
    {
        magazine.TryAddRound();
    }
}
