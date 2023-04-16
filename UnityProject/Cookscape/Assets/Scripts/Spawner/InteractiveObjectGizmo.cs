using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractiveObjectGizmo : MonoBehaviour
{
    public Color _color;
    public string _type;
    public float _radius = 1.0f;

    private void OnDrawGizmos()
    {
        if (_type == null) {
            _color = Color.red;
        } else if (_type == "Pot") {
            _color = Color.cyan;
        } else if (_type == "Valve") {
            _color = Color.green;
        } else {
            _color = Color.yellow;
        }
        // Set Gizmo color
        Gizmos.color = _color;
        // sphere
        Gizmos.DrawSphere(transform.position, _radius);
    }
}
