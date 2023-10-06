import { Injectable } from '@angular/core';
import { HttpClient, HttpErrorResponse, HttpHeaders } from '@angular/common/http';

import { Observable, of, throwError } from 'rxjs';
import { catchError, map, tap } from 'rxjs/operators';

import { Contact } from '../models/contact';
import { MessageService } from '../message.service';
import { environment } from 'src/environments/environment';

const httpOptions = {
  headers: new HttpHeaders({ 'Content-Type': 'application/json' })
};

@Injectable()
export class ContactService {

  // private contactssUrl = 'api/contacts';  // URL to web api
  baseUrl = environment.apiUrl;
  baseContactsUrl = this.baseUrl + 'contacts';

  constructor(
    private http: HttpClient,
    private messageService: MessageService) { }

  /** GET contacts from the server */
  getContacts (): Observable<Contact[]> {
    return this.http.get<Contact[]>(this.baseContactsUrl + '/ContactsList')
      .pipe(
        tap(contacts => {
          this.log(`contacts ${contacts}`)
          this.log(`fetched contacts`);
        }),
        catchError(this.handleErrorType('getContacts', []))
      );
  }

  /** GET contact by id. Return `undefined` when id not found */
  getContactNo404<Data>(id: number): Observable<Contact> {
    const url = `${this.baseContactsUrl}/?id=${id}`;
    return this.http.get<Contact[]>(url)
      .pipe(
        map(contacts => contacts[0]), // returns a {0|1} element array
        tap(h => {
          const outcome = h ? `fetched` : `did not find`;
          this.log(`${outcome} contact id=${id}`);
        }),
        catchError(this.handleErrorType<Contact>(`getContact id=${id}`))
      );
  }

  /** GET contact by id. Will 404 if id not found */
  getContact(id: number): Observable<Contact> {
    const url = `${this.baseContactsUrl}/${id}`;
    return this.http.get<Contact>(url).pipe(
      tap(_ => this.log(`fetched contact id=${id}`)),
      catchError(this.handleErrorType<Contact>(`getContact id=${id}`))
    );
  }

  /* GET contacts whose name begins with search term */
  searchContacts(term: string): Observable<Contact[]> {
    if (!term.trim()) {
      // if not search term, return empty contact array.
      return of([]);
    }
    return this.http.get<Contact[]>(`api/contacts/Search/name=${term}`).pipe(
      tap(_ => this.log(`found contacts matching "${term}"`)),
      catchError(this.handleErrorType<Contact[]>('searchContacts', []))
    );
  }

  //////// Save methods //////////

  /** POST: add a new contact to the server */
  addContact (contact: Contact): Observable<Contact> {
    return this.http.post<Contact>(this.baseContactsUrl + '/Create' , contact, httpOptions).pipe(
      tap((contact: Contact) => this.log(`added contact w/ id=${contact.Id}`)),
      catchError(this.handleErrorType<Contact>('addContact'))
    );
  }

  /** DELETE: delete the contact from the server */
  deleteContact (contact: Contact | number): Observable<Contact> {
    const id = typeof contact === 'number' ? contact : contact.Id;
    const url = `${this.baseContactsUrl}/Delete/${id}`;

    return this.http.delete<Contact>(url, httpOptions).pipe(
      tap(_ => this.log(`deleted contact id=${id}`)),
      catchError(this.handleErrorType<Contact>('deleteContact'))
    );
  }

  /** PUT: update the contact on the server */
  updateContact (contact: Contact): Observable<any> {
    return this.http.put(this.baseContactsUrl + '/Update', contact, httpOptions).pipe(
      tap(_ => this.log(`updated contact id=${contact.Id}`)),
      catchError(this.handleErrorType<any>('updateContact'))
    );
  }

  /**
   * Handle Http operation that failed.
   * Let the app continue.
   * @param operation - name of the operation that failed
   * @param result - optional value to return as the observable result
   */
  private handleErrorType<T> (operation = 'operation', result?: T) {
    return (error: any): Observable<T> => {

      // TODO: send the error to remote logging infrastructure
      console.error(error); // log to console instead

      // TODO: better job of transforming error for user consumption
      this.log(`${operation} failed: ${error.message}`);

      // Let the app keep running by returning an empty result.
      return of(result as T);
    };
  }

  private handleError(error: HttpErrorResponse) {
    console.error('server error:', error); 
    if (error.error instanceof Error) {
      let errMessage = error.error.message;
      throwError(() => new Error(errMessage));
    }
    //return throwError(error || 'ASP.NET Core server error');
    throwError(() => new Error('ASP.NET Core server error'))
}

  /** Log a ContactService message with the MessageService */
  private log(message: string) {
    this.messageService.add('ContactService: ' + message);
  }
}
