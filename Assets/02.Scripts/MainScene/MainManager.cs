using UnityEngine;

public class MainManager : MonoBehaviour
{
    public static MainManager instance;
    public static MainManager Instance
    {
        get
        {
            if(instance == null)
                return null;
            else
                return instance;
        }
    }
    public AudioClip[] pointerClips;
    public int pointerIdx = 0;
    AudioSource pointerSource;

    void Awake()
    {
        if(instance == null)
            instance = this;
        else
        Destroy(gameObject);

        pointerSource =GetComponent<AudioSource>();
    }

    void Start()
    {
        
    }

    void Update()
    {
        
    }

    public void PlayPointerSFX()
    {
        pointerSource.clip = pointerClips[pointerIdx];
        pointerSource.Play();
    }
}
