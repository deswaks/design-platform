using DesignPlatform.Core;
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
    public class RoomNode {
        public int id { get; set; }
        public string name { get; set; }
        public RoomShape shape { get; set; }
        public RoomType type { get; set; }
        public float area { get; set; }
        public DateTimeOffset dateCreated { get; set; }
        public string[] vertices { get; set; }

        public RoomNode() { }

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
        public OpeningShape openingShape { get; set; } // Door or window
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
    public class AdjacentRoomRelationship : BaseRelationship {
        public const string LabelName = "ADJACENT_TO";
        public AdjacentRoomRelationship(string from = null, string to = null)
            : base(from, to) {
        }
    }

    /// <summary>
    /// 
    /// </summary>
    [CypherLabel(Name = LabelName)]
    public class InterfaceRoomRelationship : BaseRelationship {
        public const string LabelName = "INTERFACE_OF";
        public InterfaceRoomRelationship(string from = null, string to = null)
            : base(from, to) {
        }
    }
}
