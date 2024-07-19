using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine.SceneManagement;

public class MemoryGame : MonoBehaviour
{
    public GameObject[] quads = new GameObject[16];
    public GameObject[] cubes = new GameObject[16];
    public Material[] materials = new Material[8];
    public OVRCameraRig cameraRig; 
    public TextMeshProUGUI scoreText;
    public TextMeshProUGUI matchesText;
    public TextMeshProUGUI attemptsText;
    public TextMeshProUGUI timeText;
    public TextMeshProUGUI endGameScoreText; // Reference to the score text in the end game menu
    public GameObject gameFinishedMenu; // Reference to the game finished menu

    private GameObject firstSelectedQuad = null;
    private GameObject secondSelectedQuad = null;
    private IEnumerator coroutine;
    private bool disable = false;
    private HashSet<GameObject> matchedCubes = new HashSet<GameObject>();
    private int score = 0;
    private int matchesFound = 0;
    private int attempts = 0;
    private float timeElapsed = 0f;
    private bool gameWon = false;

    void Start()
    {
        AssignMaterials();
    }

    void Update()
    {
        if (!gameWon)
        {
            timeElapsed += Time.deltaTime;
            UpdateStatsDisplay();
        }
    }


     public void CubeSelected(GameObject hitted) 
    {
        if (!disable)
        {
            GameObject hitCube = hitted;
            GameObject selectedCube = IsCube(hitCube);

            if (selectedCube != null && !matchedCubes.Contains(selectedCube))
            {
                // Check if the selected cube is the same as the first selected cube
                if (selectedCube == firstSelectedQuad)
                {
                    return; // Early return to prevent selecting the same cube twice
                }

                selectedCube.transform.Rotate(0, 180, 0);
                GameObject selectedQuad = selectedCube.transform.GetChild(0).gameObject;

                if (firstSelectedQuad == null)
                {
                    firstSelectedQuad = selectedCube;
                }
                else if (secondSelectedQuad == null)
                {
                    secondSelectedQuad = selectedCube;
                    CheckPair();
                }
            }
        }
    }

    GameObject IsCube(GameObject obj)
    {
        foreach (GameObject cube in cubes)
        {
            if (cube == obj || obj.transform.IsChildOf(cube.transform) && !matchedCubes.Contains(cube))
            {
                return cube;
            }
        }
        return null;
    }

    void AssignMaterials()
    {
        List<Material> pairedMaterials = new List<Material>();
        foreach (var mat in materials)
        {
            pairedMaterials.Add(mat);
            pairedMaterials.Add(mat);
        }

        for (int i = 0; i < pairedMaterials.Count; i++)
        {
            Material temp = pairedMaterials[i];
            int randomIndex = Random.Range(i, pairedMaterials.Count);
            pairedMaterials[i] = pairedMaterials[randomIndex];
            pairedMaterials[randomIndex] = temp;
        }

        for (int i = 0; i < quads.Length; i++)
        {
            if (quads[i] != null)
            {
                Renderer quadRenderer = quads[i].GetComponent<Renderer>();
                if (quadRenderer != null)
                {
                    quadRenderer.material = pairedMaterials[i];
                }
            }
        }
    }

    void CheckPair()
    {
        if (firstSelectedQuad != null && secondSelectedQuad != null)
        {
            disable = true;
            attempts++;
            Renderer firstRenderer = firstSelectedQuad.GetComponentsInChildren<Renderer>()[1];
            Renderer secondRenderer = secondSelectedQuad.GetComponentsInChildren<Renderer>()[1];

            if (firstRenderer.material.name == secondRenderer.material.name)
            {
                Debug.Log("It's a match!");
                matchesFound++;
                AddPoints(100);
                matchedCubes.Add(firstSelectedQuad);
                matchedCubes.Add(secondSelectedQuad);
                
                firstSelectedQuad = null;
                secondSelectedQuad = null;
                disable = false;
                CheckForWin();
            }
            else
            {
                Debug.Log("Not a match.");
                AddPoints(-10);
                coroutine = ResetCubes(1);
                StartCoroutine(coroutine);
            }
        }
    }

    private IEnumerator ResetCubes(float waitTime)
    {
        yield return new WaitForSeconds(waitTime);

        firstSelectedQuad.transform.Rotate(0, 180, 0);
        secondSelectedQuad.transform.Rotate(0, 180, 0);

        firstSelectedQuad = null;
        secondSelectedQuad = null;

        disable = false;
    }

    void UpdateStatsDisplay()
    {
        scoreText.text = "REZULTAT: " + score.ToString();
        matchesText.text = "PARI: " + matchesFound.ToString();
        attemptsText.text = "POSKUSI: " + attempts.ToString();
        timeText.text = timeElapsed.ToString("F2");
    }

    void CheckForWin()
    {
        if (matchesFound == quads.Length / 2)
        {
            WinGame();
        }
    }

    void WinGame()
    {
        gameWon = true;
        disable = true;

        scoreText.text = score.ToString();
        timeText.text = timeElapsed.ToString("F2");

        if (gameFinishedMenu != null)
        {
            // Update the score text in the end game menu
            if (endGameScoreText != null)
            {
                endGameScoreText.text = "Igra zakljucena. Rezultat: " + score.ToString();
            }

            gameFinishedMenu.SetActive(true);
            //controllerManager.PositionMenuInFrontOfPlayer(gameFinishedMenu);
        }
    }

    void AddPoints(int points)
    {
        score += points;
        if (score < 0)
        {
            score = 0;
        }
        UpdateStatsDisplay();
    }

    public void RestartGame()
    {
        SceneManager.LoadScene("Memory");
    }
}
