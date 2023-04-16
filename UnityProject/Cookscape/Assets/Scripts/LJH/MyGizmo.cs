using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MyGizmo : MonoBehaviour
{
    public Color _color = Color.yellow;
    public float _radius = 0.3f;

    private void OnDrawGizmos()
    {
        // Set Gizmo color
        Gizmos.color = _color;

        // sphere
        Gizmos.DrawSphere(transform.position, _radius);
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
