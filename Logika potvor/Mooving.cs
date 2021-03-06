﻿using System;
using System.Collections.Generic;

namespace DefendersOfH6
{
    public class Mooving : Status
    {

        public static string NULL_POSITION = "creature position is null";
        public static string NULL_DESTINATION = "final destination is null";
        public static string NULL_GRAPH = "graph is null";
        public static string NULL_GRAPH_NODES = "graph nodes are null";
        public static string INCORRECT_GRAPH_DISTANCE = "graph distence is less or equal then 0";
        public static string PATH_DOES_NOT_EXISTS = "path does not exists";

        private ICreature creature;
        private Node finalDestinantion;
        private Graph graph;

        private Node nextPosition;
        public Node NextPosition { get { return this.nextPosition; } }

        public Mooving(ICreature creature, Node finalDestinantion, Graph graph)
        {
            this.creature = creature;
            this.finalDestinantion = finalDestinantion;
            this.graph = graph;
            this.nextPosition = null;
        }
        
        public Status changeStatus()
        {
            if (this.creature.isDead())
            {
                return this.creature.getDying();
            }

            foreach(Node neighbour in this.creature.getPosition().getNeighbours())
            {
                if(neighbour == finalDestinantion)
                {
                    return this.creature.getShooting();
                }
            }
            return null;
        }

        public void onStart()
        {
            if(this.creature.getPosition() == null)
            {
                throw new NullReferenceException(NULL_POSITION);
            }
            if(this.finalDestinantion == null)
            {
                throw new NullReferenceException(NULL_DESTINATION);
            }
            if (this.graph == null)
            {
                throw new NullReferenceException(NULL_GRAPH);
            }
            if (this.graph.getNodes() == null)
            {
                throw new NullReferenceException(NULL_GRAPH_NODES);
            }
            if( this.graph.getDistance() <= 0)
            {
                throw new InvalidOperationException(INCORRECT_GRAPH_DISTANCE);
            }
        }

        public void prepare()
        {
            List<Node> path = new Dijkstra(finalDestinantion, creature.getPosition(), graph)
                                    .initialization()
                                    .algorithm()
                                    .getPath();
            if (path == null)
            {
                throw new InvalidOperationException(PATH_DOES_NOT_EXISTS);
            }
            nextPosition = path[0];
        }

        public void onEnd()
        {
        }
        
        private class Dijkstra
        {

            private Node finalDestinaton;
            private Node position;
            private Graph graph;

            private List<Node> nodes;
            private List<Node> path;
            private Dictionary<Node, Node> previous;
            private Dictionary<Node, int> distances;


            public Dijkstra(Node finalDestinaton, Node position, Graph graph)
            {
                this.finalDestinaton = finalDestinaton;
                this.position = position;
                this.graph = graph;

                this.nodes = new List<Node>();
                this.previous = new Dictionary<Node, Node>();
                this.distances = new Dictionary<Node, int>();
            }

            public Dijkstra initialization()
            {
                foreach (Node node in graph.getNodes())
                {
                    if (node == position)
                    {
                        distances[node] = 0;
                    }
                    else
                    {
                        if (node.isEnable())
                        {
                            distances[node] = int.MaxValue;
                        }
                    }
                    nodes.Add(node);
                }
                return this;
            }

            public List<Node> getPath()
            {
                return this.path;
            }

            public Dijkstra algorithm()
            {
                while (nodes.Count != 0)
                {
                    nodes.Sort((x, y) => distances[x] - distances[y]);

                    Node smallest = nodes[0];
                    nodes.Remove(smallest);

                    if (isDestination(smallest))
                    {
                        break;
                    }
                    if (distances[smallest] == int.MaxValue)
                    {
                        break;
                    }
                    calcDistanceForNeighbors(smallest);
                }
                return this;
            }

            private Boolean isDestination(Node smallest)
            {
                if (smallest == finalDestinaton)
                {
                    path = new List<Node>();
                    while (previous.ContainsKey(smallest))
                    {
                        path.Add(smallest);
                        smallest = previous[smallest];
                    }
                    return true;
                }
                return false;
            }

            private void calcDistanceForNeighbors(Node smallest)
            {
                foreach (Node neighbor in smallest.getNeighbours())
                {
                    int alt = distances[smallest] + graph.getDistance();
                    if (alt < distances[neighbor])
                    {
                        distances[neighbor] = alt;
                        previous[neighbor] = smallest;
                    }
                }
            }
        }
    }
}
