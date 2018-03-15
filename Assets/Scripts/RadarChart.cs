using System.Collections;
using UnityEngine;
using UnityEngine.UI;

[ExecuteInEditMode]
public class RadarChart : Graphic {
	public RectTransform[] maxPoints;
	private float[] percents = new float[5] { 1, 1, 1, 1, 1 };
    private Vector3[] vertexes = new Vector3[6];
	private bool isDirty = true;
	
	void Update() {
		#if UNITY_EDITOR
		isDirty = true;
		#endif
		if(isDirty) {
			isDirty = false;
			refresh();
		}
	}

	public void refresh() {
		vertexes[0] = maxPoints[0].anchoredPosition;
		for(int i=1; i<maxPoints.Length; i++) {
			vertexes[i] = maxPoints[0].anchoredPosition + (maxPoints[i].anchoredPosition - maxPoints[0].anchoredPosition)*percents[i-1];
		}
		SetAllDirty();
	}

	public float perA {
		get {
			return percents[0];
		}
		set {
			percents[0] = Mathf.Clamp01(value);
			isDirty = true;
		}
	}

	public float perB {
		get {
			return percents[1];
		}
		set {
			percents[1] = Mathf.Clamp01(value);
			isDirty = true;
		}
	}

	public float perC {
		get {
			return percents[2];
		}
		set {
			percents[2] = Mathf.Clamp01(value);
			isDirty = true;
		}
	}

	public float perD {
		get {
			return percents[3];
		}
		set {
			percents[3] = Mathf.Clamp01(value);
			isDirty = true;
		}
	}

	public float perE {
		get {
			return percents[4];
		}
		set {
			percents[4] = Mathf.Clamp01(value);
			isDirty = true;
		}
	}

    protected override void OnPopulateMesh(Mesh m)
    {
        var r = GetPixelAdjustedRect();
        var v = new Vector4(r.x, r.y, r.x + r.width, r.y + r.height);

        Color32 color32 = color;
        using (var vh = new VertexHelper())
        {
            foreach (Vector3 p in vertexes)
            {
                vh.AddVert(p, color32, Vector2.zero);
            }

            vh.AddTriangle(2, 0, 1);
            vh.AddTriangle(3, 0, 2);
            vh.AddTriangle(4, 0, 3);
            vh.AddTriangle(5, 0, 4);
            vh.AddTriangle(1, 0, 5);

            vh.FillMesh(m);
        }
    }
}