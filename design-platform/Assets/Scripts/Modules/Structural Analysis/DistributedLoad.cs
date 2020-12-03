namespace StructuralAnalysis {

    /// <summary>
    /// Represents a distributed load on a wa
    /// </summary>
    public struct DistributedLoad {

        /// <summary>Start parameter defines the location of the load start point on the wall.</summary>
        public float StartParameter { get; set; }

        /// <summary>End parameter defines the location of the load end point on the wall.</summary>
        public float EndParameter { get; set; }

        /// <summary>The magnitude of the load.</summary>
        public float Magnitude { get; set; }

        /// <summary>
        /// Default constructor. Creates a distributed load with the given magnitude and parameters.
        /// </summary>
        /// <param name="magnitude">Magnitude of the load.</param>
        /// <param name="startParameter"></param>
        /// <param name="endParameter"></param>
        public DistributedLoad(float magnitude = 0.0f, float startParameter = 0.0f, float endParameter = 1.0f) {
            StartParameter = startParameter;
            EndParameter = endParameter;
            Magnitude = magnitude;
        }
    }
}
