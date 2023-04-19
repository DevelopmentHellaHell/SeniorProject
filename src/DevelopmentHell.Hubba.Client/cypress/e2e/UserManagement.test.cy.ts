/// <reference types="cypress" />
/* cy.get(), cy.contains()
** https://filiphric.com/cypress-basics-selecting-elements
** https://on.cypress.io/type
*/

import { Ajax } from "../../src/Ajax";
import { Database } from "./TestModels/Database";



let testsRoute: string = '/tests/deleteDatabaseRecords';
let adminEmail: string = 'admin@hubba.com';
let userEmail: string = 'user@hubba.com'
let adminPassword: string = 'password1234';
let testadminRoute: string = '/tests/createAdmin';
let testadminBody: { email: string, password: string } = { email: adminEmail, password: adminPassword }

let createRoute: string = '/usermanagement/create';
let createBody: { email: string, password: string, username: string, firstname: string, lastname: string, role: string }
  = { email: 'johndoe@hubba.com', password: 'password1234', username: 'johndoe', firstname: 'John', lastname: 'Doe', role: 'VerifiedUser' };
let updateRoute: string = '/usermanagement/update';
let updateBody: { email: string, password: string, username: string, firstname: string, lastname: string, role: string }
  = { email: 'johndoe@hubba.com', password: 'password1234', username: 'johndoe', firstname: 'John', lastname: 'Doe', role: 'AdminUser' };
let disableRoute: string = '/usermanagement/disable';
let enableRoute: string = '/usermanagement/enable';
let deleteRoute: string = '/usermanagement/delete';
let emailBody: { email: string } = { email: 'johndoe@hubba.com' };
let admindashboard: string = 'http://localhost:3000/admin-dashboard';

describe('Unauthorized Access', () => {
    after(async () => {
        let result = await Ajax.post(testsRoute, { database: Database.Databases.USERS });
        expect(result.status == 200 || result.status == 204).to.be.true;
    });

    it('Create Account', async () => {
        let result = await Ajax.post(createRoute, createBody);
        expect(result.status == 200 || result.status == 204).to.be.false;
    });
    it('Update Account', async () => {
        let result = await Ajax.post(updateRoute, updateBody);
        expect(result.status == 200 || result.status == 204).to.be.false;
    });
    it('Disable Account', async () => {
        let result = await Ajax.post(disableRoute, emailBody);
        expect(result.status == 200 || result.status == 204).to.be.false;
    });
    it('Enable Account', async () => {
        let result = await Ajax.post(enableRoute, emailBody);
        expect(result.status == 200 || result.status == 204).to.be.false;
    });
    it('Delete Account', async () => {
        let result = await Ajax.post(deleteRoute, emailBody);
        expect(result.status == 200 || result.status == 204).to.be.false;
    });
});

