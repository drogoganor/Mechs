using Mechs.Utility.Generation.CityMapGenerator.Data;
using System.Collections.Concurrent;
using System.Numerics;

namespace Mechs.Utility.Generation.CityMapGenerator
{
    public class RandomSamplerConfig
    {
        public Vector2 Size { get; set; }

        /// <summary>
        /// How many random sample points are taken when getting random points.
        /// </summary>
        public int BatchSampleCount { get; set; } = 1600;

        /// <summary>
        /// Minimum distance that a point must be away from other selected points.
        /// </summary>
        public float? MinDistanceFromOthers { get; set; }

        /// <summary>
        /// Minimum distance that a point's axes must be away from other selected points axes.
        /// </summary>
        public float? MinDistanceFromOthersAxis { get; set; }

        /// <summary>
        /// Minimum distance that a point must be away from the edge of the region.
        /// </summary>
        public float? MinDistanceFromEdge { get; set; }
    }

    /// <summary>
    /// Samples random points from a region that satisfy certain distance constraints
    /// </summary>
    public class RandomSampler
    {
        public RandomSamplerConfig Config { get; set; }
        public Random Random { get; set; }

        public RandomSampler(RandomSamplerConfig config)
        {
            Config = config;
            Random = new Random();
        }

        /// <summary>
        /// Get this many random directions
        /// </summary>
        /// <param name="numDirections"></param>
        /// <returns></returns>
        public DirectionEnum[] GetRandomDirections(int numDirections)
        {
            var results = new ConcurrentBag<DirectionEnum>();
            for (var i = 0; i < numDirections; i++)
            {
                results.Add((DirectionEnum)Random.Next(4));
            }

            return results.ToArray();
        }

        /// <summary>
        /// Get this many random points that satisfy the constraints.
        /// </summary>
        /// <param name="numPoints"></param>
        /// <returns></returns>
        public Vector2[] GetRandomPoints(int numPoints)
        {
            var randomPoints = GetBatchRandom();
            var results = new List<Vector2>();

            foreach (var point in randomPoints)
            {
                if (IsPointValid(point))
                {
                    results.Add(point);

                    if (results.Count >= numPoints)
                    {
                        return results.ToArray();
                    }
                }
            }

            Console.Error.WriteLine("Couldn't find enough random points that satisfied the criteria.");

            return results.ToArray();

            /////////////
            
            bool IsPointValid(Vector2 vec)
            {
                // Check it's not too close to the edge
                if (Config.MinDistanceFromEdge.HasValue)
                {
                    var minDist = Config.MinDistanceFromEdge.Value;
                    if (vec.X < minDist || vec.Y < minDist)
                    {
                        return false;
                    }
                    else if (vec.X > Config.Size.X - minDist || vec.Y > Config.Size.Y - minDist)
                    {
                        return false;
                    }
                }

                if (Config.MinDistanceFromOthers.HasValue || Config.MinDistanceFromOthersAxis.HasValue)
                {
                    foreach (var result in results)
                    {
                        var distVec = vec - result;

                        // Check this point has a minimum distance from others
                        if (Config.MinDistanceFromOthers.HasValue)
                        {
                            if (distVec.Length() < Config.MinDistanceFromOthers.Value)
                            {
                                return false;
                            }
                        }

                        // Check this points' axes are not close to the axes of another point
                        if (Config.MinDistanceFromOthersAxis.HasValue)
                        {
                            if (Math.Abs(distVec.X) < Config.MinDistanceFromOthersAxis.Value ||
                                Math.Abs(distVec.Y) < Config.MinDistanceFromOthersAxis.Value)
                            {
                                return false;
                            }
                        }
                    }
                }

                return true;
            }
        }

        /// <summary>
        /// Get a batch of random points in range, regardless of constraints
        /// </summary>
        /// <returns></returns>
        private List<Vector2> GetBatchRandom()
        {
            var results = new List<Vector2>(Config.BatchSampleCount);
            for (var i = 0; i < Config.BatchSampleCount; i++)
            {
                var minDist = Config.MinDistanceFromEdge.HasValue ? (int)Config.MinDistanceFromEdge.Value : 0;
                var vec = new Vector2(Random.Next(((int)Config.Size.X) - (minDist * 2)), Random.Next(((int)Config.Size.Y) - (minDist * 2)));
                results.Add(vec + new Vector2(minDist));
            }

            return results;
        }
    }
}
