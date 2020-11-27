using DesignPlatform.Core;
using DesignPlatform.Geometry;
using Neo4jClient.Extension.Cypher;
using Neo4jClient.Extension.Cypher.Attributes;
using System;
using System.Collections.Generic;
using UnitsNet;
using UnityEngine;
//using System.Numerics;


namespace DesignPlatform.Database {

    /// <summary>
    /// 
    /// </summary>
    public class SpaceNode {
        public int id { get; set; }
        public string name { get; set; }
        public SpaceShape shape { get; set; }
        public SpaceFunction type { get; set; }
        public float area { get; set; }
        public DateTimeOffset dateCreated { get; set; }
        public string[] vertices { get; set; }

        public SpaceNode() { }

        public override string ToString() {
            return string.Format("Id: {0}, Name: {1}, Type: {2}, Shape: {3}", id, name, type.ToString("g"), shape.ToString());
        }

        public List<Vector3> GetVerticesAsVector3() {
            return GraphUtils.StringListToVector3List(vertices);
        }
    }

    /// <summary>
    /// 
    /// </summary>
    public class OpeningNode {
        public OpeningFunction openingShape { get; set; } // Door or window
        public string position { get; set; }
        public string rotation { get; set; }
        public float width { get; set; }
        public float height { get; set; }
    }

    /// <summary>
    /// 
    /// </summary>
    public class WallElementNode {
        public string[] vertices { get; set; }
        public string startJointType { get; set; }
        public string endJointType { get; set; }
        public string[] midPointJointTypes { get; set; }
    }


    /// <summary>
    /// 
    /// </summary>
    [CypherLabel(Name = LabelName)]
    public class AdjacentSpaceRelationship : BaseRelationship {
        public const string LabelName = "ADJACENT_TO";
        public AdjacentSpaceRelationship(string from = null, string to = null)
            : base(from, to) {
        }
    }

    /// <summary>
    /// 
    /// </summary>
    [CypherLabel(Name = LabelName)]
    public class InterfaceSpaceRelationship : BaseRelationship {
        public const string LabelName = "INTERFACE_OF";
        public InterfaceSpaceRelationship(string from = null, string to = null)
            : base(from, to) {
        }
    }
}
