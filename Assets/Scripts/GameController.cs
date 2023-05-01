using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

public class GameController : MonoBehaviour
{
    public AudioSource audioSource;
    public AudioClip popSound;
    public AudioClip clickSound;
    public AudioClip waaSound;
    public AudioClip bonusSound;
    public TextMeshProUGUI movesText;
    public GameObject extraMoveText;
    public GameObject tilePrefab;
    public GameObject popPrefab;
    public TextMeshProUGUI requirement1Text;
    public TextMeshProUGUI requirement2Text;
    public TextMeshProUGUI requirement3Text;
    (DinoType, int) requirement1 = (DinoType.tRex, 3);
    (DinoType, int) requirement2 = (DinoType.tRex, 3);
    (DinoType, int) requirement3 = (DinoType.tRex, 3);
    public TextMeshProUGUI totalDinosText;
    public Image requirementImage1;
    public Image requirementImage2;
    public Image requirementImage3;
    public GameObject strike1;
    public GameObject strike2;
    public GameObject strike3;
    public GameObject gameOverPanel;
    public GameObject deliverButton;
    public Sprite[] dinoSprites;
    public int[,] gameBoard;
    List<Tile> tiles = new List<Tile>();
    int movesRemaining = 3;
    int level = 0;
    int totalDinosDelivered = 0;
    int difficulty = 3;
    // Start is called before the first frame update
    void Start()
    {
        PopulateBoard();
        SetRequirements();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.M)) 
        {
            if (audioSource.volume <= 0)
            {
                audioSource.volume = 0.75f;
            }
            else
            {
                audioSource.volume = 0;
            }
        }
        if (Input.GetKeyDown(KeyCode.Q))
        {
            SceneManager.LoadScene("HomeScene");
        }
        if (Input.GetKeyDown(KeyCode.E))
        {
            difficulty = 1;
            ClearBoard();
            PopulateBoard();
            SetRequirements();
        }
        if (Input.GetKeyDown(KeyCode.F))
        {
            difficulty = 2;
            ClearBoard();
            PopulateBoard();
            SetRequirements();
        }
        if (Input.GetKeyDown(KeyCode.H))
        {
            difficulty = 3;
            ClearBoard();
            PopulateBoard();
            SetRequirements();
        }
    }

    void SetRequirements()
    {
        req1Met = false; 
        req2Met = false; 
        req3Met = false; 
        strike1.SetActive(false);
        strike2.SetActive(false);
        strike3.SetActive(false);
        int rand3 = Random.Range(0, 3);
        int req1 = rand3 == 0 ? level : 0;
        int req2 = rand3 == 1 ? level : 0;
        int req3 = rand3 == 2 ? level : 0;
        int dinoTypeCount = difficulty + 3;
        requirement1 = ((DinoType)Random.Range(0, dinoTypeCount), 3 + req1);
        requirement2 = ((DinoType)Random.Range(0, dinoTypeCount), 3 + req2);
        requirement3 = ((DinoType)Random.Range(0, dinoTypeCount), 3 + req3);

        requirement1Text.text = $"{requirement1.Item2}";
        requirement2Text.text = $"{requirement2.Item2}";
        requirement3Text.text = $"{requirement3.Item2}";
        requirementImage1.sprite = dinoSprites[(int)requirement1.Item1];
        requirementImage2.sprite = dinoSprites[(int)requirement2.Item1];
        requirementImage3.sprite = dinoSprites[(int)requirement3.Item1];
    }

    void PopulateBoard()
    {
        tiles.Clear();
        int[,] board = new int[7, 11];
        int dinoTypeCount = difficulty + 3;
        for (int i = 0; i <= 6; i++)
        {
            for (int j = 0; j <= 10; j++)
            {
                DinoType dinoType = (DinoType)Random.Range(0, dinoTypeCount);
                board[i, j] = (int)dinoType;
            }
        }
        CheckBoardForTrios(board);
    }

    void ClearBoard()
    {
        foreach (var tile in tiles)
        {
            Destroy(tile.gameObject); 
        }
    }
    void CheckBoardForTrios(int[,] board)
    {
        bool shouldRetry = MatchesInBoard(board) > 0;

        if (shouldRetry)
        {
            PopulateBoard();
        }
        else
        {
            for (int i = 0; i < board.GetLength(0); i++)
            {
                for (int j = 0; j < board.GetLength(1); j++)
                {
                    Tile tile = Instantiate(tilePrefab).GetComponent<Tile>(); 
                    tile.transform.position = new Vector2(i, j);
                    tile.SetType((DinoType)board[i, j]);
                    tile.gameController = this;
                    tile.SetXY(i, j);
                    tiles.Add(tile);
                }
            }
            gameBoard = board;
        }
    }

    int MatchesInBoard(int[,] board)
    {
        int count = 0;
        for (int i = 0; i < board.GetLength(0); i++)
        {
            for (int j = 0; j < board.GetLength(1); j++)
            {
                int current = board[i, j];
                if (i < board.GetLength(0) - 2)
                {
                    if (current == board[i + 1, j] && current == board[i + 2, j])
                    {
                        count++;
                    }
                }
                if (j < board.GetLength(1) - 2)
                {
                    if (current == board[i, j + 1] && current == board[i, j + 2])
                    {
                        count++;
                    }
                }
            }
        } 
        return count;
    }

    Tile selectedTile;
    public void TileSelected(Tile tile) 
    {
        selectedTile = tile;
    }

    Tile secondTile;
    public void DraggedOverTile(Tile tile) 
    {
        if (selectedTile == null) { return; }
        if (tile == selectedTile) { return; }
        if (secondTile != null) { return; }
        secondTile = tile;
        Vector2Int selectedPos = new Vector2Int((int)selectedTile.transform.position.x, (int)selectedTile.transform.position.y);
        Vector2Int secondPos = new Vector2Int((int)tile.transform.position.x, (int)tile.transform.position.y);
        if (Mathf.Abs(tile.transform.position.x - selectedTile.transform.position.x) <= 1 && tile.transform.position.y == selectedTile.transform.position.y)
        {
            Vector2 selectedPoss = selectedTile.transform.position;
            Vector2 currentPos = tile.transform.position;
            StartCoroutine(selectedTile.MoveToPos(currentPos, 10));
            StartCoroutine(tile.MoveToPos(selectedPoss, 10));
        }
        if (Mathf.Abs(tile.transform.position.y - selectedTile.transform.position.y) <= 1 && tile.transform.position.x == selectedTile.transform.position.x)
        {
            Vector2 selectedPoss = selectedTile.transform.position;
            Vector2 currentPos = tile.transform.position;
            StartCoroutine(selectedTile.MoveToPos(currentPos, 10));
            StartCoroutine(tile.MoveToPos(selectedPoss, 10));
        }
        selectedTile.transform.localScale = new Vector2(1, 1);

        // check if the tile switch should bounce back
        int prevMatches = MatchesInBoard(gameBoard);
        var prevTilesToRemove = TilesToRemove(gameBoard);
        gameBoard[selectedPos.x, selectedPos.y] = (int)tile.dinoType;
        gameBoard[secondPos.x, secondPos.y] = (int)selectedTile.dinoType;
        var newTilesToRemove = TilesToRemove(gameBoard);
        int newMatches = MatchesInBoard(gameBoard);
        if (newMatches > prevMatches && !selectedTile.lockedIn && !secondTile.lockedIn)
        {
            // add a move if it was a 4 or more match
            if (newTilesToRemove.Count - prevTilesToRemove.Count > 3)
            {
                movesRemaining++;
                extraMoveText.GetComponent<Animator>().Play("Show");
                audioSource.PlayOneShot(bonusSound);
            }
            audioSource.PlayOneShot(clickSound);
            selectedTile.SetXY(secondPos.x, secondPos.y);
            tile.SetXY(selectedPos.x, selectedPos.y);
            int countRex = 0;
            int countSteg = 0;
            int countTri = 0;
            int countPara = 0;
            int countBrach = 0;
            int countAnk = 0;
            foreach (var lockedTile in TilesToRemove(gameBoard))
            {
                if (lockedTile.dinoType == DinoType.tRex) { countRex++; }
                if (lockedTile.dinoType == DinoType.stegosaurus) { countSteg++; }
                if (lockedTile.dinoType == DinoType.triceratops) { countTri++; }
                if (lockedTile.dinoType == DinoType.paralophosaurus) { countPara++; }
                if (lockedTile.dinoType == DinoType.brachiosaurus) { countBrach++; }
                if (lockedTile.dinoType == DinoType.ankylosaurus) { countAnk++; }
                lockedTile.LockIn(); 
            }
            CheckRequirements(countRex, countSteg, countTri, countPara, countBrach, countAnk);
            selectedTile = null;
            secondTile = null;
            movesRemaining--;
            if (movesRemaining <= 0)
            {
                movesRemaining = 3;
                Invoke("RemoveMatches", 0.2f);
            }
            movesText.text = $"Moves: {movesRemaining}";
        }
        else
        {
            audioSource.PlayOneShot(waaSound);
            Invoke("PopBack", 0.2f);
        }
    }

    List<Tile> TilesToRemove(int[,] board)
    {
        List<Tile> tilesToRemove = new List<Tile>();
        for (int i = 0; i < board.GetLength(0); i++)
        {
            for (int j = 0; j < board.GetLength(1); j++)
            {
                int current = board[i, j];
                if (i < board.GetLength(0) - 2)
                {
                    if (current == board[i + 1, j] && current == board[i + 2, j])
                    {
                        foreach (var tile in tiles)
                        {
                            if (tile.x == i && tile.y == j ||
                                tile.x == i + 1 && tile.y == j ||
                                tile.x == i + 2 && tile.y == j)
                                {
                                    if (!tilesToRemove.Contains(tile))
                                    {
                                        tilesToRemove.Add(tile);
                                    }
                                } 
                        }
                    }
                }
                if (j < board.GetLength(1) - 2)
                {
                    if (current == board[i, j + 1] && current == board[i, j + 2])
                    {
                        foreach (var tile in tiles)
                        {
                            if (tile.x == i && tile.y == j ||
                                tile.x == i && tile.y == j + 1 ||
                                tile.x == i && tile.y == j + 2)
                                {
                                    if (!tilesToRemove.Contains(tile))
                                    {
                                        tilesToRemove.Add(tile);
                                    }
                                } 
                        }
                    }
                }
            }
        } 
        return tilesToRemove;
    }

    public void RemoveMatches()
    {
        movesRemaining = 3;
        movesText.text = $"Moves: {movesRemaining}";
        List<Tile> tilesToRemove = new List<Tile>();
        for (int i = 0; i < gameBoard.GetLength(0); i++)
        {
            for (int j = 0; j < gameBoard.GetLength(1); j++)
            {
                int current = gameBoard[i, j];
                if (i < gameBoard.GetLength(0) - 2)
                {
                    if (current == gameBoard[i + 1, j] && current == gameBoard[i + 2, j])
                    {
                        foreach (var tile in tiles)
                        {
                            if (tile.x == i && tile.y == j ||
                                tile.x == i + 1 && tile.y == j ||
                                tile.x == i + 2 && tile.y == j)
                                {
                                    if (!tilesToRemove.Contains(tile))
                                    {
                                        tilesToRemove.Add(tile);
                                    }
                                } 
                        }
                    }
                }
                if (j < gameBoard.GetLength(1) - 2)
                {
                    if (current == gameBoard[i, j + 1] && current == gameBoard[i, j + 2])
                    {
                        foreach (var tile in tiles)
                        {
                            if (tile.x == i && tile.y == j ||
                                tile.x == i && tile.y == j + 1 ||
                                tile.x == i && tile.y == j + 2)
                                {
                                    if (!tilesToRemove.Contains(tile))
                                    {
                                        tilesToRemove.Add(tile);
                                    }
                                } 
                        }
                    }
                }
            }
        }
        int countRex = 0;
        int countSteg = 0;
        int countTri = 0;
        int countPara = 0;
        int countBrach = 0;
        int countAnk = 0;
        foreach (var tile in tilesToRemove)
        {
            gameBoard[tile.x, tile.y] = -1;
            tiles.Remove(tile);
            Destroy(tile.gameObject); 
            GameObject pop = Instantiate(popPrefab, tile.transform.position, Quaternion.identity);
            var main = pop.GetComponent<ParticleSystem>().main;
            main.startColor = tile.ColorForDinoType(tile.dinoType);
            Destroy(pop.gameObject, 1);
            totalDinosDelivered++;
            totalDinosText.text = $"Total \nDelivered: {totalDinosDelivered}";
            if (tile.dinoType == DinoType.tRex) { countRex++; }
            if (tile.dinoType == DinoType.stegosaurus) { countSteg++; }
            if (tile.dinoType == DinoType.triceratops) { countTri++; }
            if (tile.dinoType == DinoType.paralophosaurus) { countPara++; }
            if (tile.dinoType == DinoType.brachiosaurus) { countBrach++; }
            if (tile.dinoType == DinoType.ankylosaurus) { countAnk++; }
        }
        CheckRequirements(countRex, countSteg, countTri, countPara, countBrach, countAnk);
        if (tilesToRemove.Count > 0)
        {
            audioSource.PlayOneShot(popSound);
            Invoke("SlideTilesDown", 0.2f);
        }
        else
        {
            if (Random.Range(0, 3) == 0)
            {
                level++;
            }
            if (req1Met && req2Met && req3Met)
            {
                SetRequirements();
            }
            else
            {
                GameOver();
            }
        }
    }

    void GameOver()
    {
        strike1.SetActive(false);
        strike2.SetActive(false);
        strike3.SetActive(false);
        movesText.gameObject.SetActive(false);
        deliverButton.SetActive(false);
        requirement1Text.gameObject.SetActive(false);
        requirement2Text.gameObject.SetActive(false);
        requirement3Text.gameObject.SetActive(false);
        requirementImage1.gameObject.SetActive(false);
        requirementImage2.gameObject.SetActive(false);
        requirementImage3.gameObject.SetActive(false);
        gameOverPanel.SetActive(true);
    }

    public void Restart()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    bool req1Met = false;
    bool req2Met = false;
    bool req3Met = false;
    void CheckRequirements(int countRex, int countSteg, int countTri, int countPara, int countBrach, int countAnk)
    {
        if (
            (requirement1.Item1 == DinoType.tRex && requirement1.Item2 <= countRex) ||
            (requirement1.Item1 == DinoType.triceratops && requirement1.Item2 <= countTri) ||
            (requirement1.Item1 == DinoType.stegosaurus && requirement1.Item2 <= countSteg) ||
            (requirement1.Item1 == DinoType.paralophosaurus && requirement1.Item2 <= countPara) ||
            (requirement1.Item1 == DinoType.brachiosaurus && requirement1.Item2 <= countBrach) ||
            (requirement1.Item1 == DinoType.ankylosaurus && requirement1.Item2 <= countAnk)
            )
        {
            req1Met = true;
            strike1.SetActive(true);
        }
        if (
            (requirement2.Item1 == DinoType.tRex && requirement2.Item2 <= countRex) ||
            (requirement2.Item1 == DinoType.triceratops && requirement2.Item2 <= countTri) ||
            (requirement2.Item1 == DinoType.stegosaurus && requirement2.Item2 <= countSteg) ||
            (requirement2.Item1 == DinoType.paralophosaurus && requirement2.Item2 <= countPara) ||
            (requirement2.Item1 == DinoType.brachiosaurus && requirement2.Item2 <= countBrach) ||
            (requirement2.Item1 == DinoType.ankylosaurus && requirement2.Item2 <= countAnk)
            )
        {
            req2Met = true;
            strike2.SetActive(true);
        }
        if (
            (requirement3.Item1 == DinoType.tRex && requirement3.Item2 <= countRex) ||
            (requirement3.Item1 == DinoType.triceratops && requirement3.Item2 <= countTri) ||
            (requirement3.Item1 == DinoType.stegosaurus && requirement3.Item2 <= countSteg) ||
            (requirement3.Item1 == DinoType.paralophosaurus && requirement3.Item2 <= countPara) ||
            (requirement3.Item1 == DinoType.brachiosaurus && requirement3.Item2 <= countBrach) ||
            (requirement3.Item1 == DinoType.ankylosaurus && requirement3.Item2 <= countAnk)
            )
        {
            req3Met = true;
            strike3.SetActive(true);
        }
    }

    void SlideTilesDown()
    {
        for (int i = 0; i < gameBoard.GetLength(0); i++)
        {
            for (int j = 0; j < gameBoard.GetLength(1); j++)
            {
                int current = gameBoard[i, j];
                Tile currentTile = tiles.Find(tile => tile.x == i && tile.y == j);
                if (current != -1)
                {
                    for (int k = 0; k <= j; k++)
                    {
                        if (gameBoard[i, j - k] == -1)
                        {
                            // set current to blank
                            gameBoard[i, j - k + 1] = -1;
                            // move current down by one
                            currentTile.SetXY(i, j - k);
                            gameBoard[i, j - k] = current;
                        }
                    }
                }
            }
        }
        AddNewTiles();
        foreach (var tile in tiles)
        {
            StartCoroutine(tile.MoveToPos(new Vector2((float)tile.x, (float)tile.y), 10)); 
        }
        Invoke("RemoveMatches", 0.8f);
    }

    void AddNewTiles()
    {
        int dinoTypeCount = difficulty + 3;
        Dictionary<int, int> newDict = new Dictionary<int, int>();
        for (int i = 0; i < gameBoard.GetLength(0); i++)
        {
            for (int j = 0; j < gameBoard.GetLength(1); j++)
            {
                int current = gameBoard[i, j];
                if (current == -1)
                {
                    if (newDict.ContainsKey(i))
                    {
                        newDict[i]++;
                    }
                    else
                    {
                        newDict[i] = 1;
                    }
                    DinoType dinoType = (DinoType)Random.Range(0, dinoTypeCount);
                    gameBoard[i, j] = (int)dinoType;
                    Tile tile = Instantiate(tilePrefab).GetComponent<Tile>();
                    tile.transform.position = new Vector2(i, 10 + newDict[i]);
                    tile.SetType(dinoType);
                    tile.gameController = this;
                    tile.SetXY(i, j);
                    tiles.Add(tile);
                }
            }
        }
    }

    void PopBack()
    {
        Vector2Int selectedPos = new Vector2Int((int)selectedTile.transform.position.x, (int)selectedTile.transform.position.y);
        Vector2Int secondPos = new Vector2Int((int)secondTile.transform.position.x, (int)secondTile.transform.position.y);
        Vector2 selectedPoss = selectedTile.transform.position;
        Vector2 currentPos = secondTile.transform.position;
        // selectedTile.transform.position = currentPos;
        // secondTile.transform.position = selectedPoss;
        StartCoroutine(selectedTile.MoveToPos(currentPos, 10));
        StartCoroutine(secondTile.MoveToPos(selectedPoss, 10));
        gameBoard[selectedPos.x, selectedPos.y] = (int)secondTile.dinoType;
        gameBoard[secondPos.x, secondPos.y] = (int)selectedTile.dinoType;
        selectedTile = null;
        secondTile = null;
    }
}
