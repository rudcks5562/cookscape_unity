using System;
using System.Collections.Generic;
using UnityEngine;

using Random = UnityEngine.Random;

internal class TilePosSideTuple
{
    internal TileIndex tileIndex;
    internal Side side;
}

internal struct TileIndex
{
    public int x;
    public int z;
    public int y;

    public TileIndex(int _x, int _y, int _z) : this()
    {
        x = _x;
        y = _y;
        z = _z;
    }

    internal void SetY(int _y)
    {
        y = _y;
    }
}

public class TileMapGenerator : MonoBehaviour
{
    public float TileSizeX = 1.0f;
    public float TileSizeZ = 1.0f;
    public float TileSizeY = 1.0f;

    private GameObject[] _availableTiles;
    private Vector3 _startingPos;
    private GameObject[,,] _generatedTiles;

    private List<GameObject> _generatedProps;

    private TileIndex _realCurPos;
    private TileIndex _curPos
    {
        set
        {
            Debug.Assert(value.y == _curFloorIndex);

            _realCurPos = value;
        }

        get
        {
            return _realCurPos;
        }
    }

    private GameObject _curTile;
    private int _tileCountX;
    private int _tileCountZ;
    private int _tileCountY;
    private int _targetVerticalTransportCount;

    private int _curFloorIndex;

    public bool Started = false;

    private Stack<GameObject> _lastGeneratedTiles;

    private TileGenerationManagerScript _tileGenerationManagerScript;

    private readonly Side[] allSides = { Side.PositiveX,
            Side.NegativeX,
            Side.PositiveZ,
            Side.NegativeZ
            /*Side.PositiveXPositiveZ,
            Side.PositiveXNegativeZ,
            Side.NegativeXPositiveZ,
            Side.NegativeXNegativeZ*/};

    public void StartGeneration(Vector3 startingPosition, int tileCountX, int tileCountZ, int tileCountY, int verticalTransportCount, GameObject[] availableTiles)
    {
        _tileGenerationManagerScript = gameObject.GetComponent<TileGenerationManagerScript>();

        _availableTiles = availableTiles;
        _startingPos = startingPosition;

        _tileCountX = tileCountX;
        _tileCountZ = tileCountZ;
        _tileCountY = tileCountY;
        _targetVerticalTransportCount = verticalTransportCount;

        _generatedTiles = new GameObject[tileCountX, tileCountZ, tileCountY];
        _generatedProps = new List<GameObject>();

        _curFloorIndex = -1;
        _curPos = new TileIndex(tileCountX / 2, -1, tileCountZ / 2);

        //_curPos.x = 0;
        //_curPos.z = 0;

        for (int i = 0; i < availableTiles.Length; ++i)
        {
            availableTiles[i].GetComponent<TileScript>().InitPassableSideFlags();
        }

        StartGenerateFloor(0);
    }

    private void StartGenerateFloor(int floorIndex)
    {
        _realCurPos.SetY(floorIndex);
        _curFloorIndex = floorIndex;
        _lastGeneratedTiles = new Stack<GameObject>(_tileCountX * _tileCountZ);

        _curTile = RandomTile();

        PutTile(_curTile);

        Started = true;
    }

    public void ClearAllGenerated()
    {
        for (int i = 0; i < _tileCountX; ++i)
        {
            for (int j = 0; j < _tileCountZ; ++j)
            {
                for (int k = 0; k < _tileCountY; ++k)
                {
                    Destroy(_generatedTiles[i, j, k]);
                }
            }
        }

        if (_generatedProps != null)
        {
            foreach (GameObject prop in _generatedProps)
            {
                Destroy(prop);
            }
        }

        Started = false;
    }

