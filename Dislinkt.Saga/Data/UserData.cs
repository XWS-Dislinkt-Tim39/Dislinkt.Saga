using System;

namespace Dislinkt.Saga.Data
{
    public class UserData
    {
        /// <summary>
        /// Date of birth
        /// </summary>
        public string FirstName { get; set; }
        /// <summary>
        /// Last name
        /// </summary>
        public string LastName { get; set; }
        /// <summary>
        /// Username
        /// </summary>
        public string Username { get; set; }
        public string Biography { get; set; }
        /// <summary>
        /// Email address
        /// </summary>
        public string EmailAddress { get; set; }
        /// <summary>
        /// Password
        /// </summary>
        public string Password { get; set; }
        /// <summary>
        /// Address
        /// </summary>
        public string Address { get; set; }
        /// <summary>
        /// Date of birth
        /// </summary>
        public DateTime DateOfBirth { get; set; }
        /// <summary>
        /// City
        /// </summary>
        public string City { get; set; }
        /// <summary>
        /// Country
        /// </summary>
        public string Country { get; set; }
        /// <summary>
        /// Phone number
        /// </summary>
        public string PhoneNumber { get; set; }
        /// <summary>
        /// Gender
        /// </summary>
        public GenderData Gender { get; set; }
        /// <summary>
        /// Seniority
        /// </summary>
        public Seniority Seniority { get; set; }
    }
}
