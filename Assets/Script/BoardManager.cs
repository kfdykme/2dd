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

            public const int STATUS_BLUE = 1;
            public const int STATUS_RED = 2;

            public Color color;
            
            public bool  status;

            public int statusCode  = -1;

            private  Color COLOR_BLUE =   new Color(0f/255f, 100f/255f, 211f/255f, .5f);

            private  Color COLOR_RED =  new Color(255f/255f, 100f/255f, 0f/255f, .5f);

            public void notify(bool status) {
                status = status;
                Object.SetActive(status);
                
                if (statusCode == -1) {
                    
                    Object.GetComponent<SpriteRenderer>().color = COLOR_BLUE;
                }
            }

            public void notify(bool status, int code) {
                notify(status);
                statusCode = code;
                switch(code) {
                    case STATUS_BLUE:
                        Object.GetComponent<SpriteRenderer>().color = COLOR_BLUE;
                        break;
                    case STATUS_RED:
                        Object.GetComponent<SpriteRenderer>().color = COLOR_RED;
                        break;
                    default:
                        break;
                }
            }
            // override object.Equals
            public override bool Equals(object obj)
            { 
                
                if (obj == null || GetType() != obj.GetType())
                {
                    return false;
                }
                Vector2 pos = ((Light)obj).position; 
                return pos.x == position.x && pos.y == position.y;
            }
            
            // override object.GetHashCode
            public override int GetHashCode()
            {   
                return base.GetHashCode();
            }
        }

        public int columns = 8;
        public int rows = 8;
        public Count wallCount = new Count(5, 9);
        public Count foodCount = new Count(1, 5);
        public GameObject exit;
        public GameObject[] floorTiles;
        public GameObject[] wallTiles;
        public GameObject[] foodTiles; 
        public GameObject[] outerWallTiles;
        public GameObject lightRedTiles;
        public GameObject lightBlueTiles;

        private Transform boardHolder;

        private Transform lightHolder;
        private List<Vector3> gridPosition = new List<Vector3>();
        public List<Light> lights = new List<Light>();

        public List<GameItem> floors = new List<GameItem>();
        

        public List<Light> getLightsNextTo(Light light) {
            List<Light> ls = new List<Light>();
            print(light.position);
            lights.ForEach(i => {
                if ((i.position - light.position).sqrMagnitude < 1.5
                && i.position != light.position)
                    ls.Add(i);
            });
            return ls;
        }

        private int getMovementCoast(int x, int y) {
            return GameManager.instance.getByXY(x,y).moveCoast;
        }

        public List<Light> getLightsCanGo(Vector2 source, int movement, List<Light> result) {

            List<Light> tlights = new List<Light>();
            Light s = getLightByXY((int)source.x,(int) source.y);
            if (result.Find(p => {return p.position == s.position;}) == null) {
             tlights.Add(s);
            }
            if (movement > -1) {

                getLightsNextTo(s).ForEach(l => {
                    if (!GameManager.instance.hasUnit(l.position.x, l.position.y)
                    && movement- GameManager.instance.getMovementCost(l.position.x, l.position.y)>-1) {

                        getLightsCanGo(l.Object.GetComponent<Rigidbody2D>().position, movement- getMovementCoast((int)l.position.x, (int)l.position.y), tlights)
                        .ForEach( nl => {
                            if (tlights.FindIndex(p => p.position == nl.position) == -1) 
                                tlights.Add(nl);
                            });
                    }
                }); 
            }  
            return tlights;
        }


        public GameItem getByXY(int x, int y) {
            GameItem target = null;
            floors.ForEach(floor => {
                if (floor != null  
                && floor.x == x && floor.y == y) {
                    target = floor;
                }
            });

            if (target.isDead) return null;
            return target;
        }

        // todo
        public Light getLightByXY(int x, int y) {
            Light target = null;
            lights.ForEach(i => {
                if (i != null && i.position.x == x && i.position.y == y) {
                    target = i;
                }
            });
            if (target == null) {
                throw new Exception("GetLightByXY from : " + x + "," + y + " is null");
            }
            return target;
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
            lightHolder = new GameObject("LightHolder").transform;

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
                    // add blue  lights
                    toInstantiate = lightBlueTiles;
                    instance = Instantiate(toInstantiate, new Vector3(x, y, 0f), Quaternion.identity) as GameObject;
                    g = instance.GetComponent<GameItem>();
                    if (g != null) {

                    g.x = x;
                    g.y = y;
                    } 
                    instance.transform.SetParent(lightHolder);
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

        void layoutObjectAtTeam(Team team) {
            Transform teamHolder = new GameObject(team.teamName).transform; 
            team.teamUnits.ForEach(i => {
                Vector3 position = new Vector3(i.x, i.y, 0f);
                GameObject instance = Instantiate(i.gameObject, position, Quaternion.identity);
                GameItem gameItem = instance.GetComponent<GameItem>();
                gameItem.teamColor = team.mTeamColor;
                instance.transform.SetParent(teamHolder);
                team.initialedUnits.Add(gameItem);
            });

            TeamContainor.instance.AddTeam(team);
        }

        public void SetupScene(int level)
        {
            BoardSetup();
            InitialiseList();
            layoutObjectAtRandom(wallTiles, wallCount.minimum, wallCount.maximum);
            layoutObjectAtRandom(foodTiles, foodCount.minimum, foodCount.maximum);
            int enemyCount = level;
            GameManager.instance.mTeams.ForEach(team => {

                layoutObjectAtTeam(team);
            });
            Instantiate(exit, new Vector3(columns - 1, rows - 1, 0f), Quaternion.identity);
        }

    }
}

