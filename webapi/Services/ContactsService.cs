using System.Linq;
using System.Net.Mail;
using System.Text;

using webapi.Domain.Entities;
using webapi.Domain.Interfaces;
using webapi.Models;
using webapi.Services.Interfaces;
using WFLibrary;

namespace webapi.Services;

public class ContactsService: IContactsService
{
    private readonly IContactRepository _contactRepository;

    public ContactsService(IContactRepository contactRepository)
    {
        _contactRepository = contactRepository;
    }

    public (bool, string, List<Contact>) GetAllContacts()
    {
        try
        {
            //   cannot convert from 'System.Guid?' to 'System.Guid'   nullable value type may be null ---- ! removes warning
            //   //https://learn.microsoft.com/en-us/dotnet/csharp/language-reference/compiler-messages/nullable-warnings?f1url=%3FappId%3Droslyn%26k%3Dk(CS8629)
            (bool success, string message, List<Contact> contacts) result = _contactRepository.GetAllContacts();

            if (!result.success)
				if (result.message.Contains("server"))
                    return (false, "Use the Azure Management Portal or run sp_set_firewall_rule on the master database ", result.contacts);
				else if (result.message.Contains("failed"))
					return (false, result.message, result.contacts);
				else
				    return (false, "No contacts found", result.contacts);
			else
                return (true, "Contacts found", result.contacts);
        }
        catch (Exception)
        {
            throw;
        }
    }

    public (bool, string, Contact) GetContact(Guid? id)
    {
         try
         {
            //   cannot convert from 'System.Guid?' to 'System.Guid'   nullable value type may be null ---- ! removes warning
            //   //https://learn.microsoft.com/en-us/dotnet/csharp/language-reference/compiler-messages/nullable-warnings?f1url=%3FappId%3Droslyn%26k%3Dk(CS8629)
            (bool success, string message, Contact contact) result = _contactRepository.GetContact(id!.Value);

             if (!result.success)
                return (false, "contact not found", result.contact);
             else
                return (true, "contact found", result.contact);
        }
         catch (Exception)
         {
             throw;
         }
    }

    public (bool, string, Guid) CreateContact(ContactModel model)
    {
        string errors = Validate(model);

        if (errors != string.Empty) 
            return (false, errors, Guid.NewGuid());
        
        try
        {
            Contact contact = new(Guid.NewGuid())
            {
                FirstName = model.FirstName,
                LastName = model.LastName,
                EmailAddress = model.EmailAddress
            };

            _contactRepository.CreateContact(contact);

            return (true, "Contact added", contact.Id);
        }
        catch (Exception)
        {
            throw;
        }
    }

	public (bool, string) UpdateContact(ContactUpdModel model, Guid id)
	{
		(bool success, string message, Contact contact) result = GetContact(id);

		if (!result.success)
			return (false, result.message);

		//string errors = Validate(model);

		//if (errors != string.Empty)
		//    return (false, errors);

		try
		{
			Contact contact = result.contact!;

			contact.FirstName = model.FirstName;
			contact.LastName = model.LastName;
			contact.EmailAddress = model.EmailAddress;

			_contactRepository.UpdateContact(contact);

			return (true, "Contact updated");
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
			//   cannot convert from 'System.Guid?' to 'System.Guid'   nullable value type may be null ---- ! removes warning
			//   //https://learn.microsoft.com/en-us/dotnet/csharp/language-reference/compiler-messages/nullable-warnings?f1url=%3FappId%3Droslyn%26k%3Dk(CS8629)
			(bool success, string message, List<Contact> contacts) result = _contactRepository.SearchContacts(name);

			if (!result.success)
				return (false, "no contacts found", result.contacts);
			else
				return (true, "contacts found", result.contacts);
		}
		catch (Exception)
		{
			throw;
		}
	}

	public (bool, string) DeleteContact(Guid? id)
	{
		try
		{
				(bool success, string message) result = _contactRepository.DeleteContact(id!.Value);

			if (!result.success)
				return (false, "contact not found");
			else
				return (true, "contact deleted");
		}
		catch (Exception)
		{
			throw;
		}
	}

	public (bool, string) SendContactEmail(Guid? id)
    {
        (bool success, string message, Contact contact) result = GetContact(id);

        if (!result.success)
            return (false, result.message);

        try
        {
			Contact contact = result.contact!;

            SendMail.Send("", contact.EmailAddress!, "Contact email", "Test Message");
           
            return (true, "Contact email sent");
        }
        catch (Exception)
        {
            throw;
        }
    }

    private static string Validate(ContactModel model)
    {
        var errors = new List<string>();

        if (model.FirstName == null || model.FirstName == "")
        {
            errors.Add("First name is empty");
        }
        else
            if (model.FirstName.Length > 50)
                errors.Add("First name exceeds 50 chars ");

        if (model.LastName == null || model.LastName == "")
        {
            errors.Add("Last name is empty");
        }
        else
            if (model.LastName.Length > 50)
                errors.Add("Last name exceeds 50 chars ");

        // if (model.BirthDate > DateTime.Now)
        //    errors.Add("Birth Date must be in the past ");

        if (model.EmailAddress != null && model.EmailAddress != "")
        {
            try
            {
                MailAddress m = new MailAddress(model.EmailAddress);
            }
            catch (FormatException)
            {
                errors.Add("Email Address is not a valid format ");
            }
        }

        if (errors.Any())
        {
            var errorBuilder = new StringBuilder();

            errorBuilder.AppendLine("Invalid contact info, reason: ");

            foreach (var error in errors)
            {
                errorBuilder.AppendLine(error);
            }

            // throw new Exception(errorBuilder.ToString());
            return errorBuilder.ToString();
        }
        return string.Empty;
    }
}