    public bool GenerateOneStep()
    {
        Debug.Assert(Started);

        bool updated = UpdateCurPosRandom();

        if (!updated)
        {
			//generating this floor has ended
            Started = false;
            MakePostProcessing();

            if (_curFloorIndex + 1 < _tileCountY)
            {
                StartGenerateFloor(_curFloorIndex + 1);
                //PutTileCheckingAround();
                return false;
            }

            //generating has completely ended

            for (int i = 0; i < _targetVerticalTransportCount; ++i)
            {
                CheckAndPutVerticalTransport();
            }

            return true;
        }

        PutTileCheckingAround();

        return false;
    }

    private void CheckAndPutVerticalTransport()
    {
        if (_tileCountY < 2)
        {
            return;
        }

        List<TilePosSideTuple> validForVertical = new List<TilePosSideTuple>();
        for (int i = 0; i < _tileCountX; ++i)
        {
            for (int j = 0; j < _tileCountZ; ++j)
            {
                bool allFloorsValid = true;
                HashSet<Side> invalidSides = new HashSet<Side>();
                for (int f = 0; f < _tileCountY; ++f)
                {
                    TileIndex curTilePos;
                    curTilePos.x = i;
                    curTilePos.z = j;
                    curTilePos.y = f;

                    GameObject tile = _generatedTiles[i, j, f];
                    if (tile != null)
                    {
                        TileScript ts = tile.GetComponent<TileScript>();

                        Debug.Assert(allSides.Length == (int)Side.SideCount);

                        for (int iS = 0; iS < allSides.Length; iS++)
                        {
                            bool isSideValidForFloor = true;

                            Side s = allSides[iS];

                            if (ts.IsPassableToSide(s))
                            {
                                _curFloorIndex = f;//for debugging purpose

                                TileIndex nextPos;
                                bool isValidSide = GetNextPos(out nextPos, curTilePos, s);

                                if (isValidSide)
                                {
                                    /*TileScript nextTileS = _generatedTiles[nextPos.x, nextPos.z, nextPos.y].GetComponent<TileScript>();
                                    if (nextTileS.IsPassableToSide(s.GetInverseSide()))
                                    {
                                    }
                                    else
                                    {
                                    }*/
                                }
                                else
                                {
                                    //side is invalid for this floor
                                    isSideValidForFloor = false;
                                }
                            }
                            else
                            {
                                //side is invalid for this floor
                                isSideValidForFloor = false;
                            }

                            if (!isSideValidForFloor)
                            {
                                invalidSides.Add(s);
                            }
                        }
                    }
                    else
                    {
                        allFloorsValid = false;
                        break;
                    }
                }

                if (invalidSides.Count >= allSides.Length)
                {
                    Debug.Assert(invalidSides.Count == allSides.Length);

                    allFloorsValid = false;
                }

                if (allFloorsValid)
                {
                    //"find for valid sides and keep them"
                    for (int iS = 0; iS < allSides.Length; iS++)
                    {
                        Side s = allSides[iS];

                        if (invalidSides.Contains(s))
                        {
                            //this side is invalid
                        }
                        else
                        {
                            TilePosSideTuple tpl = new TilePosSideTuple();
                            tpl.side = s;
                            tpl.tileIndex = new TileIndex(i, -1, j);

                            validForVertical.Add(tpl);
                        }
                    }
                }
            }
        }

        if (validForVertical.Count > 0)
        {
            int randIndexForVerticalTransport = Random.Range(0, validForVertical.Count);

            TilePosSideTuple tpl = validForVertical[randIndexForVerticalTransport];
            validForVertical.RemoveAt(randIndexForVerticalTransport);

            PutVerticalTransport(tpl);
        }
    }

