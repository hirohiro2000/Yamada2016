using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

[System.Serializable]
public class GenericList<Type>
{

    public List<Type> m_list = new List<Type>();

}

[System.Serializable]
public class GameObjectList : GenericList<GameObject>
{

}
