namespace Orchestrator.Util
{
    public class UserPartitionKeyGenerator
    {
        /// <summary>
        /// Generates partiion key for the user, based on his id (act as username in this app, int due to simplicity) - 2 partitions in total.
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        public static long GenerateFor(int id) 
        {
            return id % 2;
        }
    }
}