    private void PutElevator(Quaternion rot, TilePosSideTuple tpl)
    {
        Vector3 cabinPos = _startingPos + new Vector3(tpl.tileIndex.x * TileSizeX, 0 * TileSizeY, tpl.tileIndex.z * TileSizeZ);

        GameObject elevatorCabin = (GameObject)Instantiate(_tileGenerationManagerScript.ElevatorCabinPrefab, cabinPos, rot);
        _generatedProps.Add(elevatorCabin);
        ElevatorCabin cabinScp = elevatorCabin.GetComponent<ElevatorCabin>();

        for (int f = 0; f < _tileCountY; ++f)
        {
            TileIndex curTilePos;
            curTilePos.x = tpl.tileIndex.x;
            curTilePos.z = tpl.tileIndex.z;
            curTilePos.y = f;

            GameObject tileToBeRemoved = _generatedTiles[curTilePos.x, curTilePos.z, curTilePos.y];

            Vector3 shaftPos = tileToBeRemoved.transform.position;

            GameObject elevatorShaft = (GameObject)Instantiate(_tileGenerationManagerScript.ElevatorShaftPrefab, shaftPos, rot);
            _generatedProps.Add(elevatorShaft);
            ElevatorShaft shaftScp = elevatorShaft.GetComponent<ElevatorShaft>();
            shaftScp.SetCabin(cabinScp);
            shaftScp.SetFloorIndex(f);
            cabinScp.AddShaft(shaftScp);

            Destroy(tileToBeRemoved);
        }
    }

    private void PutStairs(Quaternion rot, TilePosSideTuple tpl)
    {
        for (int f = 0; f < _tileCountY; ++f)
        {
            TileIndex curTilePos;
            curTilePos.x = tpl.tileIndex.x;
            curTilePos.z = tpl.tileIndex.z;
            curTilePos.y = f;

            GameObject tileToBeRemoved = _generatedTiles[curTilePos.x, curTilePos.z, curTilePos.y];

            Vector3 stairsPos = tileToBeRemoved.transform.position;

            GameObject prefabToBeSpawned;
            if (f == 0)
            {
                prefabToBeSpawned = _tileGenerationManagerScript.StairsFloorPrefab;
            }
            else if (f == (_tileCountY - 1))
            {
                prefabToBeSpawned = _tileGenerationManagerScript.StairsTopPrefab;
            }
            else
            {
                prefabToBeSpawned = _tileGenerationManagerScript.StairsMiddlePrefab;
            }

            GameObject stairs = (GameObject)Instantiate(prefabToBeSpawned, stairsPos, rot);
            _generatedProps.Add(stairs);

            Destroy(tileToBeRemoved);
            _generatedTiles[curTilePos.x, curTilePos.z, curTilePos.y] = null;
        }

        MakePostProcessingForStairs(tpl);
    }

    private bool DisablingPassageGeneratesDeadEnd(TileIndex tileIndex, Side curS)
    {
        Side invCurS = curS.GetInverseSide();

        for (int f = 0; f < _tileCountY; ++f)
        {
            TileIndex curTilePos;
            curTilePos.x = tileIndex.x;
            curTilePos.z = tileIndex.z;
            curTilePos.y = f;
            _curFloorIndex = f;

            TileIndex sidePos;
            bool validSide = GetNextPos(out sidePos, curTilePos, curS);

            if (!validSide)
            {
                return false;
            }

            GameObject tile = _generatedTiles[sidePos.x, sidePos.z, f];
            if (tile != null)
            {
                TileScript curTs = tile.GetComponent<TileScript>();

                if (curTs.PassableSides.Length <= 2 && curTs.IsPassableToSide(invCurS))
                {
                    return true;
                }
            }
        }

        return false;
    }

