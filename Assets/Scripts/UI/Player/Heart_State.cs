using UnityEngine;
using UnityEngine.UI;

public class Heart_State : MonoBehaviour
{
    [SerializeField] [Range(0,2)] private int currentState = 2;
    [SerializeField] Image currentImage;

    [Header("Assets")]
    [SerializeField] private Sprite completeHeart;
    [SerializeField] private Sprite mediumHeart;
    [SerializeField] private Sprite nullHeart;

    public void SetState(int newState)
    {
        switch (newState)
        {
            case 2:
                currentImage.sprite = completeHeart;
                break;
            case 1:
                currentImage.sprite = mediumHeart;
                break;
            case 0:
                currentImage.sprite = nullHeart;
                break;
        }
    }
}
