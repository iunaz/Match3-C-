using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class BoardSetting // класс для настройки игровой доски
{
    public int xSize, ySize; //размерность игрового поля 
    public Tile tileGO; //созданные префабы
    public List<Sprite> tileSprite; //список с изображнием тайлов
}

public class GameManager : MonoBehaviour
{
    [Header("Настройки игровой доски")]
    public BoardSetting boardSetting;
    // Start is called before the first frame update
    void Start()
    {
        BoardController.instance.SetValue(Board.instance.SetValue(boardSetting.xSize, boardSetting.ySize, boardSetting.tileGO, boardSetting.tileSprite),
            boardSetting.xSize, boardSetting.ySize,
            boardSetting.tileSprite);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
