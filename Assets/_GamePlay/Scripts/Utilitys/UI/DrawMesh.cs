using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Utilitys;
using MoveStopMove.ContentCreation.Weapon;
public class DrawMesh : MonoBehaviour
{
    private const string MODEL_DRAW_NAME = "Model";
    private const string WEAPON_DRAW_NAME = "Weapon";
    private const string BULLET_DRAW_NAME = "Bullet";

    [Header("Property")]
    [SerializeField]
    float lineThickness = 0.1f;
    [SerializeField]
    float smoothness = 0.2f;
    [SerializeField]
    float samplePeriodicDistance = 1f;
    [SerializeField]
    float z;
    [SerializeField]
    Camera cameraMesh;

    [Header("Mouse Setting")]
    [SerializeField]
    private Vector2 offset;
    [SerializeField]
    private Vector2 size;

    [Header("Data")]
    [SerializeField]
    private GameObject baseWeapon;
    [SerializeField]
    private GameObject baseBullet;
    [SerializeField]
    private GameObject obj;
    [SerializeField]
    private GameObject parentObj;

    private GameObject weapon;
    private GameObject bullet;
    private GameObject model;

    public GameObject WeaponObject => weapon;
    public GameObject BulletObject => bullet;

    Mesh mesh;
    MeshFilter meshFilter;
    private Vector3 lastMousePosition;
    private int period;
    private float lastSmooth;
    private float lastLineThickless;

    private List<Vector3> pointsData = new List<Vector3>();
    private List<GameObject> pointObjects = new List<GameObject>();

    private void Awake()
    {
        mesh = new Mesh();
        meshFilter = GetComponent<MeshFilter>();

        period = Mathf.RoundToInt(samplePeriodicDistance / smoothness);
        lastSmooth = smoothness * z / 10;
        lastLineThickless = (lineThickness * z / 100);
    }

