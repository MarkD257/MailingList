import { Component, OnInit, Input, ViewEncapsulation } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { Location } from '@angular/common';

import { Contact }         from '../models/contact';
import { ContactService }  from '../services/contact.service';

@Component({
  selector: 'app-contact-detail',
  templateUrl: './contact-detail.component.html',
  styleUrls: [ './contact-detail.component.css' ],
  encapsulation: ViewEncapsulation.None
})
export class ContactDetailComponent implements OnInit {
  @Input()
  contact!: Contact;

  constructor(
    private route: ActivatedRoute,
    private contactService: ContactService,
    private location: Location
  ) {}

  ngOnInit(): void {
    this.getContact();
  }

  getContact(): void {
    // https://jasonwatmore.com/post/2022/11/10/angular-fix-for-argument-of-type-string-null-is-not-assignable-to-parameter-of-type-string
    const id: number = JSON.parse(this.route.snapshot.paramMap.get('id')!);
    this.contactService.getContact(id)
      .subscribe(contact => this.contact = contact);
  }

  goBack(): void {
    this.location.back();
  }

 save(): void {
    this.contactService.updateContact(this.contact)
      .subscribe(() => this.goBack());
  }
}
