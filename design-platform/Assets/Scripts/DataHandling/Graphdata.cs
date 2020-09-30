﻿//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;
//using Neo4j.Driver;
//using System;
//using Neo4jClient;
//using System.Linq;
//using Neo4jClient;
////using Newtonsoft.Json.Serialization;

////public class Graphdata {

////    public async void CreateNode() {
////        var client = new GraphClient(rootUri: new Uri("bolt://localhost:7687"), username: "neo4j", password: "test");

////        await client.ConnectAsync();

////        var agencyA = new Movie { Name = "Agency-A" };
////        client.Cypher
////            .Create("(agency:Agency {agencyA})")
////            .WithParam("agencyA", agencyA)
////            .ExecuteWithoutResultsAsync()
////            .Wait();

////    }
////}



////using System.Linq;
////using Neo4jClient;
////using Newtonsoft.Json.Serialization;

//namespace Neo4jClientSample {
//    /*
//        CREATE (AgencyA:Agency {name: 'Agency-A'})
//        CREATE (Actor1:Person {name: 'Actor-1'})
//        CREATE (Actor2:Person {name: 'Actor-2'})
//        CREATE (Actor3:Person {name: 'Actor-3'})
//        CREATE (Actor4:Person {name: 'Actor-4'})
//        CREATE (Actor5:Person {name: 'Actor-5'})
//        CREATE 
//          (AgencyA)-[:ACQUIRED]->(Actor1), 
//          (AgencyA)-[:ACQUIRED]->(Actor3), 
//          (AgencyA)-[:ACQUIRED]->(Actor5)
//        CREATE (MovieA:Movie {name: "Movie-A"})
//        CREATE (MovieB:Movie {name: "Movie-B"})
//        CREATE (MovieC:Movie {name: "Movie-C"})
//        CREATE 
//          (MovieA)-[:EMPLOYED]->(Actor1),
//          (MovieA)-[:EMPLOYED]->(Actor5),
//          (MovieB)-[:EMPLOYED]->(Actor1),
//          (MovieB)-[:EMPLOYED]->(Actor3),
//          (MovieB)-[:EMPLOYED]->(Actor5),
//          (MovieC)-[:EMPLOYED]->(Actor2),
//          (MovieC)-[:EMPLOYED]->(Actor5)
        
//        MATCH (agency:Agency { name:"Agency-A" })-[:ACQUIRED]->(actor:Person)<-[:EMPLOYED]-(movie:Movie)
//        WITH DISTINCT movie, collect(actor) AS actors
//        MATCH (movie)-[:EMPLOYED]->(allemployees:Person)
//        WITH movie, actors, count(allemployees) AS c
//        WHERE c = size(actors)
//        RETURN movie.name
//        Shoul return:
//        Movie-A
//        Movie-B
//        see: http://console.neo4j.org/?id=s9t6en
//    */

//    public class Agency {
//        public string Name { get; set; }
//    }

//    public class Person {
//        public string Name { get; set; }
//    }

//    public class Movie {
//        public string Name { get; set; }
//    }

//    public class Program {
//        // https://github.com/Readify/Neo4jClient/wiki
//        public static async void Muhmuh() {
//            // https://github.com/Readify/Neo4jClient/wiki/connecting#graph-client-basics
//            // https://github.com/Readify/Neo4jClient/wiki/connecting#threading-and-lifestyles
//            var client = new GraphClient(new Uri("http://localhost:7474/db/data"), "neo4j", "test"); 
//            //{
//            //    JsonContractResolver = new CamelCasePropertyNamesContractResolver()
//            //};

//            await client.ConnectAsync();

//            // clean up /w relationships
//            // https://github.com/Readify/Neo4jClient/wiki/cypher-examples#delete-a-user
//            // https://github.com/Readify/Neo4jClient/wiki/cypher-examples#create-a-user-and-relate-them-to-an-existing-one

//            client.Cypher
//                .OptionalMatch("(actor:Person)<-[r]-()")
//                .Delete("r, actor")
//                .ExecuteWithoutResultsAsync()
//                .Wait();

//            client.Cypher
//                .Match("(agency:Agency)")
//                .Delete("agency")
//                .ExecuteWithoutResultsAsync()
//                .Wait();

//            client.Cypher
//                .Match("(movie:Movie)")
//                .Delete("movie")
//                .ExecuteWithoutResultsAsync()
//                .Wait();

//            // create
//            // see: https://github.com/Readify/Neo4jClient/wiki/cypher-examples#create-a-user-and-relate-them-to-an-existing-one

//            var agencyA = new Agency { Name = "Agency-A" };
//            client.Cypher
//                .Create("(agency:Agency {agencyA})")
//                .WithParam("agencyA", agencyA)
//                .ExecuteWithoutResultsAsync()
//                .Wait();

//            for (int i = 1; i <= 5; i++) {
//                var actor = new Person { Name = $"Actor-{i}" };

