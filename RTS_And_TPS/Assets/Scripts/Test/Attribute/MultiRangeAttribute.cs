using UnityEngine;
using System.Collections;

public class MultiRangeAttribute : PropertyAttribute
{
	public float min;
	public float max;
	public int num;

	public MultiRangeAttribute(float min, float max ,int num)
	{
		this.min = min;
		this.max = max;
		this.num = num;
	}
}
