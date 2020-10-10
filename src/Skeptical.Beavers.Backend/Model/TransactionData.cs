using System.ComponentModel;

namespace Skeptical.Beavers.Backend.Model
{
    public sealed class TransactionData
    {
        /// <summary>
        /// Target account number
        /// </summary>
        [DefaultValue("0000 1111 2222 3333")]
        public string AccountNumber { get; set; }

        /// <summary>
        /// Holder of the target account
        /// </summary>
        [DefaultValue("John Smith")]
        public string AccountHolder { get; set; }

        /// <summary>
        /// Amount of money to send.
        /// </summary>
        [DefaultValue(10)]
        public int MoneySent { get; set; }
    }
}