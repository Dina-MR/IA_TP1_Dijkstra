using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.UIElements;
using static GridGenerator;

public class GridSystem : MonoBehaviour
{
    // Grille
    Grid grid;
    Tilemap floor;
    public List<Tilemap> obstacles;

    // Sprites des différents types de noeuds
    [SerializeField]
    GameObject walkableTile;
    [SerializeField]
    GameObject wallTile; 

    // Noeuds
    public List<Node> nodesList; // Liste non ordonnée de noeuds
    public Node[,] nodesArray; // Tableau ordonné de noeuds
    // Dimensions & point de départ de la grille
    public int gridWidth, gridHeight;
    public Vector3 startingPosition;

    private void Start()
    {
        nodesArray = new Node[gridWidth, gridHeight];
        generateGrid();
    }

    // Génération de la grille
    void generateGrid()
    {
        // Création des noeuds
        for(int y = 0; y < gridHeight; y++)
            for(int x = 0; x < gridWidth; x++)
            {
                bool isObstacle = true;
                TileBase tileBase = floor.GetTile(new Vector3Int(x, y, 0));
                if(tileBase != null)
                {
                    isObstacle = false;
                }
                Vector3 realPosition = new Vector3(x + startingPosition.x, y + startingPosition.y, 0f);
                GameObject nodeTile = displayNode(walkableTile, realPosition, x, y);
                Vector3Int cellPosition = floor.WorldToCell(realPosition);
                Node node;
                if(isObstacle)
                    node = new Node(x, y, cellPosition.x, cellPosition.y, Category.Wall);
                else
                    node = new Node(x, y, cellPosition.x, cellPosition.y, Category.Walkable);
                nodesList.Add(node);
            }

        // Mise à jour des voisins
        foreach(Node node in nodesList)
        {
            node.setNeighbours(nodesArray, gridWidth);
        }
    }

    // Affichage d'un noeud
    GameObject displayNode(GameObject tile, Vector3 position, int x, int y)
    {
        GameObject nodeTile = Instantiate(tile, position, Quaternion.identity);
        nodeTile.name = "Noeud [" + x + ", " + y + "]";
        return nodeTile;
    }

    public class Node
    {
        // Coordonnées réelles
        public int realX, realY;
        // Coordonnées cellulaires
        public int cellX, cellY;
        public int cost, heuristic;
        public Category category;
        List<Node> neighbours;

        public Node(int _realX, int _realY, int _cellX, int _cellY, Category _category)
        {
            realX = _realX;
            realY = _realY;
            cellX = _cellX;
            cellY = _cellY;
            category = _category;
        }
        
        // Voisins d'un noeud
        public void setNeighbours(Node[,] nodesArray, int _gridWidth)
        {
            List<Node> neighBours = new List<Node>();
            // Voisins du haut
            if (realY > 0)
            {
                // Remarque : on n'ajoute que les voisins qui peuvent être traversés par le joueur
                // Voisin d'en haut à gauche
                if (realX > 0 && nodesArray[realY - 1, realX - 1].category != Category.Wall)
                    neighBours.Add(nodesArray[realY - 1, realX - 1]);
                // Voisin d'en haut
                if (nodesArray[realY - 1, realX].category != Category.Wall)
                    neighBours.Add(nodesArray[realY - 1, realX]);
                // Voisin d'en haut à droite
                if (realX < _gridWidth && nodesArray[realY - 1, realX + 1].category != Category.Wall)
                    neighBours.Add(nodesArray[realY - 1, realX + 1]);

            }
            // Voisins du milieu
            if (realX > 0 && nodesArray[realY, realX - 1].category != Category.Wall)
                neighBours.Add(nodesArray[realY, realX - 1]);
            if (realX < _gridWidth && nodesArray[realY, realX + 1].category != Category.Wall)
                neighBours.Add(nodesArray[realY, realX + 1]);
            // Voisins du bas
            if (realY < _gridWidth)
            {
                // Voisin d'en bas à gauche
                if (realX > 0 && nodesArray[realY + 1, realX - 1].category != Category.Wall)
                    neighBours.Add(nodesArray[realY + 1, realX - 1]);
                // Voisin d'en bas
                if (nodesArray[realY + 1, realY].category != Category.Wall)
                    neighBours.Add(nodesArray[realY + 1, realX]);
                // Voisin d'en bas à droite
                if (realX < _gridWidth && nodesArray[realY + 1, realX + 1].category != Category.Wall)
                    neighBours.Add(nodesArray[realY + 1, realX + 1]);
            }
        }
    }


    // Catégorue de cellule
    public enum Category
    {
        Walkable,
        Wall
    }
}
