using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotationNew : MonoBehaviour
{
    private enum AxisRotate { X, Y, Z };

    [SerializeField] private int _vitezaRotatie = 100;
    [SerializeField] private bool _rotatieSensAcelorCeasornice = true;
    [SerializeField] private AxisRotate _axaRotatie;

    private int _rotationAngles = 0;

    void FixedUpdate()
    {
        if (_rotatieSensAcelorCeasornice)
            _rotationAngles += _vitezaRotatie;
        else
            _rotationAngles -= _vitezaRotatie;

        switch (_axaRotatie)
        {
            case AxisRotate.X:
                transform.eulerAngles = new Vector3(_rotationAngles * Time.deltaTime, 0, 0);
                break;
            case AxisRotate.Y:
                transform.eulerAngles = new Vector3(0, _rotationAngles * Time.deltaTime, 0);
                break;
            case AxisRotate.Z:
                transform.eulerAngles = new Vector3(0, 0, _rotationAngles * Time.deltaTime);
                break;
            default:
                break;
        }
    }
}
