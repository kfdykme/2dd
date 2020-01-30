using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Completed
{

    public class BoardManager : MonoBehaviour
    {
        [Serializable]
        public class Count
        {
            public int minimum;
            public int maximum;

            public Count(int min, int max)
            {
                maximum = max;
                minimum = min;
            }
        }

        public class Light {
            public GameObject Object;
            public Vector2 position;
        }

        public int columns = 8;
        public int rows = 8;
        public Count wallCount = new Count(5, 9);
        public Count foodCount = new Count(1, 5);
        public GameObject exit;
        public GameObject[] floorTiles;
        public GameObject[] wallTiles;
        public GameObject[] foodTiles;
        public GameObject[] enemyTiles;
        public GameObject[] outerWallTiles;
        public GameObject lightRedTiles;
        public GameObject lightBlueTiles;

        private Transform boardHolder;
        private List<Vector3> gridPosition = new List<Vector3>();
        public List<Light> lights = new List<Light>();

        public List<GameItem> floors = new List<GameItem>();
        
        public List<Light> getLightsCanGo(Rigidbody2D source, int movement) {

            List<Light> tlights = new List<Light>();
            if (movement == 0) return tlights;

            for (int i = 0 ; i < lights.Count; i++) {
                if ((lights[i].position - source.position).sqrMagnitude < 1.5) {
                    tlights.Add(lights[i]);
                    tlights.AddRange(getLightsCanGo(lights[i].Object.GetComponent<Rigidbody2D>(), movement -1));
                }
            }
            return tlights;
        }

        public GameItem getByXY(int x, int y) {
            GameItem target = null;
            floors.ForEach(floor => {
                if (floor != null && floor.x == x && floor.y == y) {
                    target = floor;
                }
            });
            return target;
        }

        // todo
        public GameItem getLightByXY(int x, int y) {
            return null;
        }

        public bool getBlueLightActive(Vector2 end) {
             
            for (int i = 0 ; i < lights.Count; i++) {
                if (lights[i].position.x == end.x && lights[i].position.y == end.y) {
                    return lights[i].Object.active;
                }
            }
            return false;
        }
        void InitialiseList()
        {
            gridPosition.Clear();

            for (int i = 1; i < columns - 1; i++)
            {
                for (int y = 1; y < rows - 1; y++)
                {
                    gridPosition.Add(new Vector3(i, y, 0f));
                }
            }
        }

        void BoardSetup()
        {
            boardHolder = new GameObject("Board").transform;
            floors.Clear();
            for (int x = -1; x < columns + 1; x++)
            {
                for (int y = -1; y < rows + 1; y++)
                {
                    GameObject toInstantiate = floorTiles[Random.Range(0, floorTiles.Length)];
                    if (x == -1 || x == columns || y == -1 || y == rows)
                    {
                        toInstantiate = outerWallTiles[Random.Range(0, outerWallTiles.Length)];
                    }

                    GameObject instance = Instantiate(toInstantiate, new Vector3(x, y, 0f), Quaternion.identity) as GameObject;
                    GameItem g = instance.GetComponent<GameItem>();
                    if (g!= null) {

                    g.x = x;
                    g.y = y;
                    }
                    floors.Add(g);
                    instance.transform.SetParent(boardHolder);
                    // add lights
                    toInstantiate = lightBlueTiles;
                    instance = Instantiate(toInstantiate, new Vector3(x, y, 0f), Quaternion.identity) as GameObject;
                    g = instance.GetComponent<GameItem>();
                    if (g != null) {

                    g.x = x;
                    g.y = y;
                    } 
                    instance.transform.SetParent(boardHolder);
                    Light light = new Light();
                    light.Object = instance;
                    light.position = new Vector2(x, y);
                    lights.Add(light);
                    light.Object.SetActive(false);
                }
            }
        }

        Vector3 RandomPosition()
        {
            int randomIndex = Random.Range(0, gridPosition.Count);
            Vector3 randomPosition = gridPosition[randomIndex];
            gridPosition.RemoveAt(randomIndex);
            return randomPosition;
        }

        void layoutObjectAtRandom(GameObject[] tileArray, int minimum, int maximum)
        {
            int objectCount = Random.Range(minimum, maximum + 1);

            for (int i = 0; i < objectCount; i++)
            {
                Vector3 randomPosition = RandomPosition();
                GameObject tileChoice = tileArray[Random.Range(0, tileArray.Length)];
                Instantiate(tileChoice, randomPosition, Quaternion.identity);
                GameItem g = tileChoice.GetComponent<GameItem>();
                if (g != null) {

                    g.x = (int)randomPosition.x;
                    g.y = (int)randomPosition.y;
                }
            }
        }

        public void SetupScene(int level)
        {
            BoardSetup();
            InitialiseList();
            layoutObjectAtRandom(wallTiles, wallCount.minimum, wallCount.maximum);
            layoutObjectAtRandom(foodTiles, foodCount.minimum, foodCount.maximum);
            int enemyCount = level;
            layoutObjectAtRandom(enemyTiles, enemyCount, enemyCount);
            Instantiate(exit, new Vector3(columns - 1, rows - 1, 0f), Quaternion.identity);
        }

    }
}

