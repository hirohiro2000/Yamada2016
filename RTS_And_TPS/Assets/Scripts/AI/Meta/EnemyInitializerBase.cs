using UnityEngine;
using System.Collections;

public class EnemyInitializerBase : MonoBehaviour {

    public virtual void Execute(Vector3 respawn_pos,
        EnemyWaveParametor param)
    {
        UserLog.Terauchi("EnemyInitializerBase::Execute call");
    }

}
