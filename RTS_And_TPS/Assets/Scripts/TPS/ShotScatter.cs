using UnityEngine;
using System.Collections;

//出現と同時に発射(親付け)
public class ShotScatter : MonoBehaviour {


	[SerializeField]
	int shotNum = 8;

	[SerializeField]
	float scatterRadian = 1.0f;

	[SerializeField]
	Transform bullet = null;

	//発射
	void    Start()
	{
        TPSAttack_Net   rMyTPSAttack    =   GetComponent< TPSAttack_Net >();

		for (int i = 0; i < shotNum; i++)
		{

			Vector3 forward = transform.forward;

			//ベクトルに変換
			int loop = 3;
			Vector2 scatterRadianVector2 = Vector2.zero;
			for (int k = 0; k < loop; k++)
			{
				scatterRadianVector2.x += Random.Range(-scatterRadian, scatterRadian);
				scatterRadianVector2.y += Random.Range(-scatterRadian, scatterRadian);
			}
			scatterRadianVector2 /= (float)loop;

			Vector3 scatterRadianVector = Quaternion.Euler(scatterRadianVector2.x, scatterRadianVector2.y, .0f) * Vector3.forward;

			forward = Quaternion.LookRotation(forward) * scatterRadianVector;

            //  生成
			Transform       rObj        =   Instantiate(bullet, transform.position, Quaternion.LookRotation(forward)) as Transform;

            //  攻撃者を設定 
            TPSAttack_Net   rTPSAttack  =   rObj.GetComponent< TPSAttack_Net >();
            rTPSAttack.c_AttackerID     =   rMyTPSAttack.c_AttackerID;
		}
	}
}
