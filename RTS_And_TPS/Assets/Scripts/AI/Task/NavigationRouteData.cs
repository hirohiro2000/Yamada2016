using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

[System.SerializableAttribute]
public class StringList
{
    public List<string> data = new List<string>(); 
}
    
/**
*@敵の通ることのできるルートデータを格納する
*/
public class NavigationRouteData : MonoBehaviour
{

    [SerializeField]
    private List<StringList> m_route_data = new List<StringList>();

    
    public StringList GetRouteData(int route_index)
    {
        if(route_index < 0 || route_index >= m_route_data.Count)
        {
            UserLog.Terauchi("NavigationRouteData::GetRouteData route_index out of range value is" + route_index);
            return null;
        }
        return m_route_data[route_index];
    }
}
