using UnityEngine;

public static class UIHelper
{
    public static Vector3 GetUIWorldPosition(Vector3 uiPosition)
    {
        Camera camera = Camera.main;
        if (camera == null)
        {
            Debug.LogError("No main camera found");
            return Vector3.zero;
        }
        
        return camera.ScreenToWorldPoint(new Vector3(uiPosition.x, uiPosition.y, camera.nearClipPlane));
    }
}
