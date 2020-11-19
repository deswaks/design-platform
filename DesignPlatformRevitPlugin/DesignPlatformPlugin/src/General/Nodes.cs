using DesignPlatform.Database;
using System;
using System.Collections.Generic;
using System.Numerics;

namespace DesignPlatform.Core {
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

    public class WallElementNode {
        public string[] vertices { get; set; }
        public string startJointType { get; set; }
        public string endJointType { get; set; }
        public string[] midPointJointTypes { get; set; }
    }
}