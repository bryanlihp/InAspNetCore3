using System;
using System.Collections.Generic;
using System.Text;

namespace Configuration.UserTypes
{
    public enum Gender
    {
        Male,
        Female
    }

    public class ContactInfo : IEquatable<ContactInfo>
    {
        public string Email { get; set; }
        public string Phone { get; set; }

        public bool Equals(ContactInfo other)
        {
            return other != null && (Email == other.Email && Phone == other.Phone);
        }
    }

    public class Profile : IEquatable<Profile>
    {
        public Gender Gender { get; set; }
        public int Age { get; set; }
        public ContactInfo ContactInfo { get; set; }

        public bool Equals(Profile other)
        {
            return other != null &&
                   (Gender == other.Gender && Age == other.Age && ContactInfo.Equals(other.ContactInfo));
        }
    }
}
