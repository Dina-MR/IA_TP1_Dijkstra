using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridGenerator : MonoBehaviour
{
    // Nombre de tiles par ligne et colonne
    public int rows;
    public int cols;
    // Coordonnées de départ de la grille, correspondant à la 1ère cellule
    public Vector3 startingCellPosition;
    // Tile représentant une cellule
    [SerializeField]
    GameObject tile;
    // Calque des tiles
    int tileLayer;

    // Liste des cellules
    public List<Cell> gridCells;

    // Start is called before the first frame update
    void Start()
    {
        gridCells = new List<Cell>();
        // Récupération du calque dans lequel se trouve les cellules
        tileLayer = SortingLayer.GetLayerValueFromName("Ground");
        // Affichage des cellules
        displayCells();
        // DEBOGGAGE - Affichage des voisins d'une cellule
    }

    // Update is called once per frame
    void Update()
    {

    }

    // Ajout & affichage des cellules
    void displayCells()
    {
        int id = 0;
        for (int i = 0; i < cols; i++)
        {
            for (int j = 0; j < rows; j++)
            {
                gridCells.Add(new Cell(id, i, j, startingCellPosition, tile, tileLayer));
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

    // Voisins d'une cellule
    public List<Cell> neighBours(Cell cell)
    {
        List<Cell> neighBours = new List<Cell> ();
        int id = cell.id;
        int x = cell.x;
        int y = cell.y;
        // Voisins du haut
        if(y > 0)
        {
            // Voisin d'en haut à gauche
            if (x > 0)
                neighBours.Add(gridCells[id - cols - 1]);
            // Voisin d'en haut
            neighBours.Add(gridCells[id - cols]);
            // Voisin d'en haut à droite
            if(x < cols)
                neighBours.Add(gridCells[id - cols + 1]);

        }
        // Voisins du milieu
        if (x > 0)
            neighBours.Add(gridCells[id - 1]);
        if (x < cols)
            neighBours.Add (gridCells[id + 1]);
        // Voisins du bas
        if(y < rows)
        {
            // Voisin d'en bas à gauche
            if (x > 0)
                neighBours.Add(gridCells[id + cols - 1]);
            // Voisin d'en bas
            neighBours.Add(gridCells[id + cols]);
            // Voisin d'en bas à droite
            if (x < cols)
                neighBours.Add(gridCells[id + cols + 1]);
        }
        return neighBours;
    }

    //Classe représentant une cellule de la grille
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

        public Cell(int _id, int _x, int _y, Vector3 _startingCellPosition, GameObject _tile, int _tileLayer)
        {
            id = _id;
            x = _x;
            y = _y;
            position = new Vector3(_startingCellPosition.x + x, _startingCellPosition.y - y, 0);
            setDisplayCell(_tile, _tileLayer);
        }

        // Affichage
        void setDisplayCell(GameObject _tile, int _tileLayer)
        {
            cellTile = Instantiate(_tile, position, Quaternion.identity);
            cellTile.name = "Cellule [" + x + ", " + y + "]";
            cellTile.layer = _tileLayer;
            // Position du calque dédié
            cellTile.GetComponent<Renderer>().sortingOrder = _tileLayer;
        }
    }
}