    private void MakePostProcessingForStairs(TilePosSideTuple tpl)
    {
        for (int iSide = 0; iSide < (int)Side.SideCount; ++iSide)
        {
            Side curS = allSides[iSide];

            bool shouldPass = DisablingPassageGeneratesDeadEnd(tpl.tileIndex, curS);

            for (int f = 0; f < _tileCountY; ++f)
            {
                TileIndex curTilePos;
                curTilePos.x = tpl.tileIndex.x;
                curTilePos.z = tpl.tileIndex.z;
                curTilePos.y = f;

                _curFloorIndex = f;

                TileIndex sidePos;
                bool validSide = GetNextPos(out sidePos, curTilePos, curS);

                if (!validSide)
                {
                    break;
                }
                
                GameObject tile = _generatedTiles[sidePos.x, sidePos.z, f];
                if (tile != null)
                {
                    TileScript curTs = tile.GetComponent<TileScript>();

                    float[] tilePoints = new float[_availableTiles.Length];

                    for (int iTile = 0; iTile < _availableTiles.Length; ++iTile)
                    {
                        GameObject testTileObj = _availableTiles[iTile];
                        TileScript testTile = testTileObj.GetComponent<TileScript>();

                        float curPoint = 0.0f;

                        //Side invCheckSide = tpl.side.GetInverseSide();

                        for (int its = 0; its < (int)Side.SideCount; ++its)
                        {
                            Side scoreS = allSides[its];

                            bool testTilePass = testTile.IsPassableToSide(scoreS);

                            if (tpl.side == curS && scoreS == curS.GetInverseSide())
                            {
                                if (testTilePass)
                                {
                                    curPoint += 100.0f;
                                }
                                else
                                {
                                    curPoint -= 100.0f;
                                }
                            }
                            else
                            {
                                if (scoreS == curS.GetInverseSide())
                                {
                                    if (shouldPass == testTilePass)
                                    {
                                        curPoint += 400.0f;
                                    }
                                    else
                                    {
                                        curPoint -= 400.0f;
                                    }
                                }
                                else
                                {
                                    bool curTestTilePass = curTs.IsPassableToSide(scoreS);
                                    if (testTilePass == curTestTilePass)
                                    {
                                        curPoint += 10.0f;
                                    }
                                    else
                                    {
                                        curPoint -= 20.0f;
                                    }
                                }
                            }
                        }

                        tilePoints[iTile] = curPoint;
                    }

                    float maxPoint = float.MinValue;
                    int maxPointIndex = -1;
                    for (int itp = 0; itp < tilePoints.Length; ++itp)
                    {
                        float curTilePoint = tilePoints[itp];
                        if (curTilePoint > maxPoint)
                        {
                            maxPoint = curTilePoint;
                            maxPointIndex = itp;
                        }
                    }

                    if (maxPointIndex != -1)
                    {
                        _curPos = sidePos;

                        GameObject choosenPref = _availableTiles[maxPointIndex];

                        GameObject toBeDeleted = _generatedTiles[_curPos.x, _curPos.z, _curPos.y].gameObject;
                        _generatedTiles[_curPos.x, _curPos.z, _curPos.y] = null;
                        Destroy(toBeDeleted);

                        PutTile(choosenPref);
                    }
                }

                MakePostProcessing();
            }
        }
    }

    private void PutVerticalTransport(TilePosSideTuple tpl)
    {
        Quaternion rot = _tileGenerationManagerScript.ElevatorCabinPrefab.transform.rotation;

        switch (tpl.side)
        {
            case Side.PositiveZ:
                rot.SetLookRotation(new Vector3(0, 0, -1), new Vector3(0, 1, 0));
                break;
            case Side.NegativeZ:
                //no need to rotate
                rot.SetLookRotation(new Vector3(0, 0, 1), new Vector3(0, 1, 0));//
                break;
            case Side.PositiveX:
                rot.SetLookRotation(new Vector3(-1, 0, 0), new Vector3(0, 1, 0));//
                break;
            case Side.NegativeX:
                rot.SetLookRotation(new Vector3(1, 0, 0), new Vector3(0, 1, 0));
                break;
        }

        if (_tileGenerationManagerScript.UseElevators)
        {
            PutElevator(rot, tpl);
        }
        else
        {
            PutStairs(rot, tpl);
        }
    }

