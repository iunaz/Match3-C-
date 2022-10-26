using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoardController : MonoBehaviour
{
    public static BoardController instance;  //ссылка глобального доступа для получения данных

    private int xSize, ySize; //используются для инициализации двумерного массива
    private List<Sprite> tileSprite = new List<Sprite>(); //для списка доступных ихображений для тайлов
    private Tile[,] tileArray; //для передачи массива из класса Board, который заполняется при создании доски
    private Tile oldSelectTile;//предыдущий выделенный тайл
    private readonly Vector2[] dirRay = new Vector2[] { Vector2.up, Vector2.down,  Vector2.left, Vector2.right };//поле отчечающее за выпуск векторов 
    private bool isFindMatch = false; // поле отвечающее за совпадения 
    private bool isShift = false; //запрет для сдвига во время хода
    private bool isSearchEmptyTile = false; //запрещение работы метода в Update


    public void SetValue(Tile[,] tileArray, int xSize, int ySize, List<Sprite> tileSprite) //метод для получения данных с Board
    {
        this.tileArray= tileArray;
        this.xSize = xSize;
        this.ySize = ySize;
        this.tileSprite = tileSprite;
    }

    private void Awake()
    {
        instance = this;
    }


    // Update is called once per frame
    void Update()
    {
        if (isSearchEmptyTile)
        {
            SearchEmptyTile();
        }

        if (Input.GetMouseButtonDown(0))
        {
            RaycastHit2D ray = Physics2D.GetRayIntersection(Camera.main.ScreenPointToRay(Input.mousePosition));
            if (ray != false)
            {
                CheсkSelectTile(ray.collider.gameObject.GetComponent<Tile>());
            }
        }
    }
    #region (Выделение тайла,снятие выделения с тайла, управление выделением)
    private void SelectTile(Tile tile) // метод отвечающий за выделение тайла
    {
        tile.isSelected = true;
        tile.spriteRenderer.color = new Color(0.5f, 0.5f, 0.5f);//цвет при нажатии 
        oldSelectTile = tile;

    }
    private void DeselectTile(Tile tile) // метод отвечающий за снятие выделения с тайла
    {
        tile.isSelected = false;
        tile.spriteRenderer.color = new Color(1,1,1);//возвращение нормального цвета 
        oldSelectTile = null;
    }

    private void CheсkSelectTile(Tile tile) // метод отвечающий за выбор выделения тайла или снятие выделения 
    {
        if (tile.isEmptry || isShift) { return; }
        if (tile.isSelected)
        {
            DeselectTile(tile);
        }
        else
        {
            //Первое выделение
            if (!tile.isSelected && oldSelectTile == null)
            {
                SelectTile(tile);
            }
            //Попытка выбрать другой тайл
            else
            {
                //Если 2ой выбрванный тайл сосед предыдущего тайла
                if (AdjacentTiles().Contains(tile))
                {
                    SwapTwoTile(tile);
                    FindAllMatch(tile);
                    DeselectTile(oldSelectTile);
                }
                //новое выделение,забываем старый тайл
                else
                {
                    DeselectTile(oldSelectTile);
                    SelectTile(tile);
                }
            }
        }
    }
    #endregion

    #region(Поиск совпадений, удаление спрайта, поиск всех совпадений)
    private List<Tile> FindMatch(Tile tile,Vector2 dir)
    {
        List<Tile> cashFindMatch = new List<Tile>();
        RaycastHit2D hit = Physics2D.Raycast(tile.transform.position, dir);
        while (hit.collider!=null && hit.collider.gameObject.GetComponent<Tile>().spriteRenderer.sprite==tile.spriteRenderer.sprite)
        {
            cashFindMatch.Add(hit.collider.gameObject.GetComponent<Tile>());
            hit= Physics2D.Raycast(hit.collider.gameObject.transform.position, dir);
        }
        return cashFindMatch;
    }

    private void DeleteSprite(Tile tile, Vector2[] dirArray)
    {
        List<Tile> cashFinhSprite = new List<Tile>();
        for (int i = 0; i < dirArray.Length; i++)
        {
            cashFinhSprite.AddRange(FindMatch(tile, dirArray[i]));
        }
            if (cashFinhSprite.Count >= 2)
            {
                for (int i = 0; i < cashFinhSprite.Count; i++)
                {
                    cashFinhSprite[i].spriteRenderer.sprite = null;
                }
            isFindMatch = true;
            }
    }

    private void FindAllMatch(Tile tile)//поиск всех совпадений 
    {
        if (tile.isEmptry) { return; }
        DeleteSprite(tile, new Vector2[2] { Vector2.up, Vector2.down });
        DeleteSprite(tile, new Vector2[2] { Vector2.left, Vector2.right});
        if (isFindMatch)
        {
            isFindMatch = false;
            tile.spriteRenderer.sprite=null;
            isSearchEmptyTile=true;
        }
    }
    #endregion

    #region(Смена 2х тайлов, соседние тайлы)
    private void SwapTwoTile(Tile tile) //метод для смены мест тайлов
    {
        if (oldSelectTile.spriteRenderer.sprite==tile.spriteRenderer.sprite) { return; }
        Sprite cashStrite = oldSelectTile.spriteRenderer.sprite;
        oldSelectTile.spriteRenderer.sprite = tile.spriteRenderer.sprite;
        tile.spriteRenderer.sprite = cashStrite;

        UI.instance.Moves(1);
    }

    private List<Tile> AdjacentTiles() // метод возвращающий список тайлов ноходящихся по соседству
    {
        List<Tile> cashTiles = new List<Tile>();
        for(int i=0; i < dirRay.Length; i++)
        {
            RaycastHit2D hit = Physics2D.Raycast(oldSelectTile.transform.position, dirRay[i]);
            if (hit.collider != null)
            {
                cashTiles.Add(hit.collider.gameObject.GetComponent<Tile>());
            }
        }
        return cashTiles;
    }
    #endregion

    #region(поиск пустого тайла, Сдвиг тайла,установка нового изображения,выбрать новое изображение тайла)

    private void SearchEmptyTile()//метод поиска пустого тайла
    {
        for(int x=0; x < xSize; x++)
        {
            for (int y = 0; y < ySize; y++)
            {
                if (tileArray[x, y].isEmptry)
                {
                    ShiftTileDown(x, y);
                    break;
                }
                if (x == xSize && y == ySize-1)
                {
                    isSearchEmptyTile = false;
                }
            }
        }
    }
    private void ShiftTileDown(int xPos, int yPos)//метод отвечающий за смещение тайла вниз
    {
        isShift = true;
        List<SpriteRenderer> cashRenderer = new List<SpriteRenderer>(); // список при помощи которого меняюся изображения у тайлов
        int count = 0;
        for (int y = yPos; y < ySize; y++) //узнать сколько тайлов над пустым тайлом
        {
            Tile tile = tileArray[xPos, y]; //для хранения ссылки выбранного тайла из массива
            if (tile.isEmptry)
            {
                count++;
            }
            cashRenderer.Add(tile.spriteRenderer) ;
        }
        for (int i=0;i<count;i++)
        {
            UI.instance.Score(50);
            SetNewSprite(xPos, cashRenderer);
        }
        isShift = false;
    }
    private void SetNewSprite(int xPos, List<SpriteRenderer> renderer)//метод установки  новых изображений у пустых тайлов
    {
        for (int y = 0; y < renderer.Count-1; y++) //узнать сколько тайлов над пустым тайлом
        {
            renderer[y].sprite = renderer[y + 1].sprite;
            renderer[y + 1].sprite = GetNewSprite(xPos, ySize - 1);
        }
    }
    private Sprite GetNewSprite(int xPos, int yPos)//метод возвращающий новое изображение для тайла с учетом других тайлов 
    {
        List<Sprite> cashSprite = new List<Sprite>();
        cashSprite.AddRange(tileSprite);

        if (xPos > 0)
        {
            cashSprite.Remove(tileArray[xPos - 1, yPos].spriteRenderer.sprite);
        }

        if (xPos < xSize - 1)
        {
            cashSprite.Remove(tileArray[xPos + 1, yPos].spriteRenderer.sprite);
        }

        if (yPos > 0)
        {
            cashSprite.Remove(tileArray[xPos, yPos - 1].spriteRenderer.sprite);
        }

        return cashSprite[Random.Range(0, cashSprite.Count)];
    }

    #endregion
}
