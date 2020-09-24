//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;
//using Neo4j.Driver;
//using System;

//public class Graphdata : MonoBehaviour
//{

//    async void Neo4jGraphStuff() {
//        //////////////// GRAPH STUFF /////////////////////////////////////////////////////////////////////

//        IDriver driver = GraphDatabase.Driver("neo4j://localhost:7687", AuthTokens.Basic("neo4j", "test"));
//        IAsyncSession session = driver.AsyncSession(o => o.WithDatabase("neo4j"));


//        try {
//            //IResultCursor cursor = await session.RunAsync("MATCH (a:Room) RETURN a.name as name");

//            IResultCursor cursor = await session.RunAsync("CREATE (n:Room{name:'Study room',area:302})");
//            await cursor.ConsumeAsync();

//            cursor = await session.RunAsync("MATCH (a:Room) RETURN a.name as name");
//            List<string> people = await cursor.ToListAsync(record => record["name"].As<string>());

//            people.ForEach(p => Debug.Log(p));
//        }
//        catch (Exception e) {
//            Debug.Log(e.Message);
//        }
//        finally {
//            await session.CloseAsync();
//        }
//        await driver.CloseAsync();
//    }

//    async void CreateNewRoomNode() {



//    }
//}
