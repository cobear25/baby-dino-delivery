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
    int maxMoves = 3;
    int movesRemaining = 3;
    int level = 0;
    int internalLevel = 0;
    int totalDinosDelivered = 0;
    int difficulty = 3;
    bool canMove = true;
    // Start is called before the first frame update
    void Start()
    {
        if (PlayerPrefs.GetInt("muted") == 1)
        {
            audioSource.volume = 0.0f;
        }
        difficulty = PlayerPrefs.GetInt("difficulty", 3);
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
                PlayerPrefs.SetInt("muted", 0);
            }
            else
            {
                audioSource.volume = 0;
                PlayerPrefs.SetInt("muted", 1);
            }
        }
        if (Input.GetKeyDown(KeyCode.Q))
        {
            SceneManager.LoadScene("HomeScene");
        }
        if (Input.GetKeyDown(KeyCode.E))
        {
            totalDinosDelivered = 0;
            totalDinosText.text = $"Total \nDelivered: 0";
            internalLevel = 0;
            maxMoves = 3;
            level = 0;
            difficulty = 1;
            PlayerPrefs.SetInt("difficulty", difficulty);
            ClearBoard();
            PopulateBoard();
            SetRequirements();
        }
        if (Input.GetKeyDown(KeyCode.F))
        {
            totalDinosDelivered = 0;
            totalDinosText.text = $"Total \nDelivered: 0";
            internalLevel = 0;
            maxMoves = 3;
            level = 0;
            difficulty = 2;
            PlayerPrefs.SetInt("difficulty", difficulty);
            ClearBoard();
            PopulateBoard();
            SetRequirements();
        }
        if (Input.GetKeyDown(KeyCode.H))
        {
            totalDinosDelivered = 0;
            totalDinosText.text = $"Total \nDelivered: 0";
            internalLevel = 0;
            maxMoves = 3;
            level = 0;
            difficulty = 3;
            PlayerPrefs.SetInt("difficulty", difficulty);
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
        int req1 = 0;
        int req2 = 0;
        int req3 = 0;

        int dinoTypeCount = difficulty + 3;
        // if the level is > 3, distribute the extra requirements across all three requirements
        if (level > 3)
        {
            for (int i = 0; i < level; i++)
            {
                int rand3 = Random.Range(0, 3);
                if (rand3 == 0)
                {
                    req1 += 1;
                }
                if (rand3 == 1)
                {
                    req2 += 1;
                }
                if (rand3 == 2)
                {
                    req3 += 1;
                }
            }
        }
        else
        {
            int rand3 = Random.Range(0, 3);
            req1 = rand3 == 0 ? level : 0;
            req2 = rand3 == 1 ? level : 0;
            req3 = rand3 == 2 ? level : 0;
        }

        requirement1 = ((DinoType)Random.Range(0, dinoTypeCount), 3 + req1);
        requirement2 = (requirement1.Item1, requirement1.Item2);
        requirement3 = (requirement1.Item1, requirement1.Item2);

        // make sure the same type isn't used
        while (requirement2.Item1 == requirement1.Item1)
        {
            requirement2 = ((DinoType)Random.Range(0, dinoTypeCount), 3 + req2);
        }
        while (requirement3.Item1 == requirement1.Item1 || requirement3.Item1 == requirement2.Item1)
        {
            requirement3 = ((DinoType)Random.Range(0, dinoTypeCount), 3 + req3);
        }

        requirement1Text.text = $"{requirement1.Item2}";
        requirement2Text.text = $"{requirement2.Item2}";
        requirement3Text.text = $"{requirement3.Item2}";
        requirementImage1.sprite = dinoSprites[(int)requirement1.Item1];
        requirementImage2.sprite = dinoSprites[(int)requirement2.Item1];
        requirementImage3.sprite = dinoSprites[(int)requirement3.Item1];
        canMove = true;

        // highlight the number that has a higher requirement
        requirement1Text.color = Color.white;
        requirement2Text.color = Color.white;
        requirement3Text.color = Color.white;
        if (req1 > 0) {
            requirement1Text.color = Color.red;
        }
        if (req2 > 0) {
            requirement2Text.color = Color.red;
        }
        if (req3 > 0) {
            requirement3Text.color = Color.red;
        }
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
        if (!canMove) { return; }
        selectedTile = tile;
    }

    Tile secondTile;
    public void DraggedOverTile(Tile tile) 
    {
        if (!canMove) { return; }
        if (selectedTile == null) { return; }
        if (tile == selectedTile) { return; }
        if (secondTile != null) { return; }
        secondTile = tile;
        Vector2Int selectedPos = new Vector2Int((int)selectedTile.transform.position.x, (int)selectedTile.transform.position.y);
        Vector2Int secondPos = new Vector2Int((int)tile.transform.position.x, (int)tile.transform.position.y);
        bool didSwap = false;
        if (Mathf.Abs(tile.transform.position.x - selectedTile.transform.position.x) <= 1 && tile.transform.position.y == selectedTile.transform.position.y)
        {
            Vector2 selectedPoss = selectedTile.transform.position;
            Vector2 currentPos = tile.transform.position;
            StartCoroutine(selectedTile.MoveToPos(currentPos, 10));
            StartCoroutine(tile.MoveToPos(selectedPoss, 10));
            didSwap = true;
        }
        if (Mathf.Abs(tile.transform.position.y - selectedTile.transform.position.y) <= 1 && tile.transform.position.x == selectedTile.transform.position.x)
        {
            Vector2 selectedPoss = selectedTile.transform.position;
            Vector2 currentPos = tile.transform.position;
            StartCoroutine(selectedTile.MoveToPos(currentPos, 10));
            StartCoroutine(tile.MoveToPos(selectedPoss, 10));
            didSwap = true;
        }
        selectedTile.transform.localScale = new Vector2(1, 1);

        if (!didSwap) { 
            selectedTile = null;
            secondTile = null;
            return; 
        }

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
            if (movesRemaining <= 0 || (req1Met && req2Met && req3Met))
            {
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
        canMove = false;
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
        requirement1.Item2 = req1Remaining;
        requirement2.Item2 = req2Remaining;
        requirement3.Item2 = req3Remaining;
        if (tilesToRemove.Count > 0)
        {
            audioSource.PlayOneShot(popSound);
            Invoke("SlideTilesDown", 0.2f);
        }
        else
        {
            if (difficulty == 1)
            {
                // maxMoves = 3 + (totalDinosDelivered / 300);
            }
            if (difficulty == 2)
            {
                maxMoves = 3 + (totalDinosDelivered / 200);
            }
            if (difficulty == 3)
            {
                maxMoves = 3 + (totalDinosDelivered / 150);
            }
            if (movesRemaining <= 0)
            {
                movesRemaining = maxMoves;
                movesText.text = $"Moves: {movesRemaining}";
                internalLevel++;
                if (internalLevel % difficulty == 0)
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
            else
            {
                if (req1Met && req2Met && req3Met)
                {
                    movesRemaining = maxMoves;
                    movesText.text = $"Moves: {movesRemaining}";
                    internalLevel++;
                    if (internalLevel % difficulty == 0)
                    {
                        level++;
                    }
                    SetRequirements();
                }
                canMove = true;
            }
        }
    }

    void GameOver()
    {
        movesText.gameObject.SetActive(false);
        deliverButton.SetActive(false);
        gameOverPanel.SetActive(true);
    }

    public void Restart()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    bool req1Met = false;
    bool req2Met = false;
    bool req3Met = false;
    int req1Remaining = 100;
    int req2Remaining = 100;
    int req3Remaining = 100;
    void CheckRequirements(int countRex, int countSteg, int countTri, int countPara, int countBrach, int countAnk)
    {
        req1Remaining = requirement1.Item2;
        req2Remaining = requirement2.Item2;
        req3Remaining = requirement3.Item2;
        if (requirement1.Item1 == DinoType.tRex)
        {
            req1Remaining -= countRex;
        }
        if (requirement2.Item1 == DinoType.tRex)
        {
            req2Remaining -= countRex;
        }
        if (requirement3.Item1 == DinoType.tRex)
        {
            req3Remaining -= countRex;
        }

        if (requirement1.Item1 == DinoType.stegosaurus)
        {
            req1Remaining -= countSteg;
        }
        if (requirement2.Item1 == DinoType.stegosaurus)
        {
            req2Remaining -= countSteg;
        }
        if (requirement3.Item1 == DinoType.stegosaurus)
        {
            req3Remaining -= countSteg;
        }
        
        if (requirement1.Item1 == DinoType.triceratops)
        {
            req1Remaining -= countTri;
        }
        if (requirement2.Item1 == DinoType.triceratops)
        {
            req2Remaining -= countTri;
        }
        if (requirement3.Item1 == DinoType.triceratops)
        {
            req3Remaining -= countTri;
        }
        
        if (requirement1.Item1 == DinoType.paralophosaurus)
        {
            req1Remaining -= countPara;
        }
        if (requirement2.Item1 == DinoType.paralophosaurus)
        {
            req2Remaining -= countPara;
        }
        if (requirement3.Item1 == DinoType.paralophosaurus)
        {
            req3Remaining -= countPara;
        }
        
        if (requirement1.Item1 == DinoType.brachiosaurus)
        {
            req1Remaining -= countBrach;
        }
        if (requirement2.Item1 == DinoType.brachiosaurus)
        {
            req2Remaining -= countBrach;
        }
        if (requirement3.Item1 == DinoType.brachiosaurus)
        {
            req3Remaining -= countBrach;
        }
        
        if (requirement1.Item1 == DinoType.ankylosaurus)
        {
            req1Remaining -= countAnk;
        }
        if (requirement2.Item1 == DinoType.ankylosaurus)
        {
            req2Remaining -= countAnk;
        }
        if (requirement3.Item1 == DinoType.ankylosaurus)
        {
            req3Remaining -= countAnk;
        }

        requirement1Text.text = $"{Mathf.Max(0, req1Remaining)}";
        requirement2Text.text = $"{Mathf.Max(0, req2Remaining)}";
        requirement3Text.text = $"{Mathf.Max(0, req3Remaining)}";
        if (req1Remaining <= 0)
        {
            req1Met = true;
            strike1.SetActive(true);
        }
        if (req2Remaining <= 0)
        {
            req2Met = true;
            strike2.SetActive(true);
        }
        if (req3Remaining <= 0)
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
            StartCoroutine(tile.MoveToPos(new Vector2((float)tile.x, (float)tile.y), 12.5f)); 
        }
        Invoke("RemoveMatches", 0.7f);
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