    private void MakePostProcessing()
    {
        int ppCount = 0;
        const int maxPpLimit = 5;

        while (MakePostProcessingAux())
        {
            ++ppCount;

            if (ppCount >= maxPpLimit)
            {
                break;
            }
        }
    }

    private bool MakePostProcessingAux()
    {
        bool recalculationDone = false;
        for (int i = 0; i < _tileCountX; ++i)
        {
            for (int j = 0; j < _tileCountZ; ++j)
            {
                TileIndex curTilePos;
                curTilePos.x = i;
                curTilePos.z = j;
                curTilePos.y = _curFloorIndex;

                GameObject tile = _generatedTiles[i, j, _curFloorIndex];
                if (tile != null)
                {
                    TileScript ts = tile.GetComponent<TileScript>();

                    List<TileIndex> recalculatedTilePosList = new List<TileIndex>();
                    for (int iS = 0; iS < allSides.Length; iS++)
                    {
                        Side s = allSides[iS];

                        if (ts.IsPassableToSide(s))
                        {
                            TileIndex nextPos;
                            bool isValidSide = GetNextPos(out nextPos, curTilePos, s);

                            bool recalculateTile = !isValidSide || (_generatedTiles[nextPos.x, nextPos.z, nextPos.y] != null &&
                                !_generatedTiles[nextPos.x, nextPos.z, nextPos.y].GetComponent<TileScript>().IsPassableToSide(s.GetInverseSide()));

                            if (recalculateTile)
                            {
                                Debug.Assert(_curFloorIndex == nextPos.y);

                                if (isValidSide)
                                {
                                    recalculatedTilePosList.Add(nextPos);
                                }
                                else
                                {
                                    recalculatedTilePosList.Insert(0, curTilePos);//if we will recalculate current tile. We should recalculate it first
                                }
                            }
                        }
                    }

                    for (int iR = 0; iR < recalculatedTilePosList.Count; ++iR)
                    {
                        _curPos = recalculatedTilePosList[iR];
                        GameObject tileToBeDeleted = _generatedTiles[_curPos.x, _curPos.z, _curPos.y];
                        _generatedTiles[_curPos.x, _curPos.z, _curPos.y] = null;

                        Destroy(tileToBeDeleted);

                        PutTileCheckingAround();

                        recalculationDone = true;
                    }
                }
            }
        }

        return recalculationDone;
    }

