using UnityEngine;
using System.Collections;

public class ToSoundPlayControl_SetRandom : MonoBehaviour {

    public  SoundPlay_Control[] c_rTarget       =   null;

    public  Vector2             c_RandomVolume  =   Vector2.one;
    public  Vector2             c_RandomPitch   =   Vector2.one;

	void    Awake()
    {
        if( c_rTarget == null )     return;
        if( c_rTarget.Length == 0 ) return;

        float   volumeRatio =   Random.Range( c_RandomVolume.x, c_RandomVolume.y );
        float   pitchRatio  =   Random.Range( c_RandomPitch.x,  c_RandomPitch.y  );
        for( int i = 0; i < c_rTarget.Length; i++ ){
            c_rTarget[ i ].c_VolumeRatio    =   volumeRatio;
            c_rTarget[ i ].c_PitchRatio     =   pitchRatio;
            c_rTarget[ i ].c_LifeTime       =   c_rTarget[ i ].c_LifeTime / c_rTarget[ i ].c_PitchRatio;
        }
    }
}
