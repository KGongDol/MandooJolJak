using Unity.VisualScripting;
using UnityEngine;

public class TextSlider : MonoBehaviour
{
    public static TextSlider instance;
    public static TextSlider Instance
    {
        get
        {
            if( instance == null)
                return null;
            else
                return instance;
        }
    }

    [Header("슬라이드 시킬 텍스트")]
    public Transform[] texts;
    [Header("고정 텍스트")]
    public Transform[] fixedTexts;
    internal int selectedIdx = 0;
    
    [Header("속도")]
    [Range(10,50)]
    public float speed = 3.0f;

    [Header("텍스트가 이동을 시작할 지점(오른쪽)")]
    public float startX = 46f;
    [Header("텍스트가 이동을 끝낼 지점(왼쪽)")]
    public float endX = -2;
    
    Transform curText;

    void Awake()
    {
        if(instance == null)
            instance = this;
        else
            Destroy(gameObject);
    }
    void Start()
    {
        curText = texts[selectedIdx];
    }

    void Update()
    {
        curText.position -= Vector3.right * speed * Time.deltaTime;
        if(curText.position.x <= endX)
            curText.position = new Vector3(startX, curText.position.y, curText.position.z);
    }

    public void SetSlidingText(int index)
    {
        
        texts[selectedIdx].gameObject.SetActive(false);

        if(selectedIdx < fixedTexts.Length)
            fixedTexts[selectedIdx].gameObject.SetActive(false);

        selectedIdx = index;
        texts[selectedIdx].gameObject.SetActive(true);

        if(selectedIdx < fixedTexts.Length)
            fixedTexts[selectedIdx].gameObject.SetActive(true);
            
        curText = texts[selectedIdx];
    }
}