    private void PutTileCheckingAround()
    {
        Debug.Assert(allSides.Length == (int)Side.SideCount);

        float[] tilePoints = new float[_availableTiles.Length];

        for (int iTile = 0; iTile < _availableTiles.Length; ++iTile)
        {
            GameObject testTileObj = _availableTiles[iTile];
            TileScript testTile = testTileObj.GetComponent<TileScript>();

            float curPoint = 0.0f;

            for (int iSide = 0; iSide < (int)Side.SideCount; ++iSide)
            {
                Side curS = allSides[iSide];

                TileIndex sidePos;
                bool validSide = GetNextPos(out sidePos, _curPos, curS);

                if (!validSide)
                {
                    if (testTile.IsPassableToSide(curS))
                    {
                        curPoint -= 130.0f;
                    }
                    else
                    {
                        curPoint += 10.0f;
                    }

                    continue;
                }

                if (_generatedTiles[sidePos.x, sidePos.z, sidePos.y] != null)
                {
                    TileScript sideTile = _generatedTiles[sidePos.x, sidePos.z, sidePos.y].GetComponent<TileScript>();

                    bool matchingSides = testTile.IsPassableToSide(curS) && sideTile.IsPassableToSide(curS.GetInverseSide());
                    matchingSides = matchingSides || (!testTile.IsPassableToSide(curS) && !sideTile.IsPassableToSide(curS.GetInverseSide()));

                    if (matchingSides)
                    {
                        curPoint += 10.0f;
                    }
                    else
                    {
                        curPoint -= 30.0f;
                    }
                }
                else
                {
                    if (testTile.IsPassableToSide(curS))
                    {
                        TileIndex nextTestPos;
                        bool validPos = GetNextPos(out nextTestPos, _curPos, curS);

                        if (validPos)
                        {
                            Side[] sidesToCheck = null;

                            switch (curS)
                            {
                                case Side.PositiveZ:
                                    sidesToCheck = new Side[] { Side.PositiveZ, Side.PositiveX, Side.NegativeX };
                                    break;
                                case Side.NegativeZ:
                                    sidesToCheck = new Side[] { Side.NegativeZ, Side.PositiveX, Side.NegativeX };
                                    break;
                                case Side.PositiveX:
                                    sidesToCheck = new Side[] { Side.PositiveZ, Side.PositiveZ, Side.NegativeZ };
                                    break;
                                case Side.NegativeX:
                                    sidesToCheck = new Side[] { Side.NegativeZ, Side.PositiveZ, Side.NegativeZ };
                                    break;
                            }

                            for (int iS = 0; sidesToCheck != null && (iS < sidesToCheck.Length); ++iS)
                            {
                                Side nextNextSide = sidesToCheck[iS];
                                TileIndex nextNextTestPos;
                                bool nextPosValid = GetNextPos(out nextNextTestPos, nextTestPos, nextNextSide);

                                if (nextPosValid)
                                {
                                    GameObject nextNextTile = _generatedTiles[nextNextTestPos.x, nextNextTestPos.z, nextNextTestPos.y];
                                    if (nextNextTile != null)
                                    {
                                        if (nextNextTile.GetComponent<TileScript>().IsPassableToSide(nextNextSide.GetInverseSide()))
                                        {
                                            curPoint += 10.0f;
                                        }
                                        else
                                        {
                                            curPoint += 1.0f;
                                        }
                                    }
                                    else
                                    {
                                        //curPoint += 5.0f;
                                    }
                                }
                            }
                        }
                    }
                }

            }

            tilePoints[iTile] = curPoint;
        }

        float maxPoint = float.MinValue;
        for (int iTiles = 0; iTiles < tilePoints.Length; ++iTiles)
        {
            if (maxPoint < tilePoints[iTiles])
            {
                maxPoint = tilePoints[iTiles];
            }
        }


        List<int> maxPointIndices = new List<int>();
        for (int iTiles = 0; iTiles < tilePoints.Length; ++iTiles)
        {
            if (tilePoints[iTiles] >= maxPoint - 0.1f)
            {
                maxPointIndices.Add(iTiles);
            }
        }

        int r = Random.Range(0, maxPointIndices.Count);

        GameObject choosenPref = _availableTiles[maxPointIndices[r]];

        PutTile(choosenPref);
    }

    private bool UpdateCurPosRandom()
    {
        List<Side> goableSideList = new List<Side>();

        GameObject tile = _lastGeneratedTiles.Peek();
        TileScript curTileScp = tile.GetComponent<TileScript>();

        Side[] sideList = curTileScp.PassableSides;

        for (int i = 0; i < sideList.Length; ++i)
        {
            Side s = sideList[i];

            TileIndex tmpPos;
            bool isValidPos = GetNextPos(out tmpPos, curTileScp.Pos, s);

            Debug.Assert(curTileScp.IsPassableToSide(s));

            if (isValidPos && (_generatedTiles[tmpPos.x, tmpPos.z, tmpPos.y] == null))
            {
                Side invS = s.GetInverseSide();

                if (IsThereTileThatHasSide(invS))
                {
                    goableSideList.Add(s);
                }
            }
        }

        if (goableSideList.Count > 0)
        {
            Side randomSide = goableSideList[Random.Range(0, goableSideList.Count)];
            TileIndex tmpPos;
            bool isValidPos = GetNextPos(out tmpPos, curTileScp.Pos, randomSide);

            Debug.Assert(isValidPos);
            _curPos = tmpPos;

            return true;
        }
        else
        {
            if (_lastGeneratedTiles.Count > 1)
            {
                _lastGeneratedTiles.Pop();
                return UpdateCurPosRandom();
            }

            List<TileIndex> avaliablePositions = new List<TileIndex>();
            for (int i = 0; i < _tileCountX; ++i)
            {
                for (int j = 0; j < _tileCountZ; ++j)
                {
                    if (_generatedTiles[i, j, _curFloorIndex] == null)
                    {
                        avaliablePositions.Add(new TileIndex(i, _curFloorIndex, j));
                    }
                }
            }

            if (avaliablePositions.Count > 0)
            {
                int r = Random.Range(0, avaliablePositions.Count);
                _curPos = avaliablePositions[r];

                return true;
            }
            else
            {
                return false;
            }
        }
    }

