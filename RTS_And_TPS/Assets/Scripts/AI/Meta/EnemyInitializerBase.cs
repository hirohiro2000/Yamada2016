using UnityEngine;
using System.Collections;

public class EnemyInitializerBase : MonoBehaviour {

    public virtual void Execute(Vector3 respawn_pos,
        StringList route_list,
        int level,
        float HPCorrectionRate)
    {
        UserLog.Terauchi("EnemyInitializerBase::Execute call");
    }

}
