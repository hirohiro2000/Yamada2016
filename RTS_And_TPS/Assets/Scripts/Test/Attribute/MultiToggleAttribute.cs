using UnityEngine;
using System.Collections;

public class MultiToggleAttribute : PropertyAttribute
{
	public int num;

	public MultiToggleAttribute(int num)
	{
		this.num = num;
	}
}
