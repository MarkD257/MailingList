using System;
using static System.Net.Mime.MediaTypeNames;

namespace webapi.Models
{
    public class ContactModel
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        // public DateTime BirthDate { get; set; }
        public string EmailAddress { get; set; }
        public Guid? Id { get; set; }
    }

	public class ContactUpdModel
	{
		public ContactUpdModel(string firstName, string lastName, string emailAddress)
		{
			FirstName = firstName;
			LastName =lastName;
			EmailAddress = emailAddress;
			//FuelType = Guard.Against.EnumOutOfRange(fuelType);
		}
		public string FirstName { get; }
		public string LastName { get; }
		public string EmailAddress { get; }
		public Guid? Id { get; }
	}
}