//                if ((i % 2) == 0) {
//                    client.Cypher
//                        .Create("(actor:Person {newActor})")
//                        .WithParam("newActor", actor)
//                        .ExecuteWithoutResultsAsync()
//                        .Wait();
//                }
//                else {
//                    // create and relate
//                    // see: https://github.com/Readify/Neo4jClient/wiki/cypher-examples#create-a-user-and-relate-them-to-an-existing-one

//                    client.Cypher
//                        .Match("(agency:Agency)")
//                        .Where((Agency agency) => agency.Name == agencyA.Name)
//                        .Create("agency-[:ACQUIRED]->(actor:Person {newActor})")
//                        .WithParam("newActor", actor)
//                        .ExecuteWithoutResultsAsync()
//                        .Wait();
//                }
//            }

//            // see: http://stackoverflow.com/questions/314466/generating-an-array-of-letters-in-the-alphabet-in-c-sharp
//            char[] chars = Enumerable.Range('a', 'z' - 'a' + 1).Select(i => (Char)i).ToArray();
//            for (int i = 0; i < 3; i++) {
//                var movie = new Movie { Name = $"Movie-{chars[i]}" };

//                client.Cypher
//                    .Create("(movie:Movie {newMovie})")
//                    .WithParam("newMovie", movie)
//                    .ExecuteWithoutResultsAsync()
//                    .Wait();
//            }

//            // relate actors to movies
//            // see: https://github.com/Readify/Neo4jClient/wiki/cypher-examples#relate-two-existing-users

//            client.Cypher
//                .Match("(movie:Movie)", "(actor1:Person)", "(actor5:Person)")
//                .Where((Movie movie) => movie.Name == "Movie-a")
//                .AndWhere((Person actor1) => actor1.Name == "Actor-1")
//                .AndWhere((Person actor5) => actor5.Name == "Actor-5")
//                .Create("(movie)-[:EMPLOYED]->(actor1), (movie)-[:EMPLOYED]->(actor5)")
//                .ExecuteWithoutResultsAsync()
//                .Wait();

//            client.Cypher
//                .Match("(movie:Movie)", "(actor1:Person)", "(actor3:Person)", "(actor5:Person)")
//                .Where((Movie movie) => movie.Name == "Movie-b")
//                .AndWhere((Person actor1) => actor1.Name == "Actor-1")
//                .AndWhere((Person actor3) => actor3.Name == "Actor-3")
//                .AndWhere((Person actor5) => actor5.Name == "Actor-5")
//                .Create("(movie)-[:EMPLOYED]->(actor1), (movie)-[:EMPLOYED]->(actor3), (movie)-[:EMPLOYED]->(actor5)")
//                .ExecuteWithoutResultsAsync()
//                .Wait();

//            client.Cypher
//                .Match("(movie:Movie)", "(actor2:Person)", "(actor5:Person)")
//                .Where((Movie movie) => movie.Name == "Movie-c")
//                .AndWhere((Person actor2) => actor2.Name == "Actor-2")
//                .AndWhere((Person actor5) => actor5.Name == "Actor-5")
//                .Create("(movie)-[:EMPLOYED]->(actor2), (movie)-[:EMPLOYED]->(actor5)")
//                .ExecuteWithoutResultsAsync()
//                .Wait();

//            /*
//                Fining the actors of Movie-a:
//                OPTIONAL MATCH (movie:Movie)-[EMPLOYED]-(actor:Person)
//                WHERE movie.Name = "Movie-a"
//                RETURN movie, collect(actor) as Actors
//            */

//            /*      
//                MATCH (agency:Agency)-[:ACQUIRED]->(actor:Person)<-[:EMPLOYED]-(movie:Movie)
//                RETURN agency, actor, movie
//            */

//            var results = client.Cypher
//                .Match("(agency:Agency)-[:ACQUIRED]->(actor:Person)<-[:EMPLOYED]-(movie:Movie)")
//                .Return((agency, actor, movie) => new {
//                    Agency = agency.As<Agency>(),
//                    Actor = actor.As<Person>(),
//                    Movie = movie.As<Movie>()
//                }).ResultsAsync;

//            // this is retrievable using the below now
//            // MATCH(agencyA: Agency)
//            // RETURN agencyA
//        }
//    }
//}

////public class Graphdata {

////    public async void CreateNode() {


////        IDriver driver = GraphDatabase.Driver("bolt://localhost:7687", AuthTokens.Basic("neo4j", "test"));


////        //IAsyncSession session = _driver.AsyncSession(o => o.WithDatabase("neo4j"));
////        IAsyncSession session = driver.AsyncSession(o => o.WithDatabase("neo4j"));

////        try {

////            IResultCursor cursor = await session.RunAsync("CREATE (n:Room{name:'TESTIESsSsSs', area:302})");
////            await cursor.ConsumeAsync();

////            cursor = await session.RunAsync("MATCH (a:Room) RETURN a.name as name");
////            List<string> people = await cursor.ToListAsync(record => record["name"].As<string>());

////            people.ForEach(p => Debug.Log(p));
////        }

////        catch (Exception e) {
////            Debug.Log(e.Message);
////        }

////        finally {
////            await session.CloseAsync();
////        }

////        await driver.CloseAsync();

////    }


////}
