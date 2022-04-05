using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager instance = null;
    public List<IEntity> entities = new List<IEntity>();
    public Queue<GameObject> spawns = new Queue<GameObject>();
    public MapGrid mapGrid;
    public SafeZone safeZone;
    public CameraController cameraController;
    public UIManager uiManager;
    public HighScores highscores;
    public float tickLength = 1f;
    public Transform entityContainer;

    public int year = 0;
    public int population = 0;
    public int food = 100;
    public int wood = 100;
    public int stone = 100;

    public int score;

    [Header("Unit Prefabs")]
    public GameObject soldierPrefab;
    public GameObject farmerPrefab;
    public GameObject woodcutterPrefab;
    public GameObject minerPrefab;

    [Header("Building Prefabs")]
    public GameObject shelterPrefab;
    public GameObject barracksPrefab;
    public GameObject farmPrefab;
    public GameObject lumberyardPrefab;
    public GameObject minePrefab;

    [Header("Zombie Prefabs")]
    public GameObject walkerPrefab;
    public GameObject runnerPrefab;
    public GameObject spitterPrefab;
    public GameObject brutePrefab;

    public bool isPlacingBuilding = false;
    public bool isPlacementValid = true;
    public bool isSelling = false;
    public bool isValidSell = false;
    public GameObject buildingToPlace;
    public Tile placeTile;

    public List<IEntity> shelters = new List<IEntity>();

    float nextTick = 0;
    int counter = 0;
    bool startTracking = false;
    bool gameEnded = false;
    public int maxPopulation = 0;

    private void Awake()
    {
        Time.timeScale = 1;
        if(instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }

        mapGrid.GenerateMap();
        SpawnStarters();
    }

    void SpawnStarters()
    {
        SpawnEntity(shelterPrefab, mapGrid.GetTile(300, 300));
        SpawnEntity(soldierPrefab, mapGrid.GetTile(307, 307));
        SpawnEntity(soldierPrefab, mapGrid.GetTile(293, 293));
        SpawnEntity(farmerPrefab, mapGrid.GetTile(310, 300));
        SpawnEntity(farmerPrefab, mapGrid.GetTile(290, 300));
        SpawnEntity(woodcutterPrefab, mapGrid.GetTile(300, 310));
        SpawnEntity(woodcutterPrefab, mapGrid.GetTile(300, 290));
        SpawnEntity(minerPrefab, mapGrid.GetTile(293, 307));
        SpawnEntity(minerPrefab, mapGrid.GetTile(307, 293));
    }

    private void Update()
    {
        if(startTracking &&  population == 0 && !gameEnded)
        {
            gameEnded = true;
            uiManager.GameOver();
        }
        if(population > maxPopulation)
        {
            maxPopulation = population;
        }

        if(Time.timeSinceLevelLoad > nextTick)
        {
            foreach(IEntity e in entities)
            {
                e.Rules();
            }

            for (int i = 0; i < spawns.Count; i++)
            {
                GameObject g = spawns.Dequeue();
                if(g == null) { continue; }

                IEntity entity = g.GetComponent<IEntity>();
                entities.Add(entity);
                if (entity.IsBuilding)
                {
                    SetUnwalkable(entity.Tile, entity);
                }
                else if(!entity.IsZombie)
                {
                    population++;

                    if (!startTracking)
                    {
                        startTracking = true;
                    }
                }
            }

            nextTick += tickLength;
            counter++;
            if(counter > 23)
            {
                year++;
                counter = 0;
            }

            foreach(Tile t in mapGrid.DarkTiles())
            {
                int spawnChance = Random.Range(0, 100000);
                if(spawnChance < year)
                {
                    int r = Random.Range(0, 8);
                    GameObject prefab;
                    if (r < 3)
                    {
                        prefab = walkerPrefab;
                    }
                    else if (r < 5)
                    {
                        prefab = runnerPrefab;
                    }
                    else if (r < 7)
                    {
                        prefab = spitterPrefab;
                    }
                    else
                    {
                        prefab = brutePrefab;
                    }
                    SpawnEntity(prefab, t);
                }
                
            }
        }

        if (isPlacingBuilding) { isPlacementValid = CheckBuildingSpace(placeTile); }
        if (isSelling) { isValidSell = CheckBuildingSell(); }
        if (isPlacingBuilding && isPlacementValid && Input.GetMouseButtonDown(0))
        {
            stone -= 200;
            wood -= 200;
            SpawnEntity(buildingToPlace, placeTile);
            isPlacingBuilding = false;
            score++;
        }
        if(isSelling && isValidSell && Input.GetMouseButtonDown(0))
        {
            stone += 100;
            wood += 100;
            placeTile.entities[0].Strength = -10;
            isPlacingBuilding = false;
        }

        if(isPlacingBuilding && Input.GetMouseButtonDown(1))
        {
            isPlacingBuilding = false;
        }
    }

    public void SpawnEntity(GameObject prefab, Tile tile)
    {
        GameObject entityObject = Instantiate<GameObject>(prefab);
        IEntity entity = entityObject.GetComponent<IEntity>();

        entity.Tile = tile;
        tile.entities.Add(entity);
 
        entityObject.transform.position = tile.worldPos;
        entityObject.transform.SetParent(entityContainer);

        spawns.Enqueue(entityObject);

        /*
        if (buildingToPlace.GetComponent<Test>() != null)
        {
            Debug.Log("made it here");

            shelters.Add(entity);
            safeZone.SetZone(entity.Tile, true);
        }*/
    }

    public void CollectResource(TileType type)
    {
        if(type == TileType.Field)
        {
            food++;
        }
        if (type == TileType.Tree)
        {
            wood++;
        }
        if (type == TileType.Rock)
        {
            stone++;
        }
    }

    void SetUnwalkable(Tile tile, IEntity entity)
    {
        tile.isWalkable = false;
        Tile n = mapGrid.GetTile(tile.x, tile.y + 1);
        n.isWalkable = false;
        Tile ne = mapGrid.GetTile(tile.x + 1, tile.y + 1);
        ne.isWalkable = false;
        ne.entities.Add(entity);
        Tile e = mapGrid.GetTile(tile.x + 1, tile.y);
        e.isWalkable = false;
        Tile se = mapGrid.GetTile(tile.x + 1, tile.y - 1);
        se.isWalkable = false;
        se.entities.Add(entity);
        Tile s = mapGrid.GetTile(tile.x, tile.y - 1);
        s.isWalkable = false;
        Tile sw = mapGrid.GetTile(tile.x - 1, tile.y - 1);
        sw.isWalkable = false;
        sw.entities.Add(entity);
        Tile w = mapGrid.GetTile(tile.x - 1, tile.y);
        w.isWalkable = false;
        Tile nw = mapGrid.GetTile(tile.x - 1, tile.y + 1);
        nw.isWalkable = false;
        nw.entities.Add(entity);
    }

    public void SetWalkable(Tile tile, IEntity entity)
    {
        tile.isWalkable = true;
        Tile n = mapGrid.GetTile(tile.x, tile.y + 1);
        n.isWalkable = true;
        Tile ne = mapGrid.GetTile(tile.x + 1, tile.y + 1);
        ne.isWalkable = true;
        ne.entities.Remove(entity);
        ne.entities.TrimExcess();
        Tile e = mapGrid.GetTile(tile.x + 1, tile.y);
        e.isWalkable = true;
        Tile se = mapGrid.GetTile(tile.x + 1, tile.y - 1);
        se.isWalkable = true;
        se.entities.Remove(entity);
        se.entities.TrimExcess();
        Tile s = mapGrid.GetTile(tile.x, tile.y - 1);
        s.isWalkable = true;
        Tile sw = mapGrid.GetTile(tile.x - 1, tile.y - 1);
        sw.isWalkable = true;
        sw.entities.Remove(entity);
        sw.entities.TrimExcess();
        Tile w = mapGrid.GetTile(tile.x - 1, tile.y);
        w.isWalkable = true;
        Tile nw = mapGrid.GetTile(tile.x - 1, tile.y + 1);
        nw.isWalkable = true;
        nw.entities.Remove(entity);
        nw.entities.TrimExcess();
    }

    bool CheckBuildingSpace(Tile tile)
    {
        for (int x = -1; x <= 1; x++)
        {
            for (int y = -1; y <= 1; y++)
            {
                Tile t = GameManager.instance.mapGrid.GetTile(tile.x + x, tile.y + y);

                if (!t.inSafeZone || !t.isWalkable)
                {
                    return false;
                }

                if (t.entities.Count > 0)
                {
                    if (t.entities[0].IsBuilding || t.entities[0].IsZombie)
                    {
                        return false;
                    }
                }
            }
        }

        return true;
    }

    bool CheckBuildingSell()
    {
        if(placeTile.entities.Count > 0 && placeTile.entities[0].IsBuilding)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    public void PauseGame()
    {
        Time.timeScale = 0;
    }

    public void ResumeGame()
    {
        Time.timeScale = 1;
    }
}
