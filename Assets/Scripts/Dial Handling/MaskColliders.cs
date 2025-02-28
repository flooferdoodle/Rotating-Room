using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MaskColliders : MonoBehaviour
{
    // Parent object to all the objects in the given room
    public Transform ObjectsParent;

    public DimensionButton.DimensionState _state;
    public void SetState(DimensionButton.DimensionState newState) { _state = newState; }

    class QuadPoly
    {
        // Clockwise oriented points
        public Vector2[] pts;

        public QuadPoly(Vector2 a, Vector2 b, Vector2 c, Vector2 d)
        {
            pts = new Vector2[4] { a, b, c, d };
        }
        public QuadPoly(Vector2[] points)
        {
            pts = new Vector2[4];
            for(int i = 0; i < Mathf.Min(points.Length, 4); i++)
            {
                pts[i] = points[i];
            }
        }
        public QuadPoly(List<Vector2> points)
        {
            pts = new Vector2[4];
            for (int i = 0; i < Mathf.Min(points.Count, 4); i++)
            {
                pts[i] = points[i];
            }
        }

        public Vector2[] ConvertToLocalPoly(Matrix4x4 worldToLocal)
        {
            return new Vector2[]
            {
                (Vector2)(worldToLocal.MultiplyPoint3x4(pts[0])),
                (Vector2)(worldToLocal.MultiplyPoint3x4(pts[1])),
                (Vector2)(worldToLocal.MultiplyPoint3x4(pts[2])),
                (Vector2)(worldToLocal.MultiplyPoint3x4(pts[3]))
            };
        }

        public Vector2 GetCenter()
        {
            return (pts[0] + pts[2]) * 0.5f;
        }

        public void Debug_DrawQuadPoly(Color c)
        {
            Debug.DrawLine(pts[0], pts[1], c);
            Debug.DrawLine(pts[1], pts[2], c);
            Debug.DrawLine(pts[2], pts[3], c);
            Debug.DrawLine(pts[3], pts[0], c);
        }

    }

    public Vector2[] ConvertToLocalPoly(Vector2[] poly, Matrix4x4 worldToLocal)
    {
        Vector2[] output = new Vector2[poly.Length];
        for(int i = 0; i < output.Length; i++)
        {
            output[i] = (Vector2)(worldToLocal.MultiplyPoint3x4(poly[i]));
        }
        return output;
    }

    private List<Transform> _objects;
    private List<QuadPoly> _objectBoxes;
    private List<PolygonCollider2D> _objectPolyColls;
    private float _angle;
    public void SetAngle(float theta) { _angle = theta; }

    QuadPoly GetBoxCorners(BoxCollider2D box)
    {
        Bounds bounds = box.bounds;

        return new QuadPoly
        (
            new Vector2(bounds.min.x, bounds.max.y), // Top-left
            new Vector2(bounds.max.x, bounds.max.y), // Top-right
            new Vector2(bounds.max.x, bounds.min.y), // Bottom-right
            new Vector2(bounds.min.x, bounds.min.y)  // Bottom-left
        );
    }

    // Start is called before the first frame update
    void Start()
    {
        int numObjects = ObjectsParent.childCount;
        _objects = new List<Transform>(numObjects);
        _objectBoxes = new List<QuadPoly>(numObjects);
        _objectPolyColls = new List<PolygonCollider2D>(numObjects);
        for(int i = 0; i < numObjects; i++)
        {
            Transform sceneObj = ObjectsParent.GetChild(i);
            _objects.Add(sceneObj);

            BoxCollider2D boxColl = sceneObj.GetComponent<BoxCollider2D>();
            _objectBoxes.Add(GetBoxCorners(boxColl));

            // Replace collider with polygon
            boxColl.enabled = false;
            PolygonCollider2D polyCollider = boxColl.gameObject.AddComponent<PolygonCollider2D>();
            _objectPolyColls.Add(polyCollider);
            polyCollider.SetPath(0, _objectBoxes[i].ConvertToLocalPoly(sceneObj.worldToLocalMatrix));
        }
    }

    private void ResetCollisionBoxes()
    {
        for(int i = 0; i < _objects.Count; i++)
        {
            _objectPolyColls[i].SetPath(0, _objectBoxes[i].ConvertToLocalPoly(_objects[i].worldToLocalMatrix));
        }
    }

    private void DisableCollision()
    {
        foreach(PolygonCollider2D coll in _objectPolyColls)
        {
            coll.enabled = false;
        }
    }
    private void EnableCollision()
    {
        foreach (PolygonCollider2D coll in _objectPolyColls)
        {
            coll.enabled = true;
        }
    }

    private void Update()
    {
        foreach(PolygonCollider2D coll in _objectPolyColls)
        {
            if (!coll.enabled) continue;
            Matrix4x4 toWorld = coll.transform.localToWorldMatrix;
            Vector2[] path = coll.GetPath(0);
            for(int i = 0; i < path.Length; i++)
            {
                Debug.DrawLine(toWorld.MultiplyPoint3x4(path[i]), toWorld.MultiplyPoint3x4(path[(i + 1) % path.Length]), Color.red);
            }
        }
    }

    void FixedUpdate()
    {
        // Update positions based on angle and state
        switch (_state)
        {
            case DimensionButton.DimensionState.Full:
                EnableCollision();
                ResetCollisionBoxes();
                break;
            case DimensionButton.DimensionState.None:
                DisableCollision();
                break;

            case DimensionButton.DimensionState.HalfPos:
                EnableCollision();
                MaskHitboxes(false);
                break;
            case DimensionButton.DimensionState.HalfNeg:
                EnableCollision();
                MaskHitboxes(false);
                break;
        }
    }

    private void MaskHitboxes(bool orientation)
    {
        float theta = _angle * Mathf.Deg2Rad;
        float a = Mathf.Sin(theta);
        float b = -Mathf.Cos(theta);

        for (int i = 0; i < _objects.Count; i++)
        {
            Vector2[] clipped = ClipBoxAgainstLine(_objectBoxes[i],
                                    a, b,
                                    orientation);
            if(clipped == null) _objectPolyColls[i].enabled = false;

            else _objectPolyColls[i].SetPath(0, ConvertToLocalPoly(clipped, _objects[i].worldToLocalMatrix));
        }
    }

    private Vector2[] ClipBoxAgainstLine(QuadPoly poly, float a, float b, bool keepPositiveSide)
    {
        Vector2[] corners = poly.pts;
        List<Vector2> output = new List<Vector2>();
        int insideCount = 0, outsideCount = 0;

        float[] distances = new float[corners.Length];
        bool[] inside = new bool[corners.Length];

        // Compute signed distances and inside/outside classification
        for (int i = 0; i < corners.Length; i++)
        {
            distances[i] = a * corners[i].x + b * corners[i].y;
            inside[i] = keepPositiveSide ? distances[i] >= 0 : distances[i] <= 0;

            if (inside[i]) insideCount++;
            else outsideCount++;
        }

        // If all points are on one side, return full box or empty set
        if (insideCount == corners.Length) return poly.pts; // Entire box is inside
        if (outsideCount == corners.Length) return null; // Entire box is outside

        // Perform clipping
        for (int i = 0; i < corners.Length; i++)
        {
            Vector2 current = corners[i];
            Vector2 next = corners[(i + 1) % corners.Length]; // Loop around

            if (inside[i]) output.Add(current); // Keep inside points

            if (inside[i] != inside[(i + 1) % corners.Length]) // Edge crosses the line
            {
                float t = distances[i] / (distances[i] - distances[(i + 1) % corners.Length]);
                Vector2 intersection = current + t * (next - current);
                output.Add(intersection);
            }
        }


        return output.ToArray();
    }

}
