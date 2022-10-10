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
    [SerializeField]
    Camera camera; // caméra utilisée pour le positionnement de la grille
    // Tiles représentant une cellule
    [SerializeField]
    GameObject tile;
    [SerializeField]
    GameObject wallTile; // tile représentant un mur, ne peut être traversée
    // Probabilité de génération de cellule de catégorie "Walkable"
    public float walkableProbability = 0.8f;
    // Poisition de départ du joueur
    public Vector3 playerStart = Vector3.zero;

    // Liste des cellules
    public List<Cell> gridCells;
    // Etat de la liste
    public bool isListFull = false;

    // Start is called before the first frame update
    void Awake()
    {
        // Initialisation de la liste de cellules
        gridCells = new List<Cell>();
        // Initialisation de la position de la 1ère cellule
        startingCellPosition = new Vector3(-camera.orthographicSize * 2, camera.orthographicSize, 0f);
        // Affichage des cellules
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
        for (int i = 0; i < cols; i++)
        {
            for (int j = 0; j < rows; j++)
            {
                // Le bord du terrain est composé de murs
                if(i == 0 || j == 0 || i == cols - 1|| j == rows - 1)
                    gridCells.Add(new Cell(id, i, j, startingCellPosition, wallTile, Category.Wall));
                else
                {
                    // On choisit aléatoirement la nature de la cellule, avec une probabilité établie
                    float probability = Random.Range(0f, 1f);
                    // On génère un mur si la probabilité dépasse celle établie ET si la case est différente de celle de la position de départ du joueur
                    if(probability > walkableProbability && new Vector3(startingCellPosition.x + i, startingCellPosition.y - j, 0) != playerStart)
                        gridCells.Add(new Cell(id, i, j, startingCellPosition, wallTile, Category.Wall));
                    else
                        gridCells.Add(new Cell(id, i, j, startingCellPosition, tile, Category.Walkable));
                }
                Debug.Log("Cellule n° :" + id + " en position " + gridCells[id].position);
                id++;
            }
            //id++;
        }
        isListFull = true;
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
            // Remarque : on n'ajoute que les voisins qui peuvent être traversés par le joueur
            // Voisin d'en haut à gauche
            if (x > 0 && gridCells[id - cols - 1].category != Category.Wall)
                neighBours.Add(gridCells[id - cols - 1]);
            // Voisin d'en haut
            if(gridCells[id - cols].category != Category.Wall)
                neighBours.Add(gridCells[id - cols]);
            // Voisin d'en haut à droite
            if(x < cols && gridCells[id - cols + 1].category != Category.Wall)
                neighBours.Add(gridCells[id - cols + 1]);

        }
        // Voisins du milieu
        if (x > 0 && gridCells[id - 1].category != Category.Wall)
            neighBours.Add(gridCells[id - 1]);
        if (x < cols && gridCells[id + 1].category != Category.Wall)
            neighBours.Add (gridCells[id + 1]);
        // Voisins du bas
        if(y < rows)
        {
            // Voisin d'en bas à gauche
            if (x > 0 && gridCells[id + cols - 1].category != Category.Wall)
                neighBours.Add(gridCells[id + cols - 1]);
            // Voisin d'en bas
            if(gridCells[id + cols].category != Category.Wall)
                neighBours.Add(gridCells[id + cols]);
            // Voisin d'en bas à droite
            if (x < cols && gridCells[id + cols + 1].category != Category.Wall)
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
        // Catégorie de la cellule
        public Category category;

        public Cell(int _id, int _x, int _y, Vector3 _startingCellPosition, GameObject _tile, Category _category)
        {
            id = _id;
            x = _x;
            y = _y;
            position = new Vector3(_startingCellPosition.x + x, _startingCellPosition.y - y, 0);
            category = _category;
            setDisplayCell(_tile);
        }

        // Affichage
        void setDisplayCell(GameObject _tile)
        {
            cellTile = Instantiate(_tile, position, Quaternion.identity);
            cellTile.name = "Cellule [" + x + ", " + y + "]";
            //cellTile.layer = _tileLayer;
            //// Position du calque dédié
            //cellTile.GetComponent<Renderer>().sortingOrder = _tileLayer;
        }
    }

    // Catégories de cellules
    public enum Category
    {
        Walkable,
        Wall
    }
}
