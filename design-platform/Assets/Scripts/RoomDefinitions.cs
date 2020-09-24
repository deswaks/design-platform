//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;

//public class RoomDefinitions
//{
//    public static void GetShape() {

//    }
//    List<Vector3> points;
//        if (shape == 0){ // Rectangle vertices
//            points = new List<Vector3> {new Vector3(0, 0, 0),
//                                        new Vector3(0, 0, 3),
//                                        new Vector3(3, 0, 3),
//                                        new Vector3(3, 0, 0)};
//roomShapeType = RoomShapeTypes.Rectangular;
//        }
//         else { // L-shape vertices
//            points = new List<Vector3> {new Vector3(0, 0, 0),
//                                        new Vector3(0, 0, 5),
//                                        new Vector3(3, 0, 5),
//                                        new Vector3(3, 0, 3),
//                                        new Vector3(5, 0, 3),
//                                        new Vector3(5, 0, 0)};
//            roomShapeType = RoomShapeTypes.L_Shaped;
//        }

//            switch (roomShapeType)
//        {
//            case Room.RoomShapeTypes.Rectangular:
//                //colliderIndexPairsList.Add(new List<int> { x1, x2, y1, y2 });
//                colliderIndexPairsList.Add( new List<int> { 1, 2, 0, 1 } ); 

//                break;
//            case Room.RoomShapeTypes.L_Shaped:
//                colliderIndexPairsList.Add(new List<int> { 1, 2, 0, 1 }); // Collider cube 1
//                colliderIndexPairsList.Add(new List<int> { 3, 4, 0, 3 }); // Collider cube 2
//                break;
//        }

//}
