using UnityEngine;
using System;
using MyMaterials.Scripts.Element;

namespace MyMaterials.Scripts.Entity.Environment
{
    public class IceWall : MonoBehaviour, IElementReactable
    {
        public ElementType ElementType => ElementType.Ice;

        public bool ReactTo(ElementType incomingType, Vector3 hitPoint, Vector3 hitDirection)
        {
            if (incomingType == ElementType.Fire)
            {
                Debug.Log("IceWallが溶けた");
                return true;
            }

            return false;
        }
    }
}