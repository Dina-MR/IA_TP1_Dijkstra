using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static GridGenerator;

public class DijkstraPathFiding : MonoBehaviour
{
    // Variables liées au personnage et sa cible
    public GameObject character;
    public GameObject target;

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
    }

    // Fonction principale
    void PathFinding(int sommetStart)
    {
        Initialization(sommetStart);
        List<Cell> cellsCopy = grid.gridCells;
        while(cellsCopy.Count > 0)
        {
            int sommet1 = minimumCell(cellsCopy);
            cellsCopy.RemoveAt(sommet1);
            // A RAJOUTER - LES VOISINS
        }
    }

    // Fonction d'intialisation des distances
    void Initialization(int characterIndex)
    {
        foreach(GridGenerator.Cell cell in grid.gridCells)
        {
            distances[cell.id] = float.MaxValue; 
        }
        distances[characterIndex] = 0;
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
                minimum = distances[sommet];
                sommet = cell.id;
            }
        }
        return sommet;
    }

    // Mise à jour des distances
    void updateDistances(int sommet1, int sommet2)
    {
        float poids = grid.getCellsDistance(sommet1, sommet2);
        if (distances[sommet2] > distances[sommet1] + poids)
        {
            distances[sommet2] = distances[sommet1] + poids;
            previousIndexes[sommet2] = sommet1;
        }
    }

    // Récupération de l'id de la cellule dans laquelle se trouve le perso
    int getCharacterCellId()
    {
        return 0;
    }
}
