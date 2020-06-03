namespace BSG.Blockchain.Models
{
    /// <summary> 
    /// Represents the user account
    /// </summary>
    public class Account
    {
        /// <summary>
        /// The unique identification that represents the user 
        /// </summary>
        /// <value></value>
        public long Id { get; set; }

        /// <summary>
        /// The user's blockchain wallet address
        /// </summary>
        /// <value></value>
        public string WalletAddress { get; set; }

        /// <summary>
        /// The user's encrypted private key
        /// </summary>
        /// <value></value>
        public string PrivateKey { get; set; }

        /// <summary>
        /// The user's blockhain generated public key
        /// </summary>
        /// <value></value>
        public string PublicKey { get; set; }

        /// <summary>
        /// Just a test secret key for DTO
        /// </summary>
        /// <value></value>
        public string Secret { get; set;}
    }
}
