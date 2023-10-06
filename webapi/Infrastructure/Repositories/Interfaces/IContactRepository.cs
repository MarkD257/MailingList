using webapi.Domain.Entities;

namespace webapi.Domain.Interfaces
{
    public interface IContactRepository : IRepository<Contact>
    {
        (bool, string, List<Contact>) GetAllContacts();

        (bool, string, Contact) GetContact(Guid id);

        (bool, string) CreateContact(Contact contact);

        (bool, string) UpdateContact(Contact contact);

		(bool, string, List<Contact>) SearchContacts(string name);

		(bool, string) DeleteContact(Guid id);
	}
}
