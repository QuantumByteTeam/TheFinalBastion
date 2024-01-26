using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ISimpleInteractable 
{
    public string InteractionPrompt { get; }

    public void SimpleInteract(SimpleInteractor Simpleinteractor);


}
