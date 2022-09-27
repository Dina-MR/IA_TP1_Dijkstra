using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridGenerator : MonoBehaviour
{
    // Nombre de tiles par ligne et colonne
    public int rows;
    public int cols;
    // Coordonn�es de d�part de la grille, correspondant � la 1�re cellule
    public Vector3 startingCellPosition;
    // Tile repr�sentant une cellule
    [SerializeField]
    GameObject tile;

    // Liste des cellules
    public List<Cell> gridCells;

    // Start is called before the first frame update
    void Start()
    {
        gridCells = new List<Cell>();
        displayCells();
    }

    // Update is called once per frame
    void Update()
    {

    }

    // Ajout & affichage des cellules
    void displayCells()
    {
        int id = 0;
        for (int i = 0; i < rows; i++)
        {
            for (int j = 0; j < cols; j++)
            {
                gridCells.Add(new Cell(id, i, j, startingCellPosition, tile));
                id++;
            }
            id++;
        }
    }

    // Distance entre deux Cellules
    public float getCellsDistance(int cellIndex1, int cellIndex2)
    {
        return Vector3.Distance(gridCells[cellIndex1].position, gridCells[cellIndex2].position);
    }

    //Classe repr�sentant une cellule de la grille
    public class Cell
    {
        // Identifiant de la cellule
        public int id;
        // Indices de la cellule
        public int x;
        public int y;
        // Position
        public Vector3 position;
        // Tile
        public GameObject cellTile;

        public Cell(int _id, int _x, int _y, Vector3 _startingCellPosition, GameObject _tile)
        {
            id = _id;
            x = _x;
            y = _y;
            position = new Vector3(_startingCellPosition.x + x, _startingCellPosition.y - y, 0);
            setDisplayCell(_tile);
        }

        // Affichage
        void setDisplayCell(GameObject _tile)
        {
            cellTile = Instantiate(_tile, position, Quaternion.identity);
            cellTile.name = "Cellule [" + x + ", " + y + "]";
        }
    }
}
