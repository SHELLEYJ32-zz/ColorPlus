using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameController : MonoBehaviour
{
    public GameObject cubePrefab, nextCubePrefab;
    public Text scoreText;
    GameObject[,] grid;
    GameObject nextCube;
    float gameLength = 60;
    int gridCol = 8;
    int gridRow = 5;
    float turnLength = 2.0f; //f for float with decimal
    int turn = 1;
    Color32[] cubeColors;
    int score = 0;
    GameObject activeCube;
    int rainbowPoints = 5;
    int sameColorPoints = 10;


    // Use this for initialization
    void Start()
    {
        CreateGrid();
        addColors();
        GenerateRandomNextCube();
        print(turn);
    }

    // Update is called once per frame
    void Update()
    {
        if (Time.time <= gameLength)
        {
            CheckKeyboardInput();
            Score();

            //update player's turn every turnLength seconds
            if (Time.time > turnLength * turn)
            {
                turn++;

                //if 1-5 is not pressed in time
                if (nextCube != null)
                {
                    if (score > 0)
                        score -= 1;
                    AddBlackCube();
                    Destroy(nextCube);
                    nextCube = null;
                    GenerateRandomNextCube();

                }
                else
                    GenerateRandomNextCube();
            }
        }
        else
        {
            if (score > 0)
                EndGame(true);
            else
                EndGame(false);
        }

    }
    //create grid with gridRow and gridCol
    void CreateGrid()
    {
        grid = new GameObject[gridRow, gridCol];

        for (int row = 0; row < gridRow; row++)
        {
            for (int col = 0; col < gridCol; col++)
            {

                Vector3 cubePos = new Vector3(col * 2, row * 2, 0);
                grid[row, col] = Instantiate(cubePrefab, cubePos, Quaternion.identity);
                grid[row, col].GetComponent<CubeController>().myRow = row;
                grid[row, col].GetComponent<CubeController>().myCol = col;
            }
        }

    }

    void addColors()
    {
        cubeColors = new Color32[5];
        cubeColors[0] = new Color32(226, 50, 0, 255);
        cubeColors[1] = new Color32(42, 104, 198, 255);
        cubeColors[2] = new Color32(72, 183, 92, 255);
        cubeColors[3] = new Color32(181, 80, 148, 255);
        cubeColors[4] = new Color32(239, 205, 23, 255);
    }

    //make a new cube with a random color appear
    void GenerateRandomNextCube()
    {
        //make a cube in the Next Cube area
        Vector3 nextCubePos = new Vector3(7, 10.7f, 0);
        nextCube = Instantiate(nextCubePrefab, nextCubePos, Quaternion.identity);
        //generate a random color
        nextCube.GetComponent<Renderer>().material.color = cubeColors[Random.Range(0, cubeColors.Length)];
    }

    //find an open column with a specific row
    GameObject FindOpenColumn(int row)
    {
        List<GameObject> whiteCubes = new List<GameObject>();

        for (int col = 0; col < gridCol; col++)
        {
            if (grid[row, col].GetComponent<Renderer>().material.color == Color.white)
                whiteCubes.Add(grid[row, col]);
        }

        if (whiteCubes.Count == 0)
            return null;
        else
            return whiteCubes[Random.Range(0, whiteCubes.Count)];
    }

    GameObject FindOpenCube()
    {
        List<GameObject> whiteCubes = new List<GameObject>();

        for (int row = 0; row < gridRow; row++)
        {
            for (int col = 0; col < gridCol; col++)
            {
                if (grid[row, col].GetComponent<Renderer>().material.color == Color.white)
                    whiteCubes.Add(grid[row, col]);
            }
        }

        if (whiteCubes.Count == 0)
            return null;
        else
            return whiteCubes[Random.Range(0, whiteCubes.Count)];
    }

    void PlaceNextCube(int row)
    {
        //if there is no open column, end game with a loss
        if (FindOpenColumn(row) == null)
            EndGame(false);
        else
        {
            //set one cube color to the same as next cube
            FindOpenColumn(row).GetComponent<Renderer>().material.color = nextCube.GetComponent<Renderer>().material.color;
            //remove next cube
            Destroy(nextCube);
            nextCube = null;
        }
    }

    void AddBlackCube()
    {
        //find a random and open cube
        if (FindOpenCube() == null)
            EndGame(false);
        else
            //set to black
            FindOpenCube().GetComponent<Renderer>().material.color = Color.black;

    }

    bool CheckIfAllBlack()
    {
        for (int row = 0; row < gridRow; row++)
        {
            for (int col = 0; col < gridCol; col++)
            {
                if (grid[row, col].GetComponent<Renderer>().material.color != Color.black)
                    return false;
            }
        }
        return true;

    }

    void EndGame(bool win)
    {
        if (win)
        {
            SceneManager.LoadScene("WinScene", LoadSceneMode.Single);
        }
        else
        {
            SceneManager.LoadScene("LoseScene", LoadSceneMode.Single);
        }
    }

    //process keyboard input every frame
    void CheckKeyboardInput()
    {
        int keyPressed = -1;

        //check if 1-5 is pressed
        if (Input.GetKeyDown(KeyCode.Alpha1) || Input.GetKeyDown(KeyCode.Keypad1))
            keyPressed = 1;
        else if (Input.GetKeyDown(KeyCode.Alpha2) || Input.GetKeyDown(KeyCode.Keypad2))
            keyPressed = 2;
        else if (Input.GetKeyDown(KeyCode.Alpha3) || Input.GetKeyDown(KeyCode.Keypad3))
            keyPressed = 3;
        else if (Input.GetKeyDown(KeyCode.Alpha4) || Input.GetKeyDown(KeyCode.Keypad4))
            keyPressed = 4;
        else if (Input.GetKeyDown(KeyCode.Alpha5) || Input.GetKeyDown(KeyCode.Keypad5))
            keyPressed = 5;

        //if next cube exists, place it in the specified row
        if (nextCube != null && keyPressed != -1)
            //minus 1 because the row starts from 0
            PlaceNextCube(keyPressed - 1);
    }

    //process mouse clicks on a specific cube
    public void ProcessMouseClick(GameObject clickedCube, int row, int col, Color color)
    {
        //if cube is colored
        if (color != Color.white && color != Color.black)
        {
            if (activeCube != null)
            {
                if (clickedCube.GetComponent<CubeController>().activated)
                {
                    clickedCube.GetComponent<CubeController>().activated = false;
                    clickedCube.transform.localScale /= 1.5f;
                    activeCube = null;
                }
                else
                {
                    activeCube.GetComponent<CubeController>().activated = false;
                    clickedCube.GetComponent<CubeController>().activated = true;
                    activeCube.transform.localScale /= 1.5f;
                    clickedCube.transform.localScale *= 1.5f;
                    activeCube = clickedCube;
                }
            }
            else
            {
                clickedCube.GetComponent<CubeController>().activated = true;
                clickedCube.transform.localScale *= 1.5f;
                activeCube = clickedCube;
            }
        }
        //if cube is white
        else if (color == Color.white && activeCube != null)
        {
            //check activated cubes around it
            if (CheckAdjacentActivateCube(row, col))
            {
                //swap colors if found
                clickedCube.GetComponent<Renderer>().material.color = activeCube.GetComponent<Renderer>().material.color;
                clickedCube.transform.localScale *= 1.5f;
                activeCube.GetComponent<Renderer>().material.color = Color.white;
                activeCube.transform.localScale /= 1.5f;
                activeCube.GetComponent<CubeController>().activated = false;
                activeCube = clickedCube;
                clickedCube.GetComponent<CubeController>().activated = true;
            }

        }
    }

    //check if there is an activated cube adjacent to the given cube
    bool CheckAdjacentActivateCube(int row, int col)
    {
        int rowGap = activeCube.GetComponent<CubeController>().myRow - row;
        int colGap = activeCube.GetComponent<CubeController>().myCol - col;
        if (rowGap >= -1 && rowGap <= 1 && colGap >= -1 && colGap <= 1)
            return true;
        else
            return false;
    }


    void Score()
    {
        for (int row = 1; row < gridRow - 1; row++)
        {
            for (int col = 1; col < gridCol - 1; col++)
            {
                if (IsRainbowPlus(row, col))
                {
                    score += rainbowPoints;
                    MakePlusBlack(row, col);
                    DeactivatePlus(row, col);
                }

                if (IsSameColorPlus(row, col))
                {
                    score += sameColorPoints;
                    MakePlusBlack(row, col);
                    DeactivatePlus(row, col);
                }
            }
        }
        scoreText.text = "Score: " + score;
    }

    bool IsRainbowPlus(int row, int col)
    {
        List<Color32> plusColors = new List<Color32>();

        if (grid[row - 1, col].GetComponent<Renderer>().material.color != Color.black &&
        grid[row - 1, col].GetComponent<Renderer>().material.color != Color.white)
        {
            if (!plusColors.Contains(grid[row - 1, col].GetComponent<Renderer>().material.color))
                plusColors.Add(grid[row - 1, col].GetComponent<Renderer>().material.color);
            else
                return false;
        }
        else
            return false;

        if (grid[row + 1, col].GetComponent<Renderer>().material.color != Color.black &&
        grid[row + 1, col].GetComponent<Renderer>().material.color != Color.white)
        {
            if (!plusColors.Contains(grid[row + 1, col].GetComponent<Renderer>().material.color))
                plusColors.Add(grid[row + 1, col].GetComponent<Renderer>().material.color);
            else
                return false;
        }
        else
            return false;

        if (grid[row, col].GetComponent<Renderer>().material.color != Color.black &&
        grid[row, col].GetComponent<Renderer>().material.color != Color.white)
        {
            if (!plusColors.Contains(grid[row, col].GetComponent<Renderer>().material.color))
                plusColors.Add(grid[row, col].GetComponent<Renderer>().material.color);
            else
                return false;
        }
        else
            return false;

        if (grid[row, col - 1].GetComponent<Renderer>().material.color != Color.black &&
        grid[row, col - 1].GetComponent<Renderer>().material.color != Color.white)
        {
            if (!plusColors.Contains(grid[row, col - 1].GetComponent<Renderer>().material.color))
                plusColors.Add(grid[row, col - 1].GetComponent<Renderer>().material.color);
            else
                return false;
        }

        if (grid[row, col + 1].GetComponent<Renderer>().material.color != Color.black &&
        grid[row, col + 1].GetComponent<Renderer>().material.color != Color.white)
        {
            if (!plusColors.Contains(grid[row, col + 1].GetComponent<Renderer>().material.color))
                plusColors.Add(grid[row, col + 1].GetComponent<Renderer>().material.color);
            else
                return false;
        }
        else
            return false;

        if (plusColors.Count == cubeColors.Length)
            return true;
        else
            return false;
    }

    bool IsSameColorPlus(int row, int col)
    {
        Color plusColor = grid[row, col].GetComponent<Renderer>().material.color;
        if (plusColor != Color.white && plusColor != Color.black)
        {
            if (grid[row - 1, col].GetComponent<Renderer>().material.color != plusColor ||
            grid[row + 1, col].GetComponent<Renderer>().material.color != plusColor ||
            grid[row, col - 1].GetComponent<Renderer>().material.color != plusColor ||
                  grid[row, col + 1].GetComponent<Renderer>().material.color != plusColor)
                return false;
        }
        else
            return false;
        return true;
    }

    void MakePlusBlack(int row, int col)
    {
        grid[row - 1, col].GetComponent<Renderer>().material.color = Color.black;
        grid[row + 1, col].GetComponent<Renderer>().material.color = Color.black;
        grid[row, col].GetComponent<Renderer>().material.color = Color.black;
        grid[row, col - 1].GetComponent<Renderer>().material.color = Color.black;
        grid[row, col + 1].GetComponent<Renderer>().material.color = Color.black;
    }

    void DeactivatePlus(int row, int col)
    {
        if (activeCube != null)
        {
            if (grid[row - 1, col].GetComponent<CubeController>().activated)
                grid[row - 1, col].GetComponent<CubeController>().activated = false;
            else if (grid[row + 1, col].GetComponent<CubeController>().activated)
                grid[row + 1, col].GetComponent<CubeController>().activated = false;
            else if (grid[row, col].GetComponent<CubeController>().activated)
                grid[row, col].GetComponent<CubeController>().activated = false;
            else if (grid[row, col - 1].GetComponent<CubeController>().activated)
                grid[row, col - 1].GetComponent<CubeController>().activated = false;
            else if (grid[row, col + 1].GetComponent<CubeController>().activated)
                grid[row, col + 1].GetComponent<CubeController>().activated = false;

            activeCube.transform.localScale /= 1.5f;
            activeCube = null;
        }
    }
}
