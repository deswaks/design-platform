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
//using System.Numerics;


namespace Database {
    
    /// CLASSES / LABELS /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    public class RoomNode {
        public int id { get; set; }
        public string name { get; set; }
        public RoomShape shape { get; set; }
        public RoomType type { get; set; }
        public float area { get; set; }
        public DateTimeOffset dateCreated { get; set; }
        public string[] vertices { get; set; }

        public RoomNode() {}

        public override string ToString() {
            return string.Format("Id: {0}, Name: {1}, Type: {2}, Shape: {3}", id, name, type.ToString("g"),shape.ToString());
        }

        public List<Vector3> GetVerticesAsVector3() {
            return GraphUtils.StringListToVector3List(vertices);
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



}
