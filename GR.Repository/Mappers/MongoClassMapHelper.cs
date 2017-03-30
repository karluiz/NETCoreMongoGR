using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Conventions;

namespace GR.Repository.Mappers
{
    public static class MongoClassMapHelper
    {

        private static readonly object Lock = new object();


        /// <summary>
        /// Registers the convention packs.
        /// </summary>
        public static void RegisterConventionPacks()
        {
            lock (Lock)
            {
                var conventionPack = new ConventionPack();
                conventionPack.Add(new IgnoreIfNullConvention(true));
                ConventionRegistry.Register("ConventionPack", conventionPack, t => true);
            }
        }

        /// <summary>
        /// Setups the mappings.
        /// </summary>
        public static void SetupClassMap()
        {
            lock (Lock)
            {

                if (!BsonClassMap.IsClassMapRegistered(typeof(Entity)))
                {
                    BsonClassMap.RegisterClassMap<Entity>(
                        (classMap) =>
                        {
                            classMap.AutoMap();
                            classMap.MapIdProperty(p => p.Id);
                            classMap.MapExtraElementsProperty(p => p.Metadata);
                        });
                }
            }
        }
    }
}
