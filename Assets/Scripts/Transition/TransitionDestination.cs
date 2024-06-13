using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TransitionDestination : MonoBehaviour
{
    public enum DestinationTag//枚举变量：传送的DesinationPoint标签
    {
        ENTER, A, B, C
    }
    public DestinationTag destinationTag;

}