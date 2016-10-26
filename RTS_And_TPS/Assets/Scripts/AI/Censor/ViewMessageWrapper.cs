using UnityEngine;
using System.Collections;

public class ViewMessageWrapper
{
    public enum MessageType
    {
        Error = -1,
        InVisibilityRange,       //視界判定内に入った
        InNearRange,            //近距離まで来た
        OutVisibilityRange,   //視界判定外から出た
        OutNearRange,         //近距離から外れた
    }

    public MessageType                      message_type;
    public string                                  sender_tag;
    public GameObject                        sender_object;
    public VisibilityChecker                   visibility_checker;
    public bool                                   is_static_object;     //動き回るオブジェクトかどうか
}
