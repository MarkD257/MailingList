import { Component, OnInit, ViewEncapsulation } from '@angular/core';

import {MatSelectModule} from '@angular/material/select';
import {MatInputModule} from '@angular/material/input';
import {MatFormFieldModule} from '@angular/material/form-field';

import { Observable, Subject } from 'rxjs';

import {
   debounceTime, distinctUntilChanged, switchMap
 } from 'rxjs/operators';

import { Contact } from '../models/contact';
import { ContactService } from '../services/contact.service';

@Component({
  selector: 'app-contact-search',
  templateUrl: './contact-search.component.html',
  styleUrls: [ './contact-search.component.css' ],
  encapsulation: ViewEncapsulation.None
  //standalone: true,
  //imports: [MatFormFieldModule, MatInputModule, MatSelectModule]
})
export class ContactSearchComponent implements OnInit {
  contacts$!: Observable<Contact[]>;
  private searchTerms = new Subject<string>();

  constructor(private contactService: ContactService) {}

  // Push a search term into the observable stream.
  search(term: string): void {
    this.searchTerms.next(term);
  }

  ngOnInit(): void {
    this.contacts$! = this.searchTerms.pipe(
      // wait 300ms after each keystroke before considering the term
      debounceTime(300),

      // ignore new term if same as previous term
      distinctUntilChanged(),

      // switch to new search observable each time the term changes
      switchMap((term: string) => this.contactService.searchContacts(term)),
    );
  }
}
