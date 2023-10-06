
export interface IContact {
    id?: string;
    firstName: string;
    lastName: string;
    emailAddress: string;
}

export interface IContactesponse {
    status: boolean;
    contact: IContact;
}