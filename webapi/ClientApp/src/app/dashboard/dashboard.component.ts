import { Component, OnInit, ViewEncapsulation } from '@angular/core';
import { Contact } from '../models/contact';
import { ContactService } from '../services/contact.service';

@Component({
  selector: 'app-dashboard',
  templateUrl: './dashboard.component.html',
  styleUrls: [ './dashboard.component.css' ],
  encapsulation: ViewEncapsulation.None
})
export class DashboardComponent implements OnInit {
  contacts: Contact[] = [];

  constructor(private contactService: ContactService) { }

  ngOnInit() {
    this.getContacts();
  }

  getContacts(): void {
    this.contactService.getContacts()
      .subscribe(contacts => {
         this.contacts = contacts.slice(1, 5);
         console.log(`fetched ${contacts.length} contacts`)
      });  // contacts.slice(1, 5)
  }
}
