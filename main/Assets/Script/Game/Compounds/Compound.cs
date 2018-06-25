using Assets.Script.Core.Math;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Script.Game.Compounds
{
    public class Compound : MonoBehaviour
    {
        public BlockContainer[] m_Blocks;

        Vector3 m_BasePosition;
        float m_BlockSize = 62.9f;
        const int m_SlicePlane = 50;
        const float m_SlicePlaneAngle = 360f / m_SlicePlane;
        Vector2 m_TileSize;
        Vector2 m_HalfTileSize;
        Vector2 m_FarthestDistance;
        Vector2 m_TileMax;

        public Vector3 BasePosition
        {
            get { return m_BasePosition; }
            set { m_BasePosition = value; }
        }

        public void OnEnable()
        {
            m_BasePosition = transform.position;
            Vector2 min = new Vector2(int.MaxValue, int.MaxValue);
            m_TileMax = new Vector2(int.MinValue, int.MinValue);
            foreach (BlockContainer container in m_Blocks)
            {
                container.m_BaseOrientation = container.m_Block.transform.localEulerAngles;

                if (container.m_AxialPosition < min.x)
                    min.x = container.m_AxialPosition;
                if (container.m_AxialPosition > m_TileMax.x)
                    m_TileMax.x = container.m_AxialPosition;

                if (container.m_TangentialPosition < min.y)
                    min.y = container.m_TangentialPosition;
                if (container.m_TangentialPosition > m_TileMax.y)
                    m_TileMax.y = container.m_TangentialPosition;
            }

            m_TileSize = m_TileMax - min + new Vector2(1, 1);
            m_HalfTileSize = m_TileSize / 2;
            m_FarthestDistance = m_HalfTileSize - new Vector2(0.5f, 0.5f);
        }

        private void SetBlockTransform(BlockContainer _container, float _ZAngle)
        {
            //compute the relative position of the tile with respect to the center of the compound.
            Vector2 tilePosition = new Vector2(_container.m_AxialPosition, _container.m_TangentialPosition);
            Vector2 tileDelta = m_TileMax - tilePosition;
            Vector2 relativeTilePosition = m_FarthestDistance - tileDelta;

            //compute orientation so the tile match the ground orientation
            _ZAngle += m_SlicePlaneAngle * Mathf.FloorToInt(relativeTilePosition.y);

            //position along the axis of the station
            Vector3 axialOffset = relativeTilePosition.x * new Vector3(0, 0, m_BlockSize);
            axialOffset += new Vector3(0, 0, m_BasePosition.z);
           
            //orientation of the tile
            float yAngle = _container.m_BaseOrientation.y;

            _container.m_Block.transform.position = Vector3.zero;
            _container.m_Block.transform.eulerAngles = Vector3.zero;

            _container.m_Block.transform.Rotate(Vector3.forward, _ZAngle);
            _container.m_Block.transform.Translate(Vector3.up * -500);
            _container.m_Block.transform.Translate(axialOffset, Space.Self);
            _container.m_Block.transform.Rotate(Vector3.up, yAngle);
        }

        public void SetCompoundTransform()
        {
            Vector3 ideal = m_BasePosition;
            ideal.z = 0;
            ideal.Normalize();
            float zAngle = FD_Math.GetSignedAngle(Vector3.right, ideal, Vector3.forward) + 90;
            float div = zAngle / m_SlicePlaneAngle;
            int iDiv = (int)Math.Round(div);
            zAngle = m_SlicePlaneAngle * iDiv;

            foreach (BlockContainer container in m_Blocks)
                SetBlockTransform(container, zAngle);
        }

        public void ComputeExtend(out float x, out float y)
        {
            x = m_HalfTileSize.x * m_BlockSize;
            y = m_HalfTileSize.y * m_BlockSize;
        }

        public void RotateCompound(float _angleInDegrees)
        {
            float angleInRadians = _angleInDegrees * Mathf.Deg2Rad;
            float cosTheta = Mathf.Cos(angleInRadians);
            float sinTheta = Mathf.Sin(angleInRadians);
            Vector2 col1 = new Vector2(cosTheta, sinTheta);
            Vector2 col2 = new Vector2(-sinTheta, cosTheta);

            Vector2 min = new Vector2(int.MaxValue, int.MaxValue);
            m_TileMax = new Vector2(int.MinValue, int.MinValue);

            foreach (BlockContainer container in m_Blocks)
            {
                Vector2 originalTilePosition = new Vector2(container.m_AxialPosition, container.m_TangentialPosition);
                container.m_AxialPosition = (int)Math.Round(Vector2.Dot(originalTilePosition, col1));
                container.m_TangentialPosition = (int)Math.Round(Vector2.Dot(originalTilePosition, col2));

                if (container.m_AxialPosition < min.x)
                    min.x = container.m_AxialPosition;
                if (container.m_AxialPosition > m_TileMax.x)
                    m_TileMax.x = container.m_AxialPosition;

                if (container.m_TangentialPosition < min.y)
                    min.y = container.m_TangentialPosition;
                if (container.m_TangentialPosition > m_TileMax.y)
                    m_TileMax.y = container.m_TangentialPosition;

                container.m_BaseOrientation.y -= _angleInDegrees;
            }

            m_TileSize = m_TileMax - min + new Vector2(1, 1);
            m_HalfTileSize = m_TileSize / 2;
            m_FarthestDistance = m_HalfTileSize - new Vector2(0.5f, 0.5f);
        }
    }

    [Serializable]
    public class BlockContainer
    {
        public GameObject m_Block;
        public int m_AxialPosition;
        public int m_TangentialPosition;

        [HideInInspector]
        public Vector3 m_BaseOrientation;
    }
}
