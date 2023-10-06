using webapi.Domain.Primitives;


namespace webapi.Domain.Entities
{
    public class Contact : Entity
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string EmailAddress { get; set; }
  
        public Contact(Guid id) : base(id)
        {
        }
    }
}