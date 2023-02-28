using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;
using System;

public class PlayManager : MonoBehaviour
{
    public Text textResult;
    public GameObject pauseButton;
    public GameObject askButton;
    public GameObject timer;
    public GameObject canvas;
    public float timeStart = 0;
    public Text timerText;
    public Text textStart;
    bool runTime = false;

    public GameObject CubePiecePref;
    Transform CubeTransf;
    List<GameObject> AllCubePieces = new List<GameObject>();
    GameObject CubeCenterPiece;
    bool canRotate = false,
         canShaffle = true,
         isFull = false,
         isStart = true;

    List<GameObject> UpPieces
    {
        get
        {
            return AllCubePieces.FindAll(x => Mathf.Round(x.transform.localPosition.y) == 0);
        }
    }
    List<GameObject> DownPieces
    {
        get
        {
            return AllCubePieces.FindAll(x => Mathf.Round(x.transform.localPosition.y) == -2);
        }
    }
    List<GameObject> LeftPieces
    {
        get
        {
            return AllCubePieces.FindAll(x => Mathf.Round(x.transform.localPosition.z) == 0);
        }
    }
    List<GameObject> RightPieces
    {
        get
        {
            return AllCubePieces.FindAll(x => Mathf.Round(x.transform.localPosition.z) == 2);
        }
    }
    List<GameObject> FrontPieces
    {
        get
        {
            return AllCubePieces.FindAll(x => Mathf.Round(x.transform.localPosition.x) == 0);
        }
    }
    List<GameObject> BackPieces
    {
        get
        {
            return AllCubePieces.FindAll(x => Mathf.Round(x.transform.localPosition.x) == -2);
        }
    }
    List<GameObject> UpHorizontalPieces
    {
        get
        {
            return AllCubePieces.FindAll(x => Mathf.Round(x.transform.localPosition.x) == -1);
        }
    }
    List<GameObject> UpVerticalPieces
    {
        get
        {
            return AllCubePieces.FindAll(x => Mathf.Round(x.transform.localPosition.z) == 1);
        }
    }
    List<GameObject> FrontHorizontalPieces
    {
        get
        {
            return AllCubePieces.FindAll(x => Mathf.Round(x.transform.localPosition.y) == -1);
        }
    }

    Vector3[] RotationVectors =
    {
        new Vector3(0, 1, 0), new Vector3(0, -1, 0),
        new Vector3(0, 0, -1), new Vector3(0, 0, 1),
        new Vector3(1, 0, 0), new Vector3(-1, 0, 0)
    };

    void Start()
    {
        CubeTransf = transform;
        CreateCube();
        timerText.text = timeStart.ToString("F2");
    }

    void Update()
    {
        if (runTime)
        {
            timeStart += Time.deltaTime;
            timerText.text = timeStart.ToString("F2");
        }
        if (true)
            CheckInput();
    }

    void CreateCube()
    {
        foreach (GameObject go in AllCubePieces)
            DestroyImmediate(go);

        AllCubePieces.Clear();

        for (int x = 0; x < 3; x++)
        {
            for (int y = 0; y < 3; y++)
            {
                for (int z = 0; z < 3; z++)
                {
                    GameObject go = Instantiate(CubePiecePref, CubeTransf, false);
                    go.transform.localPosition = new Vector3(-x, -y, z);
                    go.GetComponent<CubePieceScr>().SetColor(-x, -y, z);
                    AllCubePieces.Add(go);
                }
            }
        }
        CubeCenterPiece = AllCubePieces[13];
    }
    void CheckInput()
    {
        if (Input.GetKeyDown(KeyCode.W) && canRotate)
            StartCoroutine(Rotate(UpPieces, new Vector3(0, 1, 0)));
        else if (Input.GetKeyDown(KeyCode.S) && canRotate)
            StartCoroutine(Rotate(DownPieces, new Vector3(0, -1, 0)));
        else if (Input.GetKeyDown(KeyCode.A) && canRotate)
            StartCoroutine(Rotate(LeftPieces, new Vector3(0, 0, -1)));
        else if (Input.GetKeyDown(KeyCode.D) && canRotate)
            StartCoroutine(Rotate(RightPieces, new Vector3(0, 0, 1)));
        else if (Input.GetKeyDown(KeyCode.F) && canRotate)
            StartCoroutine(Rotate(FrontPieces, new Vector3(1, 0, 0)));
        else if (Input.GetKeyDown(KeyCode.B) && canRotate)
            StartCoroutine(Rotate(BackPieces, new Vector3(-1, 0, 0)));
        else if (Input.GetKeyDown(KeyCode.E) && isStart)
        {
            textStart.enabled = false;
            isStart = false;
            StartCoroutine(Shaffle());
            canShaffle = false;
            canRotate = true;
            canShaffle = true;
        }
    }

