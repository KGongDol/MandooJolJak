using UnityEngine;

public class TextSlider : MonoBehaviour
{
    [Header("슬라이드 시킬 텍스트")]
    public Transform slideText;

    [Header("속도")]
    [Range(10,50)]
    public float speed = 3.0f;

    [Header("텍스트가 이동을 시작할 지점(오른쪽)")]
    public float startX = 46f;
    [Header("텍스트가 이동을 끝낼 지점(왼쪽)")]
    public float endX = -2;
    
    void Update()
    {
        slideText.position -= Vector3.right * speed * Time.deltaTime;
        if(slideText.position.x <= endX)
            slideText.position = new Vector3(startX, slideText.position.y, slideText.position.z);
    }
}
