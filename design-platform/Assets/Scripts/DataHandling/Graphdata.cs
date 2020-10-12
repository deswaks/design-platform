using System;
using System.Linq;
using System.Collections.Generic;
using Neo4jClient;
using Neo4jClient.Extension.Cypher;
using Neo4jClient.Extension.Cypher.Attributes;

using UnitsNet;
using Neo4j.Driver.V1;
using UnityEngine;
//using System.Numerics;


namespace GraphStuff {
    public class GraphProgram {
        public static void WorkIt() {
            var db = new DatabaseConnection();

            // Sets up the data model, defining how the C# classes should be handled as Nodes in the graph
            NeoConfig.ConfigureDataModel();


            GraphRoom room1 = GetSampleRooms()[0];
            GraphRoom room2 = GetSampleRooms()[1];
            GraphRoom room3 = GetSampleRooms()[2];



            Debug.Log(Utils.StringListToVector3List(room1.vertices)[0]);
            
            //db.DeleteAllNodesWithLabel("Room");

            //db.CreateOrMergeRoom(room1, room2);

            //Room room = db.GetRoomById(85);
            //Console.WriteLine(room.ToString());

            //db.GetAllRooms();

            //Console.ReadKey();
        }

        public static List<GraphRoom> GetSampleRooms() {
            return new List<GraphRoom> {
            new GraphRoom{
                id = 25,
                name = "Køkken",
                area = 17.5f,
                type = RoomType.Kitchen,
                shape = RoomShape.Rectangular,
                vertices = new string[]{"(1.0;2.0;5.4)", "(3.0;2.0;1.0)"}
            },

            new GraphRoom{
                id = 15,
                name = "Soveværelse",
                area = 11.0f,
                type = RoomType.Bedroom,
                shape = RoomShape.Rectangular,
                vertices = new string[]{"(4.2;2.1;5.5)", "(5.6;2.4;0.1)"}
            },

            new GraphRoom{
                id = 85,
                name = "Stue",
                area = 42.5f,
                type = RoomType.LivingRoom,
                shape = RoomShape.UShaped
            }
        };
        }

    }
    public static class Utils {
        public static Vector3 StringToVector3(string pointString = "(0;0;0)", string seperator = ";") {
            int start = pointString.IndexOf("(");
            int firstSeperator = pointString.IndexOf(seperator);
            int secondSeperator = pointString.LastIndexOf(seperator);
            int end = pointString.IndexOf(")");

            Console.WriteLine("Point string to vectorize: " + pointString);

            float xVal = float.Parse(pointString.Substring(start + 1, firstSeperator - start - 1), System.Globalization.CultureInfo.InvariantCulture);
            float yVal = float.Parse(pointString.Substring(firstSeperator + 1, secondSeperator - firstSeperator - 1), System.Globalization.CultureInfo.InvariantCulture);
            float zVal = float.Parse(pointString.Substring(secondSeperator + 1, end - secondSeperator - 1), System.Globalization.CultureInfo.InvariantCulture);

            return new Vector3(xVal, yVal, zVal);
        }

        public static string[] Vector3ListToStringList(IEnumerable<Vector3> points) {
            List<string> stringPoints = new List<string>();
            points.ToList().ForEach(v => stringPoints.Add(
               "(" +
               v.x.ToString().Replace(",", ".") +
               ";" +
               v.y.ToString().Replace(",", ".") +
               ";" +
               v.z.ToString().Replace(",", ".") +
               ")"
               )
            );
            return stringPoints.ToArray();
        }
        public static List<Vector3> StringListToVector3List(IEnumerable<string> pointStrings) {
            List<Vector3> points = new List<Vector3>();
            if (pointStrings != null) {
                points = pointStrings.ToList().Select(p => Utils.StringToVector3(p)).ToList();
            }
            return points;
        }

    }

    public class DatabaseConnection {
        private readonly BoltGraphClient _graphClient;