    IEnumerator Shaffle()
    {
        canShaffle = false;

        for (int moveCount = UnityEngine.Random.Range(1, 2); moveCount >= 0; moveCount--)
        {
            int edge = UnityEngine.Random.Range(0, 6);
            List<GameObject> edgePieces = new List<GameObject>();

            switch (edge)
            {
                case 0:
                    edgePieces = UpPieces;
                    break;
                case 1:
                    edgePieces = DownPieces;
                    break;
                case 2:
                    edgePieces = LeftPieces;
                    break;
                case 3:
                    edgePieces = RightPieces;
                    break;
                case 4:
                    edgePieces = FrontPieces;
                    break;
                case 5:
                    edgePieces = BackPieces;
                    break;
            }

            StartCoroutine(Rotate(edgePieces, RotationVectors[edge], 5));
            yield return new WaitForSeconds(0.3f);
            if (moveCount == 0)
                runTime = true;
        }
    }

    IEnumerator Rotate(List<GameObject> pieces, Vector3 rotationVec, int speed = 2)
    {
        canRotate = false;
        int angle = 0;

        while (angle < 90)
        {
            foreach (GameObject go in pieces)
                go.transform.RotateAround(CubeCenterPiece.transform.position, rotationVec, speed);

            angle += speed;
            yield return null;
        }
        CheckComplete();
        canRotate = true;
    }

    public void DetectRotate(List<GameObject> pieces, List<GameObject> planes)
    {
        if (!canRotate || !canShaffle)
            return;

        if (UpVerticalPieces.Exists(x => x == pieces[0]) &&
            UpVerticalPieces.Exists(x => x == pieces[1]))
            StartCoroutine(Rotate(UpVerticalPieces, new Vector3(0, 0, 1) * DetectLeftRigthSign(pieces)));
        else if (UpHorizontalPieces.Exists(x => x == pieces[0]) &&
                 UpHorizontalPieces.Exists(x => x == pieces[1]))
            StartCoroutine(Rotate(UpHorizontalPieces, new Vector3(1 * DetectFrontBackSign(pieces), 0, 0)));
        else if (FrontHorizontalPieces.Exists(x => x == pieces[0]) &&
                 FrontHorizontalPieces.Exists(x => x == pieces[1]))
            StartCoroutine(Rotate(FrontHorizontalPieces, new Vector3(0, 1 * DetectUpDownSign(pieces), 0)));

        else if (DetectSize(planes, new Vector3(1, 0, 0), new Vector3(0, 0, 1), UpPieces))
            StartCoroutine(Rotate(UpPieces, new Vector3(0, 1 * DetectUpDownSign(pieces), 0)));
        else if (DetectSize(planes, new Vector3(1, 0, 0), new Vector3(0, 0, 1), DownPieces))
            StartCoroutine(Rotate(DownPieces, new Vector3(0, 1 * DetectUpDownSign(pieces), 0)));
        else if (DetectSize(planes, new Vector3(1, 0, 0), new Vector3(0, 1, 0), LeftPieces))
            StartCoroutine(Rotate(LeftPieces, new Vector3(0, 0, 1 * DetectLeftRigthSign(pieces))));
        else if (DetectSize(planes, new Vector3(1, 0, 0), new Vector3(0, 1, 0), RightPieces))
            StartCoroutine(Rotate(RightPieces, new Vector3(0, 0, 1 * DetectLeftRigthSign(pieces))));
        else if (DetectSize(planes, new Vector3(0, 0, 1), new Vector3(0, 1, 0), FrontPieces))
            StartCoroutine(Rotate(FrontPieces, new Vector3(1 * DetectFrontBackSign(pieces), 0, 0)));
        else if (DetectSize(planes, new Vector3(0, 0, 1), new Vector3(0, 1, 0), BackPieces))
            StartCoroutine(Rotate(BackPieces, new Vector3(1 * DetectFrontBackSign(pieces), 0, 0)));
    }

