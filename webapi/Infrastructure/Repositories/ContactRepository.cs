using System;
using System.Collections.Generic;

using webapi.Domain.Entities;
using webapi.Domain.Interfaces;
using webapi.Infrastructure.Data;

namespace webapi.Domain.Repositories
{
    public class ContactRepository : Repository<Contact>, IContactRepository
    {
        public ContactRepository(ApplicationDbContext context) : base(context)
        { }

        public (bool, string, List<Contact>) GetAllContacts()
        {
            try
            {
                List<Contact> contacts = _appContext.Contacts
                                    .OrderBy(c => c.LastName)
                                    .ToList();

                return (true, "", contacts);

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
				return (false, ex.Message, null);
			}
        }

        public (bool, string, Contact) GetContact(Guid id)
        {
            try
            {
                Contact contact = _appContext.Contacts
                    .Where(c => c.Id == id).FirstOrDefault();

                if (contact == null)
                    return (false, "", contact);
                else
                    return (true, "", contact); 
            }
            catch (Exception)
            {
                throw;
            }
        }

        public (bool, string) CreateContact(Contact contact)
        {
            try
            {
                _appContext.Contacts.Add(contact);

                _appContext.SaveChanges();

                return (true, "Contact added");
            }
            catch (Exception)
            {
                throw;
            }
        }

        public (bool, string) UpdateContact(Contact contact)
        {
            try
            {
                _appContext.Contacts.Update(contact);

                _appContext.SaveChanges();

                return (true, "Contact added");
            }
            catch (Exception)
            {
                throw;
            }
        }

		public (bool, string, List<Contact>) SearchContacts(string name)
		{
			try
			{
				List<Contact> contacts = _appContext.Contacts
                                    .Where(c => c.LastName!.StartsWith(name))
									.OrderBy(c => c.LastName)
									.ToList();

                if (contacts.Count > 0)
				    return (true, "", contacts);
                else
					return (false, "no contacts found", contacts);

			}
			catch (Exception)
			{
				throw;
			}
		}

		public (bool, string) DeleteContact(Guid id)
		{
			try
			{
				
				Contact contact = _appContext.Contacts
					.Where(c => c.Id == id).FirstOrDefault();

                if (contact == null)
                    return (false, "not found");
                else
                {
					_appContext.Contacts.Remove(contact);
					_appContext.SaveChanges();
					return (true, "contact deleted");
                }
			}
			catch (Exception)
			{
				throw;
			}
		}

		private ApplicationDbContext _appContext => (ApplicationDbContext)_context;
    }
}