        public DatabaseConnection() {
            //First create a 'Driver' instance.
            var driver = GraphDatabase.Driver("bolt://localhost:7687", Config.Builder.WithEncryptionLevel(EncryptionLevel.None).ToConfig());

            // Attach it to a bolt graph client
            _graphClient = new BoltGraphClient(driver);

            // Connect to the client
            _graphClient.Connect();
        }


        public void MergeAllRoomsToGraph() {

            List<Room> buildingRooms = Building.Instance.GetRooms();

            foreach(Room room in buildingRooms) {
                GraphRoom graphRoom = new GraphRoom {
                    var rand = new Random().;
                    
                    id = 2,
                    name = "Køkken",
                    area = 17.5f,
                    type = RoomType.Kitchen,
                    shape = RoomShape.Rectangular,
                    vertices = new string[] { "(1.0;2.0;5.4)", "(3.0;2.0;1.0)" }
                };

                CreateOrMergeRoom(graphRoom);

            }


        }


        // Create room in graph (or merge with existing: overwriting its parameters) 
        public void CreateOrMergeRoom(GraphRoom room) {
            _graphClient.Cypher
                .MergeEntity(room, "room")
                .ExecuteWithoutResults();
        }

        public void CreateOrMergeRoom(GraphRoom room, GraphRoom secondRoom) {
            _graphClient.Cypher
                .MergeEntity(room, "room")
                .MergeEntity(secondRoom, "secondRoom")
                .MergeRelationship(new AdjacentRoomRelationship("room", "secondRoom"))
                .ExecuteWithoutResults();

            Debug.Log("Done query stuff!");
        }

        public void CreateOrMergeAdjacentRelationship(GraphRoom room, GraphRoom secondRoom) {
            _graphClient.Cypher
                .MergeEntity(room, "room")
                .MergeEntity(secondRoom, "secondRoom")
                .MergeRelationship(new AdjacentRoomRelationship("room", "secondRoom"))
                .ExecuteWithoutResults();
        }

        public GraphRoom GetRoomById(int Id) {
            //string queryId = Id.ToString();

            GraphRoom foundRoom = _graphClient.Cypher
                .Match("( room:Room)")
                .Where<GraphRoom>(room => room.id == Id)
                .Return(room => room.As<GraphRoom>())
                .Results
                .First();

            return foundRoom;
        }
        public void GetAllRooms() {
            //string queryId = Id.ToString();
            GraphRoom r = new GraphRoom();
            List<GraphRoom> roomies = _graphClient.Cypher
                .Match("(room:Room)")
                .Return(room => room.As<GraphRoom>())
                .Results
                .ToList();
            ////////////////// VIRKER SEMI //////////////////////////////////////

            // DENNE METODE VIRKER
            //var query = _graphClient.Cypher
            //        .Match("(p:Room)")
            //        .Return(p => new { 
            //            ID = p.As<Room>().id,
            //            V = p.As<Room>().vertices 
            //        });

            //var results = query.Results.ToList();

            // DENNE METODE VIRKER OG RETURNERER NYE RUM MED EGENSKABER FRA GRAF
            //var query2 = _graphClient.Cypher
            //    .Match("(p:Room)")
            //    .Return(p => 
            //    new Room { 
            //        id = p.As<Room>().id,
            //        name = p.As<Room>().name,
            //        type = p.As<Room>().type,
            //        area = p.As<Room>().area,
            //        vertices = p.As<Room>().vertices,

            //});
            //var results2 = query2.Results.ToList();

            //foreach (var result in results)
            //{
            //    result.N.Properties.Keys.ToList().ForEach(k => Console.WriteLine(k));
            //    //nNodes.Add(JsonConvert.DeserializeObject(result.N.Properties) );
            //}

            //.Match("( room:Room)")
            //.Return(room => room.As<Room>())
            //.Results
            //.ToList();

        }



        // Deletes all nodes in the graph with the given label
        public void DeleteAllNodesWithLabel(string label = "") {
            _graphClient.Cypher
                .Match(String.Format("(r:{0})", label))
                .DetachDelete("r")
                .ExecuteWithoutResultsAsync();
        }



