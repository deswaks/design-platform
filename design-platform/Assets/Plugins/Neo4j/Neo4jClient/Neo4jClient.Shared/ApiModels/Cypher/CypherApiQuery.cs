using Neo4jClient.Cypher;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace Neo4jClient.ApiModels.Cypher {
    class CypherApiQuery {
        public CypherApiQuery(CypherQuery query) {
            Query = query.QueryText;
            Parameters = query.QueryParameters ?? new Dictionary<string, object>();
        }

        [JsonProperty("query")]
        public string Query { get; }

        [JsonProperty("params")]
        public IDictionary<string, object> Parameters { get; }
    }
}