describe('Create/Update Account', () => {
    before(() =>{
      //clear all sessions include the backend and cache
      Cypress.session.clearAllSavedSessions();
      Ajax.post(testadminRoute, testadminBody);
    });

    beforeEach(() => {
      cy.LoginViaApi(adminEmail, adminPassword);
      cy.visit(admindashboard);
      cy.get('.links').contains('Create/Update account').click();
    });

    after(async () => {
        await Ajax.post(testsRoute, { database: Database.Databases.USERS });
        Cypress.session.clearAllSavedSessions();
    });

    it('Create Success', () => {
        cy.get('#email').type(createBody.email);
        cy.get('#password').type(createBody.password);
        cy.get('#first-name').type(createBody.firstname);
        cy.get('#last-name').type(createBody.lastname);
        cy.get('.buttons').contains('Create').click();
        cy.get('#App').contains('Successfully created account');
    });

    it('Create Fail - Email already exists', () => {
        cy.get('#email').type(createBody.email);
        cy.get('#password').type(createBody.password);
        cy.get('#first-name').type(createBody.firstname);
        cy.get('#last-name').type(createBody.lastname);
        cy.get('.buttons').contains('Create').click();
        cy.get('#App').contains('Problem creating account. Please try again later.');
    });

    it('Create Fail - Invalid email', () => {
        cy.get('#email').type('invalidemail');
        cy.get('#password').type(createBody.password);
        cy.get('#first-name').type(createBody.firstname);
        cy.get('#last-name').type(createBody.lastname);
        cy.get('.buttons').contains('Create').click();
        cy.get('#App').contains('Problem creating account. Please try again later.');
    });

    it('Create Fail - Invalid password', () => {
      cy.get('#email').type('invalidemail');
      cy.get('#password').type(createBody.password);
      cy.get('#first-name').type(createBody.firstname);
      cy.get('#last-name').type(createBody.lastname);
      cy.get('.buttons').contains('Create').click();
      cy.get('#App').contains('Problem creating account. Please try again later.');
    });

    it('Update Success', () => {
      cy.get('#email').type(createBody.email);
      cy.get('#password').type(createBody.password+'2');
      cy.get('#first-name').type(createBody.firstname);
      cy.get('#last-name').type(createBody.lastname);
      cy.get('.buttons').contains('Update').click();
      cy.get('#App').contains('Successfully updated account');
    });

    it('Update Fail - Email does not exist', () => {
      cy.get('#email').type(createBody.email+'2');
      cy.get('#password').type(createBody.password);
      cy.get('#first-name').type(createBody.firstname);
      cy.get('#last-name').type(createBody.lastname);
      cy.get('.buttons').contains('Update').click();
      cy.get('#App').contains('Failed to Update account');
    });

    it('Update Fail - Invalid password', () => {
      cy.get('#email').type(createBody.email);
      cy.get('#password').type('1234');
      cy.get('#first-name').type(createBody.firstname);
      cy.get('#last-name').type(createBody.lastname);
      cy.get('.buttons').contains('Update').click();
      cy.get('#App').contains('Failed to Update account');
    });
});

describe('Disable/Enable Account', () => {
    before(() =>{
      //clear all sessions include the backend and cache
      Cypress.session.clearAllSavedSessions();
      Ajax.post(testadminRoute, testadminBody);
      Ajax.post(testadminRoute, createBody);
    });

    beforeEach(() => {
      cy.LoginViaApi(adminEmail, adminPassword);
      cy.visit(admindashboard);
      cy.get('.links').contains('Enable/Disable account').click();
    });

    after(async () => {
        await Ajax.post(testsRoute, { database: Database.Databases.USERS });
        Cypress.session.clearAllSavedSessions();
    });

    it('Disable Success', () => {
      cy.get('#email').type(createBody.email);
      cy.get('.buttons').contains('Disable').click();
      cy.get('#App').contains('Successfully Disabled Account');
    });

    it('Enable Success', () => {
      cy.get('#email').type(createBody.email);
      cy.get('.buttons').contains('Enable').click();
      cy.get('#App').contains('Successfully Enabled Account');
    });

    it('Disable Fail - Email does not exist', () => {
      cy.get('#email').type("notanemail@email.com");
      cy.get('.buttons').contains('Disable').click();
      cy.get('#App').contains('Failed to Disable account');
    });

    it('Enable Fail - Email does not exist', () => {
      cy.get('#email').type("notanemail@email.com");
      cy.get('.buttons').contains('Enable').click();
      cy.get('#App').contains('Failed to Enable account');
    });
});

describe('Delete Account', () => {
  before(() =>{
    //clear all sessions include the backend and cache
    Cypress.session.clearAllSavedSessions();
    Ajax.post(testadminRoute, testadminBody);
    Ajax.post(testadminRoute, createBody);
  });

  beforeEach(() => {
    cy.LoginViaApi(adminEmail, adminPassword);
    cy.visit(admindashboard);
    cy.get('.links').contains('Delete account').click();
  });

  after(async () => {
      await Ajax.post(testsRoute, { database: Database.Databases.USERS });
      Cypress.session.clearAllSavedSessions();
  });

  it('Success', () => {
    cy.get('#email').type(createBody.email);
    cy.get('.buttons').contains('Delete').click();
    cy.get('.buttons').contains('Confirm').click();
    cy.get('#App').contains('Successfully Deleted Account');
  });

  it('Fail - Email does not exist', () => {
    cy.get('#email').type(createBody.email);
    cy.get('.buttons').contains('Delete').click();
    cy.get('.buttons').contains('Confirm').click();
    cy.get('#App').contains('Failed to delete account');
  });
});