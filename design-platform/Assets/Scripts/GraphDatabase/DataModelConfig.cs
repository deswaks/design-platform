using Neo4jClient.Extension.Cypher;

namespace Database {
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

        // Sets up the data model, defining how the C# classes should be handled as Nodes in the graph

        public static void ConfigureDataModel() {

            // Room class configuration
            FluentConfig.Config()
               .With<RoomNode>("Room") //With<Class>("Label")
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

            // Adjacency relationship configuration
            FluentConfig.Config()
               .With<AdjacentRoomRelationship>()
               .Set();
        }
    }
}
