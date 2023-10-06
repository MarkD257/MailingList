import { NgModule }       from '@angular/core';
import { BrowserModule }  from '@angular/platform-browser';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';
import { FormsModule }    from '@angular/forms';
import { HttpClientModule }    from '@angular/common/http';

import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatSelectModule } from '@angular/material/select';

import { ToastrModule } from 'ngx-toastr';

import { AppRoutingModule }     from './app-routing.module';

import { AppComponent }         from './app.component';
import { DashboardComponent }   from './dashboard/dashboard.component';
import { ContactDetailComponent }  from './contact-detail/contact-detail.component';
import { ContactsComponent }      from './contacts/contacts.component';
import { ContactSearchComponent }  from './contact-search/contact-search.component';
import { ContactService }          from './services/contact.service';
import { MessageService }       from './message.service';
import { MessagesComponent }    from './messages/messages.component';

import { ContactComponent } from './contact/contact.component';


@NgModule({
  imports: [
    BrowserModule,
    BrowserAnimationsModule,
    FormsModule,
    AppRoutingModule,
    HttpClientModule,
    ToastrModule.forRoot(), // ToastrModule added
    MatFormFieldModule,
    MatInputModule,
    MatSelectModule
  ],
  declarations: [
    AppComponent,
    DashboardComponent,
    ContactsComponent,
    ContactDetailComponent,
    MessagesComponent,
    ContactSearchComponent,
    ContactComponent
  ],
  providers: [ ContactService, MessageService ],
  bootstrap: [ AppComponent ]
})
export class AppModule { }
