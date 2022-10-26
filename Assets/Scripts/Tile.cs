
using UnityEngine;

public class Tile : MonoBehaviour
{
    public SpriteRenderer spriteRenderer;//поле через которое будет меняться изображение спрайтов в тайле;
    public bool isSelected; //поле определение выбора тайла
    public bool isEmptry
    {
        get
        {
            return spriteRenderer.sprite == null ? true : false;
        }
    } // проверка пустоты тайла

}
