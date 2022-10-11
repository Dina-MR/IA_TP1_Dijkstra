using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using static GridGenerator;

public class DijkstraPathFiding : MonoBehaviour
{
    // Cible & id de départ 
    public GameObject target;
    PlayerMovement targetMove;
    public int startingId = 0;

    // Variables liées à la grille
    public GameObject gridObject;
    GridGenerator grid;

    // Tableau des distances & des indices des cellules parcourues
    List<float> distances;
    List<float> previousIndexes;

    void Start()
    {
        // Récupération de la grille
        grid = gridObject.GetComponent<GridGenerator>();
        // Initialisation de variables
        distances = new List<float>();
        previousIndexes = new List<float>();
        // Récupération du script de mouvement du joueur
        targetMove = target.GetComponent<PlayerMovement>();
        // Exécution de l'algorithme de pathfinding
        if (grid.isListFull)
        {
            Debug.Log("Parcours !");
            PathFinding(startingId);
        }
    }

    private void Update()
    {
        // Exécution de l'algorithme de pathfinding, qui se remet à jour dès que la cible bouge
        //if (grid.isListFull && targetMove.isMoving)
        //    PathFinding(startingId);
    }

    // Fonction principale
    void PathFinding(int sommetStart)
    {
        Initialization(sommetStart);
        List<Cell> cellsCopy = grid.gridCells;
        while(cellsCopy.Count > 0)
        {
            int sommet1 = minimumCell(cellsCopy);
            Cell cell = cellsCopy[sommet1];
            //Coloration de la cellule en cours d'analyse
            cell.changeColor(Color.cyan);
            cellsCopy.RemoveAt(sommet1);
            foreach(Cell neighbour in grid.neighBours(cell))
            {
                int sommet2 = neighbour.id;
                updateDistances(cellsCopy, sommet1, sommet2);
                Debug.Log("Sommet 2");
            }
        }
        //List<Cell> path = new List<Cell>();

    }

    // Fonction d'intialisation des distances
    void Initialization(int characterIndex)
    {
        foreach(GridGenerator.Cell cell in grid.gridCells)
        {
            distances.Add(float.MaxValue);
            Debug.Log("Id courant " + cell.id);
        }
        distances[characterIndex] = 0;
        // Déboggage pour le nombre de distances
        Debug.Log("Taille de la liste distance : " + distances.Count);
        // Copie de la liste distances dans previousIndexes
        previousIndexes = distances;
        // Positionnement de l'ennemi
        transform.position = grid.gridCells[startingId].position;
    }

    // Recherche de la cellule de distance minimale
    int minimumCell(List<Cell> subGridCells)
    {
        float minimum = float.MaxValue;
        int sommet = -1;
        foreach(GridGenerator.Cell cell in subGridCells)
        {
            if (distances[cell.id] < minimum)
            {
                minimum = distances[cell.id];
                sommet = cell.id;
            }
        }
        return sommet;
    }

    // Mise à jour des distances
    void updateDistances(List<Cell> _cellsList, int sommet1, int sommet2)
    {
        Debug.Log("Sommet 1 :" + sommet1);
        Debug.Log("Sommet 2 :" + sommet2);
        float poids = float.MaxValue;
        poids = grid.getCellsDistance(_cellsList, sommet1, sommet2);
        if (distances[sommet2] > distances[sommet1] + poids && sommet2 < _cellsList.Count)
        {
            distances[sommet2] = distances[sommet1] + poids;
            previousIndexes[sommet2] = sommet1;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Floor"))
            Debug.Log("Ok");
    }

    // Récupération de l'id de la cellule dans laquelle se trouve le perso
    int getCharacterCellId()
    {
        return 0;
    }
}
