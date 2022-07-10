namespace Dislinkt.Saga.Data
{
    public class ConnectionData
    {

        /// <summary>
        /// Unique identificator
        /// </summary>
        public string Id { get; set; }
        /// <summary>
        /// Username
        /// </summary>
        public string UserName { get; set; }

        /// <summary>
        /// Visibility status (private, public)
        /// </summary>
        public int Status { get; set; }
    }
}
