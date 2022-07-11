namespace Dislinkt.Saga.Data
{
    public class ConnectionJobData
    {
        /// <summary>
        /// Unique identificator
        /// </summary>
        public string Id { get; set; }
        /// <summary>
        /// Username
        /// </summary>
        public string Username { get; set; }

        /// <summary>
        /// Visibility status (private, public)
        /// </summary>
        public Seniority Seniority { get; set; }
    }
}
