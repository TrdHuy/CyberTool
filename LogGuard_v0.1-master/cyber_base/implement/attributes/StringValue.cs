using System;

namespace cyber_base.implement.attributes
{
    ///<summary>
    ///This attribute is used to represent a string value
    ///for a value enum.
    ///</summary>
    public class StringValueAttribute : Attribute
    {
        ///<summary>
        ///Holds the string value for an enum
        ///</summary>
        public string StringValue { get; protected set; }

        ///<summary>
        ///Constructor for StringValue Attribute
        ///</summary>
        public StringValueAttribute(string value)
        {
            this.StringValue = value;
        }
    }
}