﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class FieldScript : MonoBehaviour {

    public GameObject letter, number, fieldCell, rotateB;


    int fieldSize = 10;

    int GameState = 0;
    int CurrentShipType = 4;
    int CurrentShipDirrection = 0;

    GameObject[] ShipForWindowY;
    GameObject[] ShipForWindowX;
    GameObject rotateButton;

    GameObject[] letters;
    GameObject[] numbers;
    GameObject[,] field;

    public bool FieldLock = false;
    public bool HideShips = false;

    int[] ShipsCount = { 0, 4, 3, 2, 1 };

    struct Coords
    {
        public int X, Y;
    }
    struct Ship
    {
        public Coords[] ShipCoords;
    }

    List<Ship> ShipsList = new List<Ship>();

    void BuildField()
    {
        Vector3 startPose = transform.position;

        Point startPoint = new Point(startPose.x + 1, startPose.y - 1);

        letters = new GameObject[fieldSize];
        numbers = new GameObject[fieldSize];
        field =  new GameObject[fieldSize,fieldSize];

        ShipForWindowInit();

        for (int i = 0; i < fieldSize; i++)
        {
            letters[i] = Instantiate(letter);
            letters[i].transform.position = new Vector3(startPoint.x, startPose.y, startPose.z);
            letters[i].GetComponent<CellImgScript>().imgId = i;
            startPoint.x++;

            numbers[i] = Instantiate(number);
            numbers[i].transform.position = new Vector3(startPose.x, startPoint.y, startPose.z);
            numbers[i].GetComponent<CellImgScript>().imgId = i;
            startPoint.y--;
        }

        startPoint = new Point(startPose.x + 1, startPose.y - 1);

        for (int i = 0; i < fieldSize; i++)
        {
            for (int j = 0; j < fieldSize; j++)
            {
                field[i, j] = Instantiate(fieldCell);
                field[i, j].transform.position = new Vector3(startPoint.x, startPoint.y, startPose.z);
                field[i, j].GetComponent<CellImgScript>().imgId = 0;
                field[i, j].GetComponent<CellImgScript>().HideCell = HideShips;
                if(!FieldLock)
                field[i, j].GetComponent<CellClick>().parent = this.gameObject;
                field[i, j].GetComponent<CellClick>().coordX = i;
                field[i, j].GetComponent<CellClick>().coordY = j;
                startPoint.x++;
            }
            startPoint.x = startPose.x + 1;
            startPoint.y--;
        }
    }

    bool CheckIfShipAround(int X, int Y)
    {
        if ((X > -1) && (Y > -1) && (X < 10) && (Y < 10))
        {
            int[] XX = new int[9], YY = new int[9];
            XX[0] = X + 1;
            YY[0] = Y + 1;
            XX[1] = X;
            YY[1] = Y + 1;
            XX[2] = X - 1;
            YY[2] = Y + 1;
            XX[3] = X + 1;
            YY[3] = Y;
            XX[4] = X;
            YY[4] = Y;
            XX[5] = X - 1;
            YY[5] = Y;
            XX[6] = X + 1;
            YY[6] = Y - 1;
            XX[7] = X;
            YY[7] = Y - 1;
            XX[8] = X - 1;
            YY[8] = Y - 1;

            for (int i = 0; i < 9; i++)
            {
                if ((XX[i] > -1) && (YY[i] > -1) && (XX[i] < 10) && (YY[i] < 10))
                {
                    if (field[XX[i], YY[i]].GetComponent<CellImgScript>().imgId != 0)
                        return false;
                }
            }
        }
            return true;
        
    }

    Coords[] CheckPlacingShipDirection(int ShipType, int Xd, int Yd, int X, int Y) // Xd, Yd - смещение по осям проверки
    {
        Coords[] result = new Coords[ShipType];
        for (int i = 0; i < ShipType; i++)
        {
            if (CheckIfShipAround(X, Y))
            {
                result[i].X = X;
                result[i].Y = Y;
            }
            else
                return null;
            X += Xd;
            Y += Yd;
        }
        return result;
    }

    Coords[] CheckPlacingShip (int ShipType, int Direction, int X, int Y) //Direction: 0-vertical, 1-horizontal
    {
        Coords[] result = new Coords[ShipType];

        if (CheckIfShipAround(X, Y))
        {
            switch (Direction)
            {
                case 0:
                    result = CheckPlacingShipDirection(ShipType, 1, 0, X, Y);
                    if (result == null)
                        result = CheckPlacingShipDirection(ShipType, -1, 0, X, Y);

                    break;
                case 1:
                    result = CheckPlacingShipDirection(ShipType, 0, 1, X, Y);
                    if (result == null)
                        result = CheckPlacingShipDirection(ShipType, 0, -1, X, Y);
                    
                    break;
            }

            return result;
        }

        return null;
    }
    public string str;
    bool CheckIfPlaceable(Coords[] coords)
    {
        if (coords != null)
        {
            int counter = 0;
            foreach (Coords T in coords)
            {
                if ((T.X > -1) && (T.Y > -1) && (T.X < 10) && (T.Y < 10))
                    counter++;
            }
            if (counter == coords.Length)
                return true;
        }
        return false;
    }

    bool PlacingShip (int ShipType, int Direction, int X, int Y)
    {
        Coords[] coordsT = CheckPlacingShip(ShipType, Direction, X, Y);
        str += CheckIfPlaceable(coordsT).ToString() + " ";
        if (coordsT != null)
        {
            if (CheckIfPlaceable(coordsT))
            {
                foreach (Coords T in coordsT)
                {
                    field[T.X, T.Y].GetComponent<CellImgScript>().imgId = 1;
                }

                Ship sh;
                sh.ShipCoords = coordsT;
                ShipsList.Add(sh);

                return true;
            }
        }

        return false;
    }

    bool isDead(int X, int Y)
    {
        bool result = false;
        foreach (Ship ship in ShipsList)
        {
            foreach (Coords shipPart in ship.ShipCoords)
            {
                if ((shipPart.X == X) && (shipPart.Y == Y))
                {
                    int deathCount = 0;
                    foreach (Coords deathShipPart in ship.ShipCoords)
                    {
                        int check = field[deathShipPart.X, deathShipPart.Y].GetComponent<CellImgScript>().imgId;
                        if (check == 3)
                            deathCount++;
                    }
                    if (deathCount == ship.ShipCoords.Length)
                    {
                        foreach (Coords c in ship.ShipCoords)
                            CellMissAround(c.X, c.Y);
                        result = true;
                    }
                    else
                        result = false;

                    return result;
                }
            }
        }

        return result;
    }
    void CellMissAround(int X, int Y)
    {
        int[] XX = new int[9], YY = new int[9];
        XX[0] = X + 1;
        YY[0] = Y + 1;
        XX[1] = X;
        YY[1] = Y + 1;
        XX[2] = X - 1;
        YY[2] = Y + 1;
        XX[3] = X + 1;
        YY[3] = Y;
        XX[4] = X;
        YY[4] = Y;
        XX[5] = X - 1;
        YY[5] = Y;
        XX[6] = X + 1;
        YY[6] = Y - 1;
        XX[7] = X;
        YY[7] = Y - 1;
        XX[8] = X - 1;
        YY[8] = Y - 1;
        for (int i = 0; i < 9; i++)
        {
            if ((XX[i] > -1) && (YY[i] > -1) && (XX[i] < 10) && (YY[i] < 10))
            {
                if (field[XX[i], YY[i]].GetComponent<CellImgScript>().imgId == 0)
                    field[XX[i], YY[i]].GetComponent<CellImgScript>().imgId = 2;
            }
        }
    }
    void FieldClear()
    {
        ShipsCount = new int[] { 0, 4, 3, 2, 1 };
        ShipsList.Clear();
        CurrentShipType = 4;
        foreach (GameObject go in field)
        {
            go.GetComponent<CellImgScript>().imgId = 0;
        }
        ShipForWindowUpate();
    }

    bool ShipsExist()
    {
        int i = 0;
        foreach (int ship in ShipsCount)
            i += ship;
        if (i != 0)
            return true;
        return false;
    }

    void GenerateRandomField()
    {
        FieldClear();
        int selectedShip = 4;

        int X, Y;
        int Direction;

        while (ShipsExist())
        {
            
            X = Random.Range(0, 10);
            Y = Random.Range(0, 10);

            Direction = Random.Range(0, 2);

            if (PlacingShip(selectedShip, Direction, X, Y))
            {
                ShipsCount[selectedShip]--;

                if (ShipsCount[selectedShip] == 0)
                    selectedShip--;
            }
        }
        CurrentShipType = 0;
        ShipForWindowUpate();

    }

    bool Shot(int X, int Y)
    {
        int cellStatus = field[X, Y].GetComponent<CellImgScript>().imgId;
        bool result = false;
        switch (cellStatus)
        {
            case 0:
                field[X, Y].GetComponent<CellImgScript>().imgId = 2;
                result = false;
                // промах
                break;

            case 1:
                field[X, Y].GetComponent<CellImgScript>().imgId = 3;
                result = true;
                if (isDead(X, Y))
                {
                    //убит

                }
                else
                {
                    //ранен
                }
                break;
        }
        return result;
    }

    private void OnGUI()
    {
        GUI.Label(new Rect(200, 15, 150, 75), str);
    }
    public void Click(int X, int Y)
    {
        switch (GameState)
        {
            case 0:
                if (ShipsCount[CurrentShipType] >= 0)
                {
                    if (ShipsCount[CurrentShipType] != 0)
                    {
                        if (PlacingShip(CurrentShipType, CurrentShipDirrection, X, Y))
                            ShipsCount[CurrentShipType]--;
                    }
                    if (ShipsCount[CurrentShipType] == 0)
                        CurrentShipType--;
                }
                break;

            case 1:
                Shot(X, Y);
                break;
        }
        ShipForWindowUpate();
        //Shot(X, Y);
        //PlacingShip (3, 1, X, Y);
        //if (CheckIfShipAround(X,Y)) field[X, Y].GetComponent<CellImgScript>().imgId = 1;
    }

    void ShipForWindowInit()
    {
        Vector3 startPose = transform.position;
        Point startPoint;
        startPoint = new Point(startPose.x - 9, startPose.y - 1);
        
        ShipForWindowY = new GameObject[4];
        ShipForWindowX = new GameObject[4];
        for (int i = 0; i < 4; i++)
        {
            ShipForWindowY[i] = Instantiate(fieldCell);
            ShipForWindowY[i].transform.position = new Vector3(startPoint.x, startPoint.y, startPose.z);
            ShipForWindowY[i].GetComponent<CellImgScript>().imgId = 4;
            startPoint.y--;
        }
        startPoint = new Point(startPose.x - 9, startPose.y - 2);
        for (int i = 0; i < 4; i++)
        {
            ShipForWindowX[i] = Instantiate(fieldCell);
            ShipForWindowX[i].transform.position = new Vector3(startPoint.x, startPoint.y, startPose.z);
            ShipForWindowX[i].GetComponent<CellImgScript>().imgId = 4;
            startPoint.x++;
        }
    }
    void ShipForWindowUpate()
    {
        for (int i = 0; i < 4; i++)
        {
            ShipForWindowX[i].GetComponent<CellImgScript>().imgId = 4;
            ShipForWindowY[i].GetComponent<CellImgScript>().imgId = 4;
        }
        for (int i = 0; i < CurrentShipType; i++)
        {
            if (CurrentShipDirrection == 1)
                ShipForWindowX[i].GetComponent<CellImgScript>().imgId = 1;
            else
                ShipForWindowY[i].GetComponent<CellImgScript>().imgId = 1;
        }
    }

    void RotateButtonInit()
    {
        Vector3 startPose = transform.position;
        Point startPoint;
        startPoint = new Point(startPose.x - 8, startPose.y - 6);

        rotateButton = Instantiate(rotateB);
        rotateButton.transform.position = new Vector3(startPoint.x, startPoint.y, startPose.z);
        rotateButton.GetComponent<RotateButtonScript>().parent = this.gameObject;
    }

    public void Rotate()
    {
        if (CurrentShipDirrection == 0)
            CurrentShipDirrection = 1;
        else
            CurrentShipDirrection = 0;
        ShipForWindowUpate();
    }

	// Use this for initialization
	void Start () {
        BuildField();
        ShipForWindowUpate();
        RotateButtonInit();
	}
	
	// Update is called once per frame
	void Update () {
        if (Input.GetKeyDown(KeyCode.R))
            GenerateRandomField();
        if (Input.GetKeyDown(KeyCode.C))
            FieldClear();
        if (Input.GetKeyDown(KeyCode.Q))
            GameState = 1;
	}
}
