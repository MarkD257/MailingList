import { Component, Input, Output, EventEmitter } from '@angular/core';
import { Contact } from '../models/contact';

@Component({
  selector: 'app-contact',
  templateUrl: './contact.component.html',
  styleUrls:  ['./contact.component.css']
})
export class ContactComponent {
  @Input()
  contact!: Contact;
  @Output() delete = new EventEmitter();

  onDeleteClick($event: any): void {
    $event.stopPropagation();
    this.delete.emit();
  }
}
