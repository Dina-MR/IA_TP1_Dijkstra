using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static GridGenerator;

public class GridGenerator : MonoBehaviour
{
    // Nombre de tiles par ligne et colonne
    public int rows, cols;
    // Coordonn�es de d�part de la grille, correspondant � la 1�re cellule
    public Vector3 startingCellPosition;
    [SerializeField]
    Camera camera; // cam�ra utilis�e pour le positionnement de la grille
    // Tiles repr�sentant une cellule
    [SerializeField]
    GameObject tile;
    [SerializeField]
    GameObject wallTile; // tile repr�sentant un mur, ne peut �tre travers�e
    // Probabilit� de g�n�ration de cellule de cat�gorie "Walkable"
    public float walkableProbability = 0.8f;
    // Poisition de d�part du joueur
    public Vector3 playerStart = Vector3.zero;

    // Liste des cellules
    public List<Cell> gridCells;
    // Etat de la liste
    public bool isListFull = false;

    // Sommets de d�part et d'arriv�e, pour le pathfinding
    public int startId, targetId;

    // Start is called before the first frame update
    void Awake()
    {
        // Initialisation de la liste de cellules
        gridCells = new List<Cell>();
        // Initialisation de la position de la 1�re cellule
        startingCellPosition = new Vector3(-camera.orthographicSize * 2, camera.orthographicSize, 0f);
        // Affichage des cellules
        displayCells();
    }

    // Update is called once per frame
    void Update()
    {
        updateStart();
        updateGoal();
    }

    // Ajout & affichage des cellules
    void displayCells()
    {
        int id = 0;
        for (int i = 0; i < cols; i++)
        {
            for (int j = 0; j < rows; j++)
            {
                // Le bord du terrain est compos� de murs
                if(i == 0 || j == 0 || i == cols - 1|| j == rows - 1)
                    gridCells.Add(new Cell(id, i, j, startingCellPosition, wallTile, Category.Wall));
                else
                {
                    // On choisit al�atoirement la nature de la cellule, avec une probabilit� �tablie
                    float probability = Random.Range(0f, 1f);
                    // On g�n�re un mur si la probabilit� d�passe celle �tablie ET si la case est diff�rente de celle de la position de d�part du joueur
                    if(probability > walkableProbability && new Vector3(startingCellPosition.x + i, startingCellPosition.y - j, 0) != playerStart)
                        gridCells.Add(new Cell(id, i, j, startingCellPosition, wallTile, Category.Wall));
                    else
                        gridCells.Add(new Cell(id, i, j, startingCellPosition, tile, Category.Walkable));
                }
                Debug.Log("Cellule n� :" + id + " en position " + gridCells[id].position);
                id++;
            }
            //id++;
        }
        isListFull = true;
    }

    // Distance entre deux Cellules
    public float getCellsDistance(List<Cell> _gridCells, int cellIndex1, int cellIndex2)
    {
        return Vector3.Distance(_gridCells[cellIndex1].position, _gridCells[cellIndex2].position);
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
            // Remarque : on n'ajoute que les voisins qui peuvent �tre travers�s par le joueur
            // Voisin d'en haut � gauche
            if (x > 0 && gridCells[id - cols - 1].category != Category.Wall)
                neighBours.Add(gridCells[id - cols - 1]);
            // Voisin d'en haut
            if(gridCells[id - cols].category != Category.Wall)
                neighBours.Add(gridCells[id - cols]);
            // Voisin d'en haut � droite
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
            // Voisin d'en bas � gauche
            if (x > 0 && gridCells[id + cols - 1].category != Category.Wall)
                neighBours.Add(gridCells[id + cols - 1]);
            // Voisin d'en bas
            if(gridCells[id + cols].category != Category.Wall)
                neighBours.Add(gridCells[id + cols]);
            // Voisin d'en bas � droite
            if (x < cols && gridCells[id + cols + 1].category != Category.Wall)
                neighBours.Add(gridCells[id + cols + 1]);
        }
        return neighBours;
    }

    // Mise � jour du sommet de d�part
    void updateStart()
    {
        int newStart = -1;
        newStart = gridCells.FindIndex(cell => cell.cellTile.GetComponent<TileCollision>().isStart == true);
        if (newStart > -1)
            startId = newStart;
    }

    // Mise � jour du sommet d'arriv�e
    void updateGoal()
    {
        int newGoal = -1;
        newGoal = gridCells.FindIndex(cell => cell.cellTile.GetComponent<TileCollision>().isGoal == true);
        if (newGoal > -1)
            targetId = newGoal;
    }

    //Classe repr�sentant une cellule de la grille
    public class Cell : MonoBehaviour
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
        // Cat�gorie de la cellule
        public Category category;
        // Indique si le joueur et/ou l'ennemi se trouve sur cette cellule
        public bool isStart = false;
        public bool isGoal = false;

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
            //// Position du calque d�di�
            //cellTile.GetComponent<Renderer>().sortingOrder = _tileLayer;
        }

        // Changement de couleur � des fins d'analyse
        public void changeColor(Color _color)
        {
            cellTile.GetComponent<Renderer>().material.color = _color;
        }
    }

    // Cat�gories de cellules
    public enum Category
    {
        Walkable,
        Wall
    }
}
