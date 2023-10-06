import { Component, OnInit, ViewEncapsulation } from '@angular/core';

import { Contact } from '../models/contact';
import { ContactService } from '../services/contact.service';
import { ToastrService } from 'ngx-toastr';

@Component({
  selector: 'app-contacts',
  templateUrl: './contacts.component.html',
  styleUrls: ['./contacts.component.css'],
  encapsulation: ViewEncapsulation.None
})
export class ContactsComponent implements OnInit {
  contacts!: Contact[];

  constructor(
    private contactService: ContactService,
    private toastr: ToastrService) { }

  ngOnInit() {
    this.getContacts();
  }

  getContacts(): void {
    this.contactService.getContacts().subscribe(contacts => {
        this.contacts = contacts;
        this.toastr.success('Hello world!', 'Toastr says found contacts!');
    });

  }

  add(firstName: string, lastName: string, emailAddress: string): void {
    firstName = firstName.trim();
    lastName = lastName.trim();
    emailAddress = emailAddress.trim();

    if (!firstName) { return; }
    if (!lastName) { return; }
    if (!emailAddress) { return; }

    const contact = new Contact();
    contact.FirstName = firstName;
    contact.LastName = lastName;
    contact.EmailAddress = emailAddress;

    this.contactService.addContact(contact).subscribe(contact => {
        this.contacts.push(contact);
        this.toastr.success('Hello world!', 'Toastr says contact created!');
      });
  }

  delete(contact: Contact): void {
    this.contacts = this.contacts.filter(c => c !== contact);
    this.contactService.deleteContact(contact).subscribe();
  }

}
