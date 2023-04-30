using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mouse3D
{
    public static Mouse3D Instance = new Mouse3D();
    private int mouseColliderLayerMask = 1 << LayerMask.NameToLayer("Grid"); // Makes sure it collider with the box collider on the Grid layer

    private Vector3 GetMouseWorldPosition_Instance() //Converts mouse screen position to a ray and the first thing it hits is the Vector3 that is returned
    {
        Vector3 point = Input.mousePosition;
        Ray ray = Camera.main.ScreenPointToRay(point);
        if (Physics.Raycast(ray, out RaycastHit raycastHit, 100f, mouseColliderLayerMask))
        {
            return raycastHit.point;
        }
        else
            return Vector2.zero;
    }

    public static Vector3 GetMouseWorldPosition() => Instance.GetMouseWorldPosition_Instance();
}