    private int periodTimer;
    private void Update()
    {
        Vector3 mousePos = Input.mousePosition;
        if(mousePos.x < offset.x || mousePos.x > offset.x + size.x || mousePos.y < offset.y || mousePos.y > offset.y + size.y)
        {
            return;
        }

        if (Input.GetMouseButtonDown(0))
        {
            pointsData.Clear();
            mesh.Clear();
            pointObjects.Clear();
            Destroy(model);

            periodTimer = period;

            Vector3 newWorldPointPos = MathHelper.GetMouseWorldPositionWithZ(mousePos, cameraMesh, z);

            Vector3[] vertices = new Vector3[4];
            Vector2[] uv = new Vector2[4];
            int[] triangles = new int[6];

            vertices[0] = newWorldPointPos;
            vertices[1] = newWorldPointPos;
            vertices[2] = newWorldPointPos;
            vertices[3] = newWorldPointPos;

            uv[0] = Vector2.zero;
            uv[1] = Vector2.zero;
            uv[2] = Vector2.zero;
            uv[3] = Vector2.zero;

            triangles[0] = 0;
            triangles[1] = 3;
            triangles[2] = 1;

            triangles[3] = 1;
            triangles[4] = 3;
            triangles[5] = 2;

            mesh.vertices = vertices;
            mesh.uv = uv;
            mesh.triangles = triangles;
            mesh.MarkDynamic();

            meshFilter.mesh = mesh;
            lastMousePosition = newWorldPointPos;
        }

        Vector3 newWorldPoint = MathHelper.GetMouseWorldPositionWithZ(mousePos, cameraMesh, z);
        Vector3 drawDirection = newWorldPoint - lastMousePosition;
        if (Input.GetMouseButton(0) && drawDirection.sqrMagnitude > lastSmooth * lastSmooth)
        {
            #region InitData
            int time = Mathf.FloorToInt(Mathf.Sqrt(drawDirection.sqrMagnitude / (lastSmooth * lastSmooth)));
            drawDirection = drawDirection.normalized * lastSmooth;

            for(int i = 0; i < time; i++)
            {
                newWorldPoint = lastMousePosition + drawDirection;
                #endregion

                #region Draw Line
                Vector3[] vertices = new Vector3[mesh.vertices.Length + 2];
                Vector2[] uv = new Vector2[mesh.uv.Length + 2];
                int[] triangles = new int[mesh.triangles.Length + 6];

                mesh.vertices.CopyTo(vertices, 0);
                mesh.uv.CopyTo(uv, 0);
                mesh.triangles.CopyTo(triangles, 0);


                int vIndex = vertices.Length - 4;
                int vIndex0 = vIndex + 0;
                int vIndex1 = vIndex + 1;
                int vIndex2 = vIndex + 2;
                int vIndex3 = vIndex + 3;


                Vector3 mouseForwardVector = (newWorldPoint - lastMousePosition).normalized;
                Vector3 normal2D = -transform.forward;
                Vector3 newVertexUp = newWorldPoint + Vector3.Cross(mouseForwardVector, normal2D) * lastLineThickless;
                Vector3 newVertexDown = newWorldPoint + Vector3.Cross(mouseForwardVector, normal2D * -1f) * lastLineThickless;

                vertices[vIndex2] = newVertexUp;
                vertices[vIndex3] = newVertexDown;

                uv[vIndex2] = Vector2.zero;
                uv[vIndex3] = Vector2.zero;

                int tIndex = triangles.Length - 6;

                triangles[tIndex + 0] = vIndex0;
                triangles[tIndex + 1] = vIndex2;
                triangles[tIndex + 2] = vIndex1;

                triangles[tIndex + 3] = vIndex1;
                triangles[tIndex + 4] = vIndex2;
                triangles[tIndex + 5] = vIndex3;

                mesh.vertices = vertices;
                mesh.uv = uv;
                mesh.triangles = triangles;

                lastMousePosition = newWorldPoint;
                #endregion

                #region Collect Data
                if (periodTimer > 0)
                {
                    periodTimer--;
                }
                else
                {
                    periodTimer = period;
                    pointsData.Add(newWorldPoint);
                }
                #endregion
            }

        }

        if (Input.GetMouseButtonUp(0))
        {
            if(pointsData.Count > 0)
            {
                CreateModelWeaponNormalize();
            }
            
        }
    }

    
    private void CreateModelWeaponNormalize()
    {
        model = Instantiate(parentObj); //TODO: Use Pool Here
        model.name = MODEL_DRAW_NAME;
        for(int i = 0; i < pointsData.Count; i++)
        {
            GameObject newGameObj = Instantiate(obj); //TODO: Use Pool Here            
            newGameObj.transform.parent = model.transform;
            newGameObj.transform.localScale = lastSmooth * Vector3.one * 5f;
            newGameObj.transform.localPosition = pointsData[i];
            pointObjects.Add(newGameObj);
        }
        model.transform.position = transform.position;
        model.transform.eulerAngles = Vector3.zero;
        
    }
    float posZModel;
    float scale = 1f;
    private void CreateModel()
    {
        model.transform.position = Vector3.zero;
        for (int i = 0; i < pointObjects.Count; i++)
        {
            Vector3 pos = pointObjects[i].transform.localPosition * scale / pointObjects[i].transform.localScale.x;
            posZModel = pos.z;
            pos.z = 0;
            pointObjects[i].transform.localPosition = pos;
            pointObjects[i].transform.localScale = Vector3.one * scale;
        }
    }

    private void ResetModel()
    {
        for (int i = 0; i < pointObjects.Count; i++)
        {
            Vector3 pos = pointObjects[i].transform.localPosition * scale / pointObjects[i].transform.localScale.x;
            pos.z = posZModel;
            pointObjects[i].transform.localPosition = pos;
            pointObjects[i].transform.localScale = Vector3.one * scale;
        }
    }



    private readonly Vector3 MODEL_SCALE = new Vector3(0.05f,0.05f,0.05f);  
    public void CreateWeaponAndBullet()
    {
        CreateModel();
        GetWeapon();
        GetBullet();
        ResetModel();
    }
    private GameObject GetWeapon()
    {
        Debug.Log("Get Weapon");
        weapon = Instantiate(baseWeapon);
        weapon.name = WEAPON_DRAW_NAME;

        GameObject modelObj = Instantiate(model);
        modelObj.transform.parent = weapon.transform.GetChild(0).transform;
        modelObj.transform.localScale = MODEL_SCALE;
        weapon.AddComponent<HorizontalWeapon>();
        return weapon;
    }

    private GameObject GetBullet()
    {
        bullet = Instantiate(baseBullet); //TODO: Use Pool Here
        bullet.name = BULLET_DRAW_NAME;

        GameObject modelObj = Instantiate(model);
        modelObj.transform.parent = bullet.transform;
        modelObj.transform.localScale = MODEL_SCALE;
        return bullet;
    }

    public void ResetData()
    {
        bullet = null;
        weapon = null;

        pointsData.Clear();
        mesh.Clear();
        pointObjects.Clear();
        Destroy(model);

    }

    public void DestroyData()
    {
        Destroy(bullet);
        Destroy(weapon);
    }
  
}
