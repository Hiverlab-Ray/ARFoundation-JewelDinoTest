using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UnityEngine.XR.ARFoundation.Samples
{
    public class Rotator : MonoBehaviour
    {
        float m_Angle;

        void Update()
        {
            m_Angle += Time.deltaTime * 10f;
            transform.rotation = Quaternion.Euler(m_Angle, m_Angle, m_Angle);
        }
    }
}