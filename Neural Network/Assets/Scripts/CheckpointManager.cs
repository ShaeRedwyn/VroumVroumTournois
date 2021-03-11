using UnityEngine;

public class CheckpointManager : MonoBehaviour
{
    public static CheckpointManager Instance;
    public Transform firstCheckpoint;

    private void Awake()
    {
        Instance = this;
        Init();
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    [ContextMenu("Init")]
    void Init()
    {
        firstCheckpoint = transform.GetChild(0);
        int childCount = transform.childCount;

        for (int i = 0; i < transform.childCount-1; i++)
        { 
            transform.GetChild(i).GetComponent<Checkpoint>().nextCheckpoint = transform.GetChild(i + 1);
        }

        transform.GetChild(transform.childCount - 1).GetComponent<Checkpoint>().nextCheckpoint = transform.GetChild(0);
    }
}