    bool DetectSize(List<GameObject> planes, Vector3 fDirection, Vector3 sDirection, List<GameObject> side)
    {
        GameObject centerPiece = side.Find(x => x.GetComponent<CubePieceScr>().Planes.FindAll(y => y.activeInHierarchy).Count == 1);

        List<RaycastHit> hit1 = new List<RaycastHit>(Physics.RaycastAll(planes[1].transform.position, fDirection)),
                         hit2 = new List<RaycastHit>(Physics.RaycastAll(planes[0].transform.position, fDirection)),
                         hit1_m = new List<RaycastHit>(Physics.RaycastAll(planes[1].transform.position, -fDirection)),
                         hit2_m = new List<RaycastHit>(Physics.RaycastAll(planes[0].transform.position, -fDirection)),

                         hit3 = new List<RaycastHit>(Physics.RaycastAll(planes[1].transform.position, sDirection)),
                         hit4 = new List<RaycastHit>(Physics.RaycastAll(planes[0].transform.position, sDirection)),
                         hit3_m = new List<RaycastHit>(Physics.RaycastAll(planes[1].transform.position, -sDirection)),
                         hit4_m = new List<RaycastHit>(Physics.RaycastAll(planes[0].transform.position, -sDirection));

        return hit1.Exists(x => x.collider.gameObject == centerPiece) ||
               hit2.Exists(x => x.collider.gameObject == centerPiece) ||
               hit1_m.Exists(x => x.collider.gameObject == centerPiece) ||
               hit2_m.Exists(x => x.collider.gameObject == centerPiece) ||

               hit3.Exists(x => x.collider.gameObject == centerPiece) ||
               hit4.Exists(x => x.collider.gameObject == centerPiece) ||
               hit3_m.Exists(x => x.collider.gameObject == centerPiece) ||
               hit4_m.Exists(x => x.collider.gameObject == centerPiece);
    }

    float DetectLeftRigthSign(List<GameObject> pieces)
    {
        float sign = 0;

        if (Mathf.Round(pieces[1].transform.position.y) != Mathf.Round(pieces[0].transform.position.y))
        {
            if (Mathf.Round(pieces[0].transform.position.x) == -2)
                sign = Mathf.Round(pieces[0].transform.position.y) - Mathf.Round(pieces[1].transform.position.y);
            else
                sign = Mathf.Round(pieces[1].transform.position.y) - Mathf.Round(pieces[0].transform.position.y);
        }
        else
        {
            if (Mathf.Round(pieces[0].transform.position.y) == -2)
                sign = Mathf.Round(pieces[1].transform.position.x) - Mathf.Round(pieces[0].transform.position.x);
            else
                sign = Mathf.Round(pieces[0].transform.position.x) - Mathf.Round(pieces[1].transform.position.x);
        }

        return sign;
    }