        public void QueryPersonStuff() {
            //var agent = SampleDataFactory.GetWellKnownPerson(7);

            var niklas = new Person {
                Id = 25,
                Name = "Anders Bomannowich",
                Sex = Gender.Male,
                HomeAddress = new Address { Street = "Rådsmandsgade 15", Suburb = "Nørrebro" },
                WorkAddress = new Address { Street = "Bredgade 25X", Suburb = "Lyngby" },
                IsOperative = true,
                SerialNumber = 123456,
                SpendingAuthorisation = 100.23m,
                DateCreated = DateTimeOffset.Parse("2015-07-11T08:00:00+10:00")
            };

            _graphClient.Cypher
                .CreateEntity(niklas, "niggus")
                .CreateEntity(niklas.HomeAddress, "homeAddress")
                .CreateEntity(niklas.WorkAddress, "workAddress")
                .CreateRelationship(new HomeAddressRelationship("niggus", "homeAddress"))
                .CreateRelationship(new WorkAddressRelationship("niggus", "workAddress"))
                .ExecuteWithoutResults();

            Console.WriteLine("Done query stuff!");

        }

    }


    /// LABELS / CLASSES /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    #region ourClasses

    public enum RoomShape {
        Rectangular,
        LShaped,
        UShaped
    }
    public enum RoomType {
        Preview,
        Default,
        Kitchen,
        LivingRoom,
        Bedroom
    }

    public class GraphRoom {
        public int id { get; set; }
        public string name { get; set; }
        public RoomShape shape { get; set; }
        public RoomType type { get; set; }
        public float area { get; set; }
        public DateTimeOffset dateCreated { get; set; }
        public string[] vertices { get; set; }

        public GraphRoom() {

        }

        public override string ToString() {
            return string.Format("Id: {0}, Name: {1}, Type: {2}", id, name, type.ToString("g"));
        }

        public List<Vector3> GetVerticesAsVector3() {
            return Utils.StringListToVector3List(vertices);
        }
    }



    /// RELATIONSHIPS /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    [CypherLabel(Name = LabelName)]
    public class AdjacentRoomRelationship : BaseRelationship {
        public const string LabelName = "ADJACENT_TO";
        public AdjacentRoomRelationship(string from = null, string to = null)
            : base(from, to) {
        }
    }

    #endregion ourClasses


    #region workingSampleClasses
    /// <summary>
    /// Contains value types and one complex type
    /// </summary>
    public class Person {
        /// <summary>
        /// Primary key seeded from else where
        /// </summary>
        public int Id { get; set; }
        public string Name { get; set; }
        public Gender Sex { get; set; }
        public string Title { get; set; }
        public Address HomeAddress { get; set; }
        public Address WorkAddress { get; set; }
        public bool IsOperative { get; set; }
        public int SerialNumber { get; set; }
        public Decimal SpendingAuthorisation { get; set; }
        public DateTimeOffset DateCreated { get; set; }


        public Person() {
            HomeAddress = new Address();
            WorkAddress = new Address();
        }

        public override string ToString() {
            return string.Format("Id={0}, Name={1}", Id, Name);
        }
    }

    public class Weapon {
        public int Id { get; set; }

        public string Name { get; set; }

        public Area BlastRadius { get; set; }
        /// <summary>
        /// Test unusal types 
        /// </summary>

    }
    public class Address {
        public string Street { get; set; }

        public string Suburb { get; set; }


        public override string ToString() {
            return string.Format("Street='{0}', Suburb='{1}'", Street, Suburb);
        }
    }

    public enum Gender {
        Unspecified = 0,
        Male,
        Female
    }




    [CypherLabel(Name = "HAS_CHECKED_OUT")]
    public class CheckedOutRelationship : BaseRelationship {
        public CheckedOutRelationship() : base("agent", "weapon") {

        }
    }

    [CypherLabel(Name = LabelName)]
    public class HomeAddressRelationship : BaseRelationship {
        public const string LabelName = "HOME_ADDRESS";

