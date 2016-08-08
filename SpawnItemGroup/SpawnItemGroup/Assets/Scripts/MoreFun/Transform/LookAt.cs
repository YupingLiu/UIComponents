using UnityEngine;
using System.Collections;
using MoreFun;

namespace MoreFun
{
    public class LookAt : MoreBehaviour
    {
        public Transform target;
        public Vector3 lookAtOffset;
        public Vector3 upwards = Vector3.up;
        public Transform source;
        
        void Awake()
        {
            if (null == source)
            {
                source = transform;
            }
        }
        
        void Update()
        {
            if(null != target)
            {
                Quaternion result = Quaternion.LookRotation(
                    target.position - source.position,
                    upwards);

                result = result * Quaternion.Euler(new Vector3(0.0f, lookAtOffset.y, 0.0f));
                result = result * Quaternion.Euler(new Vector3(lookAtOffset.x, 0.0f, lookAtOffset.z));

                source.rotation = result;
            }
        }
    }

}