    float DetectFrontBackSign(List<GameObject> pieces)
    {
        float sign = 0;

        if (Mathf.Round(pieces[1].transform.position.z) != Mathf.Round(pieces[0].transform.position.z))
        {
            if (Mathf.Round(pieces[0].transform.position.y) == 0)
                sign = Mathf.Round(pieces[1].transform.position.z) - Mathf.Round(pieces[0].transform.position.z);
            else
                sign = Mathf.Round(pieces[0].transform.position.z) - Mathf.Round(pieces[1].transform.position.z);
        }
        else
        {
            if (Mathf.Round(pieces[0].transform.position.z) == 0)
                sign = Mathf.Round(pieces[1].transform.position.y) - Mathf.Round(pieces[0].transform.position.y);
            else
                sign = Mathf.Round(pieces[0].transform.position.y) - Mathf.Round(pieces[1].transform.position.y);
        }

        return sign;
    }

    float DetectUpDownSign(List<GameObject> pieces)
    {
        float sign = 0;

        if (Mathf.Round(pieces[1].transform.position.z) != Mathf.Round(pieces[0].transform.position.z))
        {
            if (Mathf.Round(pieces[0].transform.position.x) == -2)
                sign = Mathf.Round(pieces[1].transform.position.z) - Mathf.Round(pieces[0].transform.position.z);
            else
                sign = Mathf.Round(pieces[0].transform.position.z) - Mathf.Round(pieces[1].transform.position.z);
        }
        else
        {
            if (Mathf.Round(pieces[0].transform.position.z) == 0)
                sign = Mathf.Round(pieces[0].transform.position.x) - Mathf.Round(pieces[1].transform.position.x);
            else
                sign = Mathf.Round(pieces[1].transform.position.x) - Mathf.Round(pieces[0].transform.position.x);
        }

        return sign;
    }

    void CheckComplete()
    {
        if (IsSideComplete(UpPieces) &&
            IsSideComplete(DownPieces) &&
            IsSideComplete(LeftPieces) &&
            IsSideComplete(RightPieces) &&
            IsSideComplete(FrontPieces) &&
            IsSideComplete(BackPieces))
        {
            runTime = false;
            canRotate = false;
            canShaffle = false;
            isFull = true;
            textResult.text = timerText.text;
            string line = "";
            List<double> resultI = new List<double>();
            using (StreamReader sr = new StreamReader("Assets\\Files\\result.txt"))
            {
                while ((line = sr.ReadLine()) != null)
                {
                    bool b = Double.TryParse(line, out double a);
                    if (b)
                    {
                        resultI.Add(a);
                    }
                }
            }
            int m = (int)Double.Parse(timerText.text) / 60;
            int s = (int)Double.Parse(timerText.text) % 60;
            resultI.Add(Convert.ToDouble(m + (s / 100.0)));
            double[] r = resultI.ToArray();
            Array.Sort(r);
            using (StreamWriter sw = new StreamWriter("Assets\\Files\\result.txt"))
            {
                for(int i=0; i<Math.Min(10, r.Length); i++)
                {
                    sw.WriteLine(r[i]);
                }
            }
            canvas.SetActive(true);
            timer.SetActive(false);
            askButton.SetActive(false);
            pauseButton.SetActive(false);
        }
    }

    bool IsSideComplete(List<GameObject> pieces)
    {
        int mainPlaneIndex = pieces[4].GetComponent<CubePieceScr>().Planes.FindIndex(x => x.activeInHierarchy);

        for (int i = 0; i < pieces.Count; i++)
        {
            if (!pieces[i].GetComponent<CubePieceScr>().Planes[mainPlaneIndex].activeInHierarchy ||
                pieces[i].GetComponent<CubePieceScr>().Planes[mainPlaneIndex].GetComponent<Renderer>().material.color !=
                pieces[4].GetComponent<CubePieceScr>().Planes[mainPlaneIndex].GetComponent<Renderer>().material.color)
                return false;
        }

        return true;
    }
}
