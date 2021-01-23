using System;
using System.Linq;
using System.Collections.Generic;
using Neo4jClient;
using Neo4jClient.Extension.Cypher;
using Neo4jClient.Extensions;
using Neo4jClient.Extension.Cypher.Attributes;

using UnitsNet;
using Neo4j.Driver.V1;
using Neo4j.Driver;
using UnityEngine;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System.IO;
using DesignPlatform.Core;


//using System.Numerics;


namespace DesignPlatform.Database {
    /// <summary>
    /// Contains functions for reading/writing from/to graph database.
    /// </summary>
    public class GraphDatabase {

        private static GraphDatabase instance;
        private readonly BoltGraphClient _graphClient;

        /// <summary>
        /// Main GraphDatabase instance. All access to the database should use this single instance.
        /// </summary>
        public static GraphDatabase Instance {
            // Use the ?? operator, to return 'instance' if 'instance' does not equal null
            // otherwise we assign instance to a new component and return that
            get { return instance ?? (instance = new GraphDatabase()); }
        }

        /// <summary>
        /// Establishes connection to Neo4j graph database at "bolt://localhost:7687" using no credentials.
        /// </summary>
        public GraphDatabase() {
            //First create a 'Driver' instance.
            var driver = Neo4j.Driver.V1.GraphDatabase.Driver("bolt://localhost:7687", Config.Builder.WithEncryptionLevel(EncryptionLevel.None).ToConfig());

            // Attach it to a bolt graph client
            _graphClient = new BoltGraphClient(driver);

            // Connect to the client
            _graphClient.Connect();

        }

        /// <summary>
        /// Finds Spaces in graph and creates corresponding spaces in Unity
        /// </summary>
        public void LoadAndBuildUnitySpacesFromGraph() {
            // LOAD AND BUILD ALL ROOMS FROM GRAPH
            foreach (int id in GetAllSpaceIdsInGraph()) {
                SpaceNode SpaceNode = GetSpaceNodeById(id);
                LocalDatabase.CreateUnitySpaceFromSpaceNode(SpaceNode);
            }
        }

        /// <summary>
        /// Reads SpaceNodes specified in the json file and merges them into graph
        /// </summary>
        /// <param name="savePath">Full path of json file, with backslashes.</param>
        public void ImportJsonSpaceNodesToGraph(string jsonPath = null) {
            jsonPath = jsonPath != null ? jsonPath : Settings.SaveFolderPath+"SpaceNodes.json";

            IEnumerable<SpaceNode> spaceNodes = LocalDatabase.LoadSpaceNodesFromJson(jsonPath);

            foreach (SpaceNode spaceNode in spaceNodes) {
                CreateOrMergeSpaceNode(spaceNode);
            }
        }

        /// <summary>
        /// Retrieves a list of all Space nodes in graph.
        /// </summary>
        /// <returns>A list of all Space nodes in graph.</returns>
        public List<SpaceNode> GetAllSpaceNodes() {

            List<SpaceNode> SpaceNodes = _graphClient.Cypher
                .Match("(space:Space)")
                .Return(space => space.As<SpaceNode>())
                .Results
                .ToList();

            return SpaceNodes;

        }

        /// <summary>
        /// Finds and returns list of IDs for all spaces in graph
        /// </summary>
        /// <returns>List of IDs of all spaces in graph</returns>
        public List<int> GetAllSpaceIdsInGraph() {

            var query = _graphClient.Cypher
                        .Match("(p:Space)")
                        .Return(p =>
                            p.As<SpaceNode>().id
            );
            var results = query.Results.ToList();

            return results.Select(r => Convert.ToInt32(r)).ToList();
        }

        /// <summary>
        /// Saves/inserts all spaces from Unity into graph database
        /// </summary>
        public void PushAllUnitySpacesToGraph() {
            // Finds all spaces currently in Unity project
            Debug.Log(Building.Instance.Spaces.Count);

            foreach (Core.Space space in Building.Instance.Spaces) {
                System.Random rd = new System.Random();
                SpaceNode SpaceNode = new SpaceNode {
                    id = rd.Next(0, 5000),                              /////////////////////////// SKAL OPDATERES
                    name = space.Shape.ToString().ToLower(),    /////////////////////////// SKAL OPDATERES
                    area = 17.5f,                                       /////////////////////////// SKAL OPDATERES
                    type = space.Function,
                    shape = space.Shape,
                    vertices = GraphUtils.Vector3ListToStringList(space.GetControlPoints())
                };

                CreateOrMergeSpaceNode(SpaceNode);
            }
        }


        /// <summary>
        /// Create space in graph (or merge with existing: overwriting its parameters)  
        /// </summary>
        public void CreateOrMergeSpaceNode(SpaceNode space) {

            _graphClient.Cypher
                .MergeEntity(space, "space")
                .ExecuteWithoutResults();
        }


        /// <summary>
        /// Create adjacency relationship between two spaces
        /// </summary>
        public void CreateOrMergeAdjacentRelationship(SpaceNode space, SpaceNode secondSpace) {
            _graphClient.Cypher
                .MergeEntity(space, "space")
                .MergeEntity(secondSpace, "secondSpace")
                .MergeRelationship(new AdjacentSpaceRelationship("space", "secondSpace"))
                .ExecuteWithoutResults();
        }


        /// <summary>
        /// Finds a space in the graph based on its Id
        /// </summary>
        /// <param name="Id">Id to search for</param>
        /// <returns>SpaceNode with given id</returns>
        public SpaceNode GetSpaceNodeById(int Id) {
            //string queryId = Id.ToString();

            SpaceNode foundSpace = _graphClient.Cypher
                .Match("( space:Space)")
                .Where<SpaceNode>(space => space.id == Id)
                .Return(space => space.As<SpaceNode>())
                .Results
                .First();
            return foundSpace;
        }

        /// <summary>
        /// Deletes all nodes in the graph with the given label
        /// </summary>
        /// <param name="label">Label of nodes to delete</param>
        public void DeleteAllNodesWithLabel(string label = "") {
            _graphClient.Cypher
                .Match(string.Format("(r:{0})", label))
                .DetachDelete("r")
                .ExecuteWithoutResultsAsync();
        }
    }
}