    private bool IsThereTileThatHasSide(Side s)
    {
        for (int i = 0; i < _availableTiles.Length; ++i)
        {
            if (_availableTiles[i].GetComponent<TileScript>().IsPassableToSide(s))
            {
                return true;
            }
        }

        return false;
    }

    private bool GetNextPos(out TileIndex outPoint, TileIndex curPoint, Side s)
    {
        Debug.Assert(curPoint.y == _curFloorIndex);

        outPoint = curPoint;

        switch (s)
        {
            case Side.PositiveX:
                ++outPoint.x;
                break;
            case Side.NegativeX:
                --outPoint.x;
                break;
            case Side.PositiveZ:
                ++outPoint.z;
                break;
            case Side.NegativeZ:
                --outPoint.z;
                break;
            /*case Side.PositiveXPositiveZ:
                ++outPoint.x;
                ++outPoint.z;
                break;
            case Side.PositiveXNegativeZ:
                ++outPoint.x;
                --outPoint.z;
                break;
            case Side.NegativeXPositiveZ:
                --outPoint.x;
                ++outPoint.z;
                break;
            case Side.NegativeXNegativeZ:
                --outPoint.x;
                --outPoint.z;
                break;*/
        }


        return IsPosInside(outPoint);
    }

    private bool IsPosInside(TileIndex p)
    {
        Debug.Assert(p.y >= 0 && p.y < _tileCountY);
        return (p.x >= 0) && (p.x < _tileCountX) && (p.z >= 0) && (p.z < _tileCountZ);
    }

    private void PutTile(GameObject newCurTile)
    {
        _curTile = newCurTile;

        Debug.Assert(_curPos.y == _curFloorIndex);

        Vector3 pos = _startingPos + new Vector3(_curPos.x * TileSizeX, _curPos.y * TileSizeY, _curPos.z * TileSizeZ);
        GameObject go = (GameObject)Instantiate(newCurTile, pos, newCurTile.transform.rotation);
        go.GetComponent<TileScript>().InitPassableSideFlags();
        go.GetComponent<TileScript>().Pos = _curPos;

        _lastGeneratedTiles.Push(go);

        Debug.Assert(_generatedTiles[_curPos.x, _curPos.z, _curPos.y] == null);

        _generatedTiles[_curPos.x, _curPos.z, _curPos.y] = go;
    }

    private GameObject RandomTile(Side availableSide = Side.Invalid)
    {
        if (availableSide == Side.Invalid)
        {
            int r = Random.Range(0, _availableTiles.Length);

            GameObject tile = _availableTiles[r];
            return tile;
        }

        {
            List<GameObject> passableTiles = new List<GameObject>();
            for (int i = 0; i < _availableTiles.Length; ++i)
            {
                if (_availableTiles[i].GetComponent<TileScript>().IsPassableToSide(availableSide))
                {
                    passableTiles.Add(_availableTiles[i]);
                }
            }

            int r = Random.Range(0, passableTiles.Count);
            GameObject tile = passableTiles[r];

            return tile;
        }
    }
}