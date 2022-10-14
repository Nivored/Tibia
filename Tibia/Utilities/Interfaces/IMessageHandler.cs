namespace Tibia.Utilities.Interfaces
{
    public interface IMessageHandler
    {
        public void Write(string message = "");
        public void WriteL(string message = "");
        public string Read();

        /// <summary>
        /// Write a message on a line and then read the line
        /// </summary>
        /// <param name="message"></param>
        public void WriteRead(string message);

        /// <summary>
        /// Should be used to clear the screen
        /// </summary>
        public void Clear();
    }
}
