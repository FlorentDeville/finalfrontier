using UnityEngine;

namespace Assets.Script.Core.DebugDrawer
{
    public class FD_DebugDrawer
    {
        public static void DrawFrame(Matrix4x4 _mat, float _size)
        {
            DrawFrame(_mat.GetColumn(3), _mat.GetColumn(0), _mat.GetColumn(1), _mat.GetColumn(2), _size);
        }

        public static void DrawFrame(Vector3 _pos, Vector3 _right, Vector3 _up, Vector3 _forward, float _size)
        {
            Debug.DrawLine(_pos, _pos + _right * _size, Color.red);
            Debug.DrawLine(_pos, _pos + _up * _size, Color.green);
            Debug.DrawLine(_pos, _pos + _forward * _size, Color.blue);
        }
    }
}
