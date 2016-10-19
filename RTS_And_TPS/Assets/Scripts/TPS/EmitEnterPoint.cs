using UnityEngine;
using System.Collections;

public class EmitEnterPoint : MonoBehaviour {

	[SerializeField]
	int startLevel;

	[SerializeField]
	bool[] emitFlag;

	public bool IsActiveEmit(int curLevel)
	{
		return curLevel >= startLevel;
	}

	public int getEmitIndex()
	{
		int count = 0;
		for (int i = 0; i < emitFlag.Length; i++)
		{
			if (emitFlag[i] == true) count++;
		}

		int emit = Random.Range(0, count);

		for (int i = 0; i < emitFlag.Length; i++)
		{
			{
				if (emitFlag[i] == true)
				{
					emit--;
					if (emit < 0)
					{
						return i;
					}
				}
			}
		}
		return -1;
	}
}
