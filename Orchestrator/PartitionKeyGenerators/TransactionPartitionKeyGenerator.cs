namespace Orchestrator.Util
{
    public class TransactionPartitionKeyGenerator
    {
        /// <summary>
        /// Generates partiion key for the transaction, based on users id (act as username in this app, int due to simplicity)
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        public static long GenerateFor(int id) 
        {
            return id % 4;
        }
    }
}
