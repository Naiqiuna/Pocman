using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelGenerator : MonoBehaviour
{
    public int[,] levelMap =
    {
        {1,2,2,2,2,2,2,2,2,2,2,2,2,7},
        {2,5,5,5,5,5,5,5,5,5,5,5,5,4},
        {2,5,3,4,4,3,5,3,4,4,4,3,5,4},
        {2,6,4,0,0,4,5,4,0,0,0,4,5,4},
        {2,5,3,4,4,3,5,3,4,4,4,3,5,3},
        {2,5,5,5,5,5,5,5,5,5,5,5,5,5},
        {2,5,3,4,4,3,5,3,3,5,3,4,4,4},
        {2,5,3,4,4,3,5,4,4,5,3,4,4,3},
        {2,5,5,5,5,5,5,4,4,5,5,5,5,4},
        {1,2,2,2,2,1,5,4,3,4,4,3,0,4},
        {0,0,0,0,0,2,5,4,3,4,4,3,0,3},
        {0,0,0,0,0,2,5,4,4,0,0,0,0,0},
        {0,0,0,0,0,2,5,4,4,0,3,4,4,0},
        {2,2,2,2,2,1,5,3,3,0,4,0,0,0},
        {0,0,0,0,0,0,5,0,0,0,4,0,0,0},
    };

    private Vector2[] playerPath =
    {
        new Vector2(1,1),new Vector2(1,2),new Vector2(1,3),new Vector2(1,4),new Vector2(1,5),new Vector2(1,6),new Vector2(2,6),new Vector2(3,6),new Vector2(4,6),new Vector2(5,6),
        new Vector2(5,5),new Vector2(5,4),new Vector2(5,3), new Vector2(5,2), new Vector2(5,1),new Vector2(4,1),new Vector2(3,1),new Vector2(2,1)
    };

    // 27 * 29    2700 * 2900
    private int[,] allMap = null;
    private Transform[,] allMapObjects = null;
    private Transform[] playerPathPoints = null;
    private Player player = null;

    private void Awake()
    {
        allMap = new int[levelMap.GetLength(0) * 2 - 1, levelMap.GetLength(1) * 2 - 1];
        allMapObjects = new Transform[allMap.GetLength(0), allMap.GetLength(1)];
        for (int i = 0; i < allMap.GetLength(0); i++)
        {
            if (i < levelMap.GetLength(0))
            {
                //left top
                for (int j = 0; j < levelMap.GetLength(1); j++)
                {
                    allMap[i, j] = levelMap[i, j];
                }
                //right top
                for (int j = levelMap.GetLength(1); j < allMap.GetLength(1); j++)
                {
                    allMap[i, j] = levelMap[i, allMap.GetLength(1) - 1 - j];
                }
            }
            else if(i >= levelMap.GetLength(0))
            {
                // left bottom
                for (int j = 0; j < levelMap.GetLength(1); j++)
                {
                    allMap[i, j] = levelMap[allMap.GetLength(0) - 1 - i, j];
                }
                //right bottom
                for (int j = levelMap.GetLength(1); j < allMap.GetLength(1); j++)
                {
                    allMap[i, j] = levelMap[allMap.GetLength(0) - 1 - i, allMap.GetLength(1) - 1 - j];
                }
            }
        }

        //generate map
        Camera.main.orthographicSize = (allMap.GetLength(0) + 1) * 100 / 2 / 100;
        for (int i = 0; i < allMap.GetLength(0); i++)
        {
            float y = allMap.GetLength(0) / 2 - i;
            GameObject newRow = new GameObject();
            newRow.name = i.ToString();
            newRow.transform.parent = transform;
            newRow.transform.localPosition = new Vector3(0, y, 0);
            for (int j = 0; j < allMap.GetLength(1); j++)
            {
                float x = j - allMap.GetLength(1) / 2;
                GameObject newObj = new GameObject();
                newObj.name = string.Format("{0},{1}", i, j);
                newObj.transform.parent = newRow.transform;
                newObj.transform.localPosition = new Vector3(x, 0, 0);
                int type = allMap[i, j];
                allMapObjects[i, j] = newObj.transform;
                if (type == 0) continue; //empty
                SpriteRenderer render = newObj.AddComponent<SpriteRenderer>();
                render.sprite = Resources.Load<Sprite>(allMap[i, j].ToString());
               
            }
        }
        //check rotation
        //check type 2 || 4
        for (int i = 0; i < allMap.GetLength(0); i++)
        {
            for (int j = 0; j < allMap.GetLength(1); j++)
            {
                int type = allMap[i, j];
                Transform t = allMapObjects[i, j];
                if (type == 2 || type == 4)
                {
                    if (((LeftIsWall(i, j) && RightIsWall(i, j)) == false) && (TopIsWall(i, j) && BottomIsWall(i, j) == true))
                    {
                        t.localEulerAngles = Vector3.forward * 90;
                    }
                }
            }
        }
        //check type 1 || 3
        for (int i = 0; i < allMap.GetLength(0); i++)
        {
            for (int j = 0; j < allMap.GetLength(1); j++)
            {
                int type = allMap[i, j];
                Transform t = allMapObjects[i, j];
                if (type == 1 || type == 3)
                {
                    Type1RotationCheck(t, i, j);
                }
            }
        }

        //check type 7
        for (int i = 0; i < allMap.GetLength(0); i++)
        {
            for (int j = 0; j < allMap.GetLength(1); j++)
            {
                int type = allMap[i, j];
                Transform t = allMapObjects[i, j];
                if (type == 7)
                {
                    if (!BottomIsWall(i,j))
                    {
                        allMapObjects[i, j].localEulerAngles = Vector3.forward * 180;
                    }
                }
            }
        }



        //generate player
        playerPathPoints = new Transform[playerPath.Length];
        for (int i = 0; i < playerPathPoints.Length; i++)
        {
            playerPathPoints[i] = allMapObjects[(int)playerPath[i].x, (int)playerPath[i].y];
        }


        //generate player

        player = Instantiate<Player>(Resources.Load<Player>("Player"));
    }

    private void Start()
    {
        player.Init(playerPathPoints);
    }

    private Vector3 GetPos(int i,int j)
    {
        float y = allMap.GetLength(0) / 2 - i;
        float x = j - allMap.GetLength(1) / 2;
        return new Vector3(x, y, 0);
    }

    private bool IsStraightWall(int type)
    {
        return type == 2 || type == 4;
    }

    private bool IsCurveWall(int type)
    {
        return type == 1 || type == 3;
    }

    private bool IsWall(int type)
    {
        return IsStraightWall(type) || IsCurveWall(type) || type == 7;
    }

    private bool RightIsWall(int i,int j)
    {
        return j + 1 < allMap.GetLength(1) && IsWall(allMap[i, j + 1]);
    }

    private bool LeftIsWall(int i, int j)
    {
        return j - 1 >= 0 && IsWall(allMap[i, j - 1]);
    }


    private bool BottomIsWall(int i, int j)
    {
        return i + 1 < allMap.GetLength(0) && IsWall(allMap[i + 1,j]);
    }

    private bool TopIsWall(int i, int j)
    {
        return i - 1 >= 0 && IsWall(allMap[i - 1, j]);
    }

    private void Type1RotationCheck(Transform trans, int i, int j)
    {
        
        List<Vector2> walls = new List<Vector2>();
        for (int a = -1; a <= 1; a++)
        {
            if (a != 0 && a + j >= 0 && a + j < allMap.GetLength(1))
            {
                if (IsWall(allMap[i, a + j])) walls.Add(new Vector2(i, a + j));
            }
        }
        for (int a = -1; a <= 1; a++)
        {
            if (a != 0 && a + i >= 0 && a + i < allMap.GetLength(0))
            {
                if (IsWall(allMap[a + i, j])) walls.Add(new Vector2(a + i, j));
            }
        }
        if (walls.Count == 0) return;
        Vector3 pos = GetPos(i, j);

        if (walls.Count == 1)
        {
            Vector3 p = GetPos((int)walls[0].x, (int)walls[0].y);
            Vector3 d = (p - pos).normalized;
            float _cross = Vector3.Cross(Vector3.right, d).z;
            float _angle = Vector3.Angle(Vector3.right, d);
            trans.localEulerAngles = new Vector3(0, 0, _angle * (_cross > 0 ? 1 : -1));
            return;
        }

        if (walls.Count >= 3)
        {
            List<Vector2> temp = new List<Vector2>();
            List<Vector2> temp2 = new List<Vector2>();
            for (int a = 0; a < walls.Count; a++)
            {
                bool isForwardMe = IsForwardMe(i, j, (int)walls[a].x, (int)walls[a].y);
                if (IsStraightWall(allMap[(int)walls[a].x, (int)walls[a].y]) && isForwardMe) 
                {
                    temp.Add(walls[a]);
                }
                else
                {
                    temp2.Add(walls[a]);
                }
            }
            walls.Clear();
            walls.AddRange(temp.ToArray());
            walls.AddRange(temp2.ToArray());
        }

        Vector3 pos1 = GetPos((int)walls[0].x, (int)walls[0].y);
        Vector3 pos2 = GetPos((int)walls[1].x, (int)walls[1].y);
        Vector3 dir1 = (pos1 - pos).normalized;
        Vector3 dir2 = (pos2 - pos).normalized;

        float cross = Vector3.Cross(dir1, dir2).z;
        Vector3 dir = Quaternion.AngleAxis(45 * (cross > 0 ? -1 : 1), Vector3.forward) * dir1;

        float cross2 = Vector3.Cross(Vector3.right, Vector3.up * -1).z;
        Vector3 curDir = Quaternion.AngleAxis(45 * (cross2 > 0 ? -1 : 1), Vector3.forward) * Vector3.right;

        float cross3 = Vector3.Cross(curDir, dir).z;
        float angle = Vector3.Angle(curDir, dir);
        trans.localEulerAngles = new Vector3(0, 0, angle * (cross3 > 0 ? -1 : 1));
    }

    private bool IsForwardMe(int i,int j,int a, int b)
    {
     
        Transform t = allMapObjects[a, b];
        Transform me = allMapObjects[i, j];
        Vector3 dir = t.position - me.position;
        Vector3 wallDir = t.localEulerAngles == Vector3.zero ? Vector3.right : Vector3.up;
        float angle = Vector3.Angle(wallDir, dir);
        return angle == 0 || angle == 180;
    }
}
