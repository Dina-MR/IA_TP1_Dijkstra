using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using Random = UnityEngine.Random;

public class Graph : MonoBehaviour
{
    /*
     * VARIABLES
     */

    [SerializeField]
    int nodesNumber; // nombre de noeuds
    [SerializeField]
    Vector3 positionMin; // position minimale d'un noeud
    [SerializeField]
    Vector3 positionMax; // position maximale d'un noeud
    [SerializeField]
    Color defaultColorNode; // couleur par d�faut des noeuds
    [SerializeField]
    float verticeMaxValue; // poids maximum d'une ar�te

    // Liste des sommets du graphe
    List<Node> nodes;
    // Matrice d'adjacence
    float[,] adjacencyMatrix;

    /*
     * FONCTIONS
     */

    // Start is called before the first frame update
    void Start()
    {
        nodes = generateRandomNodes();
        adjacencyMatrix = generateRandomVertices();
        Debug.Log("Nombre de noeuds cr��s : " + nodes.Count);
        Debug.Log("Nombre d'ar�tes cr��es : " + adjacencyMatrix.Length);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    // G�n�rateur al�atoire de noeuds
    List<Node> generateRandomNodes()
    {
        List<Node> newNodes = new List<Node>();
        for(int i = 0; i < nodesNumber; i++)
        {
            Node newNode = new Node(i, positionMin, positionMax, defaultColorNode);
            newNode.nodeDebug();
            newNode.drawNode();
            newNodes.Add(newNode);
        }
        return newNodes;
    }

    // F�n�rateur al�atoire d'ar�tes pond�r�es
    float[,] generateRandomVertices()
    {
        float[,] newVertices = new float[nodes.Count, nodes.Count];

        for (int i = 0; i < nodes.Count; i++)
            for (int j = i; j < nodes.Count; j++)
            {
                if (i == j)
                    newVertices[i,j] = 0;
                else
                    newVertices[i,j] = Random.Range(0, verticeMaxValue);
                Debug.Log("Ar�te n� " + (i * 10 + j) + " cr��e. Poids : " + newVertices[i, j]);
            }

        return newVertices;
    }

    // Affichage de tous les noeuds
    void drawAllNodes()
    {
        foreach(Node node in nodes)
            node.drawNode();
    }

    // Affichage de l'ensemble des ar�tes non nulles
    void drawAllVertices()
    {
        List<int> indexes = new List<int>();

        for (int i = 0; i < nodes.Count; i++)
            for(int j = 0; j < nodes.Count; j++)
            {
                // On cr�� ces indices afin de v�rifier si l'ar�te a d�j� �t� dessin�e ou non
                int indexValue1 = i * 10 + j;
                int indexValue2 = j * 10 + i;
                // Si un des deux indices existe dans la liste des indices, leur somme sera sup�rieure � -2
                if ((adjacencyMatrix[i, j] != 0 || adjacencyMatrix[j, i] != 0) && (indexes.IndexOf(indexValue1) - indexes.IndexOf(indexValue2) == -2))
                {
                    Node start = nodes[i];
                    Node end = nodes[j];
                    drawVertice(start, end, indexValue1, Color.green, 2.0f);
                }
            }
    }

    // Affichage d'une ar�te d'un graphe
    void drawVertice(Node start, Node end, int index, Color color, float width)
    {
        LineRenderer verticeRenderer = new GameObject().AddComponent<LineRenderer>();

        // Coloration de l'ar�te
        verticeRenderer.startColor = color;
        verticeRenderer.endColor = color;

        // Epaisseur
        verticeRenderer.startWidth = width;
        verticeRenderer.endWidth = width;

        // Positionnement de l'ar�te � partir des deux noeuds
        verticeRenderer.SetPosition(index, start.position);
        verticeRenderer.SetPosition(index, end.position);
    }

    /*
     * SOUS-CLASSES
     */

    // Classe repr�sentant un noeud/sommet du graphe
    public class Node
    {
        /*
         * A RAJOUTER : Instanciate OU AddComponent
         */

        int id;
        public Vector3 position;
        Color color;

        // Constructeurs
        public Node(int _id, Vector3 _position, Color _color)
        {
            id = _id;
            position = _position;
            color = _color;
        }

        public Node(int _id, Vector3 _positionMin, Vector3 _positionMax, Color _color)
        {
            id = _id;
            color = _color;

            // Randomisation des positions
            float x = Random.Range(_positionMin.x, _positionMax.x);
            float y = Random.Range(_positionMin.y, _positionMax.y);
            //float z = Random.Range(_positionMin.z, _positionMax.z);
            position = new Vector3(x, y, 0); // z a une valeur nulle par d�faut
        }

        //D�buggeurs
        public void nodeDebug()
        {
            Debug.Log("Noeud n� " + id + ", position : " + position);
        }

        // Affichage du noeud sous forme de rectangle
        public void drawNode()
        {
            Debug.Log(position.x);
            EditorGUI.DrawRect(new Rect(0, 0, 10, 10), Color.red);
            //EditorGUI.DrawRect(new Rect(position.x, position.y, 10, 10), color);
        }
    }
}