        public HomeAddressRelationship(DateTimeOffset effective, string from = "agent", string to = "address")
            : base(from, to) {
            DateEffective = effective;
        }

        public HomeAddressRelationship(string from = "person", string to = "address")
            : base(from, to) {
        }

        public HomeAddressRelationship(string relationshipIdentifier, string from, string to)
          : base(relationshipIdentifier, from, to) {
        }

        public DateTimeOffset DateEffective { get; set; }
    }

    [CypherLabel(Name = LabelName)]
    public class WorkAddressRelationship : BaseRelationship {
        public const string LabelName = "WORK_ADDRESS";
        public WorkAddressRelationship(string from = null, string to = null)
            : base(from, to) {
        }
    }



    public class SampleDataFactory {
        public static Person GetWellKnownPerson(int n) {
            var archer = new Person {
                Id = n,
                Name = "Sterling Archer",
                Sex = Gender.Male,
                HomeAddress = GetWellKnownAddress(200),
                WorkAddress = GetWellKnownAddress(59),
                IsOperative = true,
                SerialNumber = 123456,
                SpendingAuthorisation = 100.23m,
                DateCreated = DateTimeOffset.Parse("2015-07-11T08:00:00+10:00")
            };

            return archer;
        }

        public static Address GetWellKnownAddress(int n) {
            var address = new Address { Street = n + " Isis Street", Suburb = "Fakeville" };
            return address;
        }

        public static Weapon GetWellKnownWeapon(int n) {
            var weapon = new Weapon();
            weapon.Id = n;
            weapon.Name = "Grenade Launcher";
            return weapon;
        }
    }

    #endregion workingSampleClasses


    public class NeoConfig {
        // Sets up the data model, defining how the C# classes should be handled as Nodes in the graph
        //public static void ConfigureModel() {
        //    FluentConfig.Config()
        //       .With<Person>("Personus")
        //       .Match(x => x.Id)
        //       .Merge(x => x.Id)
        //       .MergeOnCreate(p => p.Id)
        //       .MergeOnCreate(p => p.DateCreated)
        //       .MergeOnMatchOrCreate(p => p.Title)
        //       .MergeOnMatchOrCreate(p => p.Name)
        //       .MergeOnMatchOrCreate(p => p.IsOperative)
        //       .MergeOnMatchOrCreate(p => p.Sex)
        //       .MergeOnMatchOrCreate(p => p.SerialNumber)
        //       .MergeOnMatchOrCreate(p => p.SpendingAuthorisation)
        //       .Set();

        //    FluentConfig.Config()
        //        .With<Address>()
        //        .MergeOnMatchOrCreate(a => a.Street)
        //        .MergeOnMatchOrCreate(a => a.Suburb)
        //        .Set();

        //    FluentConfig.Config()
        //        .With<Weapon>()
        //        .Match(x => x.Id)
        //        .Merge(x => x.Id)
        //        .MergeOnMatchOrCreate(w => w.Name)
        //        .Set();

        //    FluentConfig.Config()
        //        .With<HomeAddressRelationship>()
        //        .Match(ha => ha.DateEffective)
        //        .MergeOnMatchOrCreate(hr => hr.DateEffective)
        //        .Set();

        //    FluentConfig.Config()
        //       .With<WorkAddressRelationship>()
        //       .Set();
        //}

        public static void ConfigureDataModel() {
            FluentConfig.Config()
               .With<GraphRoom>("Room") //With<Class>("Label")
               .Match(x => x.id)
               .Merge(x => x.id)
               .MergeOnCreate(p => p.id)
               .MergeOnCreate(p => p.dateCreated)
               .MergeOnMatchOrCreate(p => p.name)
               .MergeOnMatchOrCreate(p => p.area)
               .MergeOnMatchOrCreate(p => p.shape)
               .MergeOnMatchOrCreate(p => p.type)
               .MergeOnMatchOrCreate(p => p.vertices)


               .Set();

            FluentConfig.Config()
               .With<AdjacentRoomRelationship>()
               .Set();
        }
    }


}
