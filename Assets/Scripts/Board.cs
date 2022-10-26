using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Board : MonoBehaviour
{
    public static Board instance;//для взаимодействия с классом (ссылка глобального доступа)

    private int xSize, ySize; //размерность игрового поля 
    private Tile tileGO; //созданные префабы
    private List<Sprite> tileSprite= new List<Sprite>(); //список с изображнием тайлов

    private void Awake()
    {
        instance = this;
    }
    public Tile[,] SetValue(int xSize, int ySize, Tile tileGO, List<Sprite> tileSprite)//метод для получения данных с GameManager
    {
        this.xSize = xSize;
        this.ySize = ySize;
        this.tileGO = tileGO;
        this.tileSprite = tileSprite;
        return CreateBroad();
    }

    private Tile[,] CreateBroad()//метод создающий игровую доску
    {
        Tile[,] tileArray = new Tile[xSize,ySize];//двухмерный массив куда помещаются созданные тайлы по х и у
        float xPos = transform.position.x; //для создания тайлов где находится Вoard
        float yPos = transform.position.y;
        Vector2 tileSize = tileGO.spriteRenderer.bounds.size;//используется для предотвращения скапливания тайлов

        Sprite cashStrite = null;

        for(int x=0; x < xSize; x++)
        {
            for (int y = 0; y < ySize; y++)
            {
                Tile newTile = Instantiate(tileGO, transform.position, Quaternion.identity);
                newTile.transform.position = new Vector3(xPos + (tileSize.x * x), yPos + (tileSize.y * y),0);//смещение плиток
                newTile.transform.parent = transform; //тайлы страновятся дочерними объектами Board чтобы не засорять иерархию 
                tileArray[x, y] = newTile;

                List<Sprite> tempSprite = new List<Sprite>(); //список используемый для хранения изображений
                tempSprite.AddRange(tileSprite); //помещаем в список, список полученный из GameManager(т.е список спрайтов доступных для тайла)

                tempSprite.Remove(cashStrite);//предотвращение повторов по у
                if (x > 0)
                {
                    tempSprite.Remove(tileArray[x-1,y].spriteRenderer.sprite);//проветраа по х, удаляем изобращение которое находится слева
                }
                newTile.spriteRenderer.sprite = tempSprite[Random.Range(0, tempSprite.Count)]; //помещение случайного спрайта в поле спрайта из списка
                cashStrite = newTile.spriteRenderer.sprite;

            }
        }

        return tileArray;
    }
    
}
