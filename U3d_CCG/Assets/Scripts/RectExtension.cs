using UnityEngine;

public static class RectExtension
{
    public static Rect WorldSpaceRect(this RectTransform rt)
    {
        var r = rt.rect;
        r.center = rt.TransformPoint(r.center);
        r.size = rt.TransformVector(r.size);
        return r;
    }
}