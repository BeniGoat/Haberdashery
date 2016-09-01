using UnityEngine;

public class ScorePlacer : MonoBehaviour
{
    public RectTransform[] ScoreTransforms;
    private RectTransform[] OriginalTransforms;
    private int numPlayers;

    void Start()
    {
        numPlayers = PlayerCount.numberOfPlayers;
        OriginalTransforms = ScoreTransforms;

        switch (numPlayers)
        {
            case 2:
                for (int i = 0; i < numPlayers; i++)
                {
                    ScoreTransforms[i].position = ScoreTransforms[i + 1].position;
                }
                break;

            case 3:
                ScoreTransforms[2].position = ScoreTransforms[3].position;
                ScoreTransforms[1].position = new Vector3((ScoreTransforms[0].position.x + ScoreTransforms[2].position.x) / 2, ScoreTransforms[1].position.y, ScoreTransforms[1].position.z);
                break;

            case 4:
                ScoreTransforms = OriginalTransforms;
                break;
        }
    }
}
