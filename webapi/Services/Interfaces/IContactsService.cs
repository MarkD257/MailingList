

using webapi.Domain.Entities;
using webapi.Models;

namespace webapi.Services.Interfaces;

public interface IContactsService
{
    (bool, string, List<Contact>) GetAllContacts();

    (bool, string, Contact) GetContact(Guid? id);

    (bool, string, Guid) CreateContact(ContactModel model);

    (bool, string) UpdateContact(ContactUpdModel model, Guid id);

	(bool, string, List<Contact>) SearchContacts(string name);

	(bool, string) DeleteContact(Guid? id);

	(bool, string) SendContactEmail(Guid? id);
}
