using UnityEngine;
using System.Collections;

public class FieldResources : MonoBehaviour
{
    [SerializeField]
    private RTSCamera       m_rtsCamera     = null;

    void Update()
    {
        if ( m_rtsCamera.m_target == null ) return;
        
        Vector3     pos         = m_rtsCamera.transform.position;
        Vector3     target      = m_rtsCamera.m_target.position;
                    target.y   += 1.5f;

        Vector3     dir         = ( target - pos );
        RaycastHit  rHit        = new RaycastHit();
        Ray         rRay        = new Ray( pos, dir.normalized );
        float       maxDist     = dir.magnitude;
        int         layerMask   = LayerMask.GetMask( "Field" );
        
        //  レイ判定
        UpdateVisibility( !Physics.Raycast( rRay, out rHit, maxDist, layerMask ) );

    }
    void UpdateVisibility( bool canSee )
    {
                    
        int numChild = transform.childCount;
        for (int i = 0; i < numChild; i++)
        {
            Transform child = transform.GetChild(i);
            
            Vector3 girl    = m_rtsCamera.m_target.position;
            Vector3 tower   = child.transform.position;
            float   range   = 1.2f;
            bool    inTo    = ( ( girl - tower ).sqrMagnitude <= range*range );
            
            child.GetComponent<MaterialSwitchToConvert>().SetVisibility( canSee );

        }

    }

}
