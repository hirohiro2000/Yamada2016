﻿using UnityEngine;
using System.Collections;

public class ViewMessageWrapper
{
    public enum MessageType
    {
        InVisibilityRange,       //視界判定内に入った
        InNearRange,            //近距離まで来た
        OutVisibilityRange,   //視界判定外から出た
        OutNearRange,         //近距離から外れた
    }

    public MessageType                                 message_type;
    public string                                   sender_tag;
    public GameObject                         sender_object;
}
