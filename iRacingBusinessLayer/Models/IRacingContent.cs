using System.Runtime.Serialization;

namespace iRacingBusinessLayer.Models
{
    public enum IRacingContentType
    {
        Car,
        Track
    }

    [Serializable]
    public class IRacingContent : ISerializable
    {
        public string Name { get; set; }
        public bool IsFree { get; set; }
        public IRacingContentType Type { get; set; }

        public IRacingContent() { }
        public IRacingContent(string name, bool isFree = false)
        {
            Name = name;
            IsFree = isFree;
        }

        public IRacingContent(SerializationInfo info, StreamingContext context)
        {
            Name = info.GetString("Name");
            IsFree = info.GetBoolean("IsFree");
        }

        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("Name", Name);
            info.AddValue("IsFree", IsFree);
        }
    }
}
