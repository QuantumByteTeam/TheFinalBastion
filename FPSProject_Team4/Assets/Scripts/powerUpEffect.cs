using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class powerUpEffect : ScriptableObject //abstract class to make Power Ups with
{
    public abstract void Apply(GameObject target); //target being who power effects, so player
}
