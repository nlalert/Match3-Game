using UnityEngine;

public class BoardManager : MonoBehaviour{
    public Fossil[,] fossils;
    public int width = 15;
    public int height = 20;
    public int hiddenRow = 5; // Hidden rows for spawning and animation
    public Fossil selectedFossil = null; 
    public Fossil[] swapPair = new Fossil[2];

    public AnimationManager animationManager;
    public FossilSpawner fossilSpawner;
    public SwapManager swapManager;
    public TimerManager timer;

    // Initialize board and start game timer and background music
    private void Start(){
        hiddenRow = height;
        InitializeBoard();
        timer.StartTimer();
        AudioManager.Instance.PlayMusic(AudioManager.Instance.stageMusic);
    }

    // Set up the board data and fill with fossils
    private void InitializeBoard(){
        fossils = new Fossil[width, height + hiddenRow];
        FillBoardWithFossils();
    }

    // Fill the board including hidden rows with fossils
    private void FillBoardWithFossils(){
        for (int x = 0; x < width; x++){
            for (int y = 0; y < height + hiddenRow; y++){
                fossilSpawner.SpawnFossil(x, y);
            }
        }
    }

    // Handle the player click on a fossil and processes selectio
    public void HandleFossilClick(Vector3 mousePos){
        if (animationManager.isAnimating) return;

        Vector2Int gridPos = GetBoardGridPosition(mousePos);

        if (!IsInBoard(gridPos.x, gridPos.y)) return;

        Fossil clickedFossil = fossils[gridPos.x, gridPos.y];
        ProcessFossilSelection(clickedFossil);
    }

    // Handle the selection and swapping of fossils
    private void ProcessFossilSelection(Fossil clickedFossil){
        if (selectedFossil == null){
            // Select the first fossil
            selectedFossil = clickedFossil;
            ScaleFossil(selectedFossil, 1.2f);
        }
        else{
            ScaleFossil(selectedFossil, 1.0f);

            // Swap with the second fossil
            swapPair[0] = selectedFossil;
            swapPair[1] = clickedFossil;
            swapManager.CheckAndSwap(swapPair[0], swapPair[1]);

            selectedFossil = null;
        }
    }

    // Change the scale of a fossil when select
    private void ScaleFossil(Fossil fossil, float scale){
        if (fossil != null){
            fossil.transform.localScale = new Vector3(scale, scale, scale);
        }
    }

    // Convert world position into grid coordinate on the board
    private Vector2Int GetBoardGridPosition(Vector3 worldPosition){
        int x = Mathf.RoundToInt(worldPosition.x + (width / 2));
        int y = Mathf.RoundToInt(worldPosition.y + (height / 2));

        return new Vector2Int(x, y);
    }

    // Check if grid position is in the board area
    public bool IsInBoard(int x, int y){
        return x >= 0 && x < width && y >= 0 && y < height;
    }

    // Destroy a fossil and removes it from the board
    public void DestroyFossil(Fossil fossil){
        fossils[fossil.x, fossil.y] = null;
        Destroy(fossil.gameObject);
    }
}
