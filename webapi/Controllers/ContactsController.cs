using System.Net;
using Microsoft.AspNetCore.Mvc;

using webapi.Services.Interfaces;
using webapi.Domain.Entities;
using webapi.Models;
using Microsoft.AspNetCore.Cors;

namespace webapi.Controllers;

// [09-11-23 MWD] Adddc api/ to route ----   Angular has api/  and Angular port is same as Swagger port
[Route("api/[controller]")]
public class ContactsController: ControllerBase
{
    private readonly IContactsService _contactService;

    public ContactsController(IContactsService contactService)
    {
        _contactService = contactService;
    }

	[HttpGet()]
	[Route("ContactsList")]
	public IActionResult GetAllContacts()
	{
		(bool success, string message, List<Contact> contacts) result = _contactService.GetAllContacts();

		if (!result.success)
			return NotFound();
		else
			return Ok(result.contacts); ;
	}


    [HttpGet("{id:guid}")]
    public IActionResult GetContact(Guid id)
    {
        (bool success, string message, Contact contact) result = _contactService.GetContact(id);

        if (!result.success)
            return new StatusCodeResult((int)HttpStatusCode.NotFound);
        else
            return Ok(result.contact);
    }

    [HttpPost]
	[Route("Create")]
	public ActionResult<string> CreateContact([FromBody] ContactModel contactModel)
    {
        try
        {
            (bool success, string message, Guid id) result = _contactService.CreateContact(contactModel);

            if (result.success)
                return result.id!.ToString();
            else
                return new BadRequestObjectResult(result.message);
        }
        catch (Exception)
        {
            //return BadRequestResult(ex)
            throw;
        }
    }

    [HttpPost]
	[Route("Update")]
	public ActionResult<string> UpdateContact(ContactModel contactModel, Guid id)
    {
        try
        {
            ContactUpdModel contactUpdModel = new ContactUpdModel(
               contactModel.FirstName,
               contactModel.LastName,
               // contactModel.BirthDate,
               contactModel.EmailAddress
            );

            (bool success, string message) result = _contactService.UpdateContact(contactUpdModel, id);

            if (result.success)
                return "Contact Updated";
            else
                return new BadRequestObjectResult(result.message);
        }
        catch (Exception)
        {
            throw;
        }
    }

	[HttpGet]
	[Route("Search")]
	public IActionResult SearchContacts(string name)
	{
		try
		{
			(bool success, string message, List<Contact> contacts) result = _contactService.SearchContacts(name);

			if (!result.success)
				return NotFound();
			else
				return Ok(result.contacts); ;
		}
		catch (Exception)
		{
			//return BadRequestResult(ex)
			throw;
		}
	}
	
    [HttpPost("{id:guid}")]
	public IActionResult Delete(Guid id)
	{
		(bool success, string message) result = _contactService.DeleteContact(id);

		if (!result.success)
			return new StatusCodeResult((int)HttpStatusCode.NotFound);
		else
			return Ok();
	}


	[HttpPut]
	[Route("SendEmail/{id:guid}")]
	public ActionResult<string> SendContactEmail(Guid id)
    {
        try
        {
            (bool success, string message) result = _contactService.SendContactEmail(id);

            if (result.success)
                return new ActionResult<string>("Contact Updated");
            else
                return new BadRequestObjectResult(result.message);
        }
        catch (Exception)
        {
            throw;
        }
    }
}