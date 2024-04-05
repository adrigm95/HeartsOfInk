public class FactionScore
{
    private int totalScore;

    public FactionScore()
    {
        totalScore = 0;
    }

    public void AddScore(int score)
    {
        totalScore += score;
    }

    public int GetTotalScore()
    {
        return totalScore;
    }
}