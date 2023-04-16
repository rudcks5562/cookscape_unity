using UnityEngine;

public class TileGenerationManagerScript : MonoBehaviour
{
    public GameObject[] UsableTileList;
    public GameObject ElevatorCabinPrefab;
    public GameObject ElevatorShaftPrefab;

    public bool UseElevators = false;

    public GameObject StairsFloorPrefab;
    public GameObject StairsMiddlePrefab;
    public GameObject StairsTopPrefab;

    public Vector3 StartingPosition;

    public int TileCountX = 0;
    public int TileCountZ = 0;
    public int TileCountY = 0;

    public int StairsCount = 1;

    public bool UseGivenSeed = false;
    public int GivenSeed = 1241921836;

    private TileMapGenerator _generator;

    void Start()
    {
        Debug.Assert(UsableTileList != null);
        Debug.Assert(UsableTileList.Length > 0);

        if (UseGivenSeed)
        {
            Random.InitState(GivenSeed);
        }
        else
        {
            //1241921836
            int selectedSeed = (int)System.DateTime.Now.Ticks;

            Debug.Log("Used Seed: " + selectedSeed);

            Random.InitState(selectedSeed);
        }

        _generator = GetComponent<TileMapGenerator>();

        UseElevators = false;//disabled for now. Will be activated in future releases.

        GenerateAtOnce();

    }

    private double _lastGenTime = 0;
    void Update()
    {
        //GenerateStepBase();
        //GenerateAtOnce();

        if (Input.GetKeyDown(KeyCode.G))
            GenerateAtOnce();
    }

    private void GenerateAtOnce()
    {
        bool generate = _lastGenTime + 0.1 < Time.timeSinceLevelLoad;
        generate = true;
        //generate = Input.GetKeyDown(KeyCode.N);

        if (generate)
        {
            _generator.ClearAllGenerated();
            _generator.StartGeneration(StartingPosition, TileCountX, TileCountZ, TileCountY, StairsCount, UsableTileList);

            while (true)
            {
                bool generationEnd = _generator.GenerateOneStep();

                if (generationEnd)
                {
                    break;
                }
            }

            _lastGenTime = Time.timeSinceLevelLoad;
        }
    }

    private void GenerateStepBase()
    {
        float delayTime = 0.01f;
        bool generate = _lastGenTime + delayTime < Time.timeSinceLevelLoad;

        //generate = Input.GetKeyDown(KeyCode.N);

        if (generate)
        {
            bool generationEnd = false;
            if (_generator.Started)
            {
                generationEnd = _generator.GenerateOneStep();
            }
            else
            {
                _generator.ClearAllGenerated();
                _generator.StartGeneration(StartingPosition, TileCountX, TileCountZ, TileCountY, StairsCount, UsableTileList);
            }

            _lastGenTime = Time.timeSinceLevelLoad;

            if (generationEnd)
            {
                _lastGenTime += 0.1f;
            }
        }
    }
}