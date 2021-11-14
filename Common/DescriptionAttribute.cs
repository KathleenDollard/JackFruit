using System;
    
namespace Common
{
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Parameter)]
    public class DescriptionAttribute : Attribute
    {
        public DescriptionAttribute(string description)
        {
            Description = description;
        }

        public string Description { get; }
    }
}