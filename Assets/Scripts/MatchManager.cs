using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MatchManager : MonoBehaviour{

    public BoardManager board;

    public List<Candy> GetMatches(Candy candy) {
        HashSet<Candy> matchedCandies = new HashSet<Candy>();

        // Get horizontal matches
        List<Candy> horizontalMatches = GetHorizontalMatches(candy);
        if (horizontalMatches.Count >= 3) {
            foreach (Candy c in horizontalMatches) {
                matchedCandies.Add(c);
            }
        }

        // Get vertical matches
        List<Candy> verticalMatches = GetVerticalMatches(candy);
        if (verticalMatches.Count >= 3) {
            foreach (Candy c in verticalMatches) {
                matchedCandies.Add(c);
            }
        }

        // Return the combined list of matches
        return matchedCandies.Count >= 3 ? new List<Candy>(matchedCandies) : null;
    }

    private List<Candy> GetHorizontalMatches(Candy candy) {
        List<Candy> matches = new List<Candy> { candy };

        // Check left
        for (int i = candy.x - 1; i >= 0; i--) {
            if (board.candies[i, candy.y] != null && board.candies[i, candy.y].type == candy.type) {
                matches.Add(board.candies[i, candy.y]);
            } else {
                break;
            }
        }

        // Check right
        for (int i = candy.x + 1; i < board.width; i++) {
            if (board.candies[i, candy.y] != null && board.candies[i, candy.y].type == candy.type) {
                matches.Add(board.candies[i, candy.y]);
            } else {
                break;
            }
        }

        return matches;
    }

    private List<Candy> GetVerticalMatches(Candy candy) {
        List<Candy> matches = new List<Candy> { candy };

        // Check down
        for (int i = candy.y - 1; i >= 0; i--) {
            if (board.candies[candy.x, i] != null && board.candies[candy.x, i].type == candy.type) {
                matches.Add(board.candies[candy.x, i]);
            } else {
                break;
            }
        }

        // Check up
        for (int i = candy.y + 1; i < board.height; i++) {
            if (board.candies[candy.x, i] != null && board.candies[candy.x, i].type == candy.type) {
                matches.Add(board.candies[candy.x, i]);
            } else {
                break;
            }
        }

        return matches;
    }

    public void DestroyMatches(List<Candy> matches)
    {
        AudioManager.Instance.PlaySound(AudioManager.Instance.matchSound); // Play match sound
        board.scoreManager.CalculateScore(matches);

        foreach (Candy candy in matches)
        {
            board.candies[candy.x, candy.y] = null; 
            Destroy(candy.gameObject);    
        }
    }
}