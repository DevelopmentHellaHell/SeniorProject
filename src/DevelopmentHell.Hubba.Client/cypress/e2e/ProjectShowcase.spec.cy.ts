/// <reference types="cypress" />
/* cy.get(), cy.contains()
** https://filiphric.com/cypress-basics-selecting-elements
** https://on.cypress.io/type
*/

import { Ajax } from "../../src/Ajax";
import { Database } from "./TestModels/Database";



let testsRoute: string = '/tests/deleteDatabaseRecords';
let adminEmail: string = 'admin@hubba.com';
let userEmail: string = 'user@hubba.com';
let adminPassword: string = '12345678';
let testadminRoute: string = '/tests/createAdmin';
let testuserRoute: string = '/tests/createUser';
let testuserBody: { email: string, password: string } = { email: userEmail, password: adminPassword }
let testadminBody: { email: string, password: string } = { email: adminEmail, password: adminPassword }

let accountRoute: string = 'http://localhost:3000/account';
let showcaseRoute: string = 'http://localhost:3000/showcases/p/view?s=';
let createShowcaseRoute = '/showcases/p/new';
let getShowcaseRoute = '/showcases/p/view';
let getUserShowcasesRoute = '/showcases/p/user';

let testFile1 = 'cypress/fixtures/test1.png';
let testFile2 = 'cypress/fixtures/test2.png';


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
let showcaseId: string;
let showcaseName: string;


describe ('Showcase Tests', () => {
  before(async () => {
      await Ajax.post(testuserRoute, testuserBody).then((result) => {
      });
      await Ajax.post(testadminRoute, testadminBody).then((result) => {
      });
  });

  beforeEach(() => {
    Cypress.session.clearAllSavedSessions();
  });

  

  it('Seq 1: Create+view Showcase', () => {
    cy.LoginViaApi(userEmail, adminPassword);
    cy.visit(accountRoute);
    cy.get('.links').contains('Project Showcases').click();
    cy.get('.btn').contains('Create New Showcase').click();
    cy.wait(1000);
    cy.get('.input-description').type('This is test showcase 1');
    showcaseName = 'Test Showcase ' + Date.now();
    cy.get('.input-title[type=text]').type(showcaseName);
    cy.get('input[type=file]').selectFile([testFile1]);
    cy.wait(3000);
    cy.get('button[type=submit]').click();
    cy.url().should('include', Cypress.env("baseUrl")+"/showcases/p/view");
    cy.url().then((url) => {
      showcaseId = url.slice(url.search('=')+1);
    });
  });

  it('Seq 2: Publish Showcase', () => {
    cy.LoginViaApi(userEmail, adminPassword);
    cy.visit(accountRoute);
    cy.get('.links').contains('Project Showcases').click();
    cy.get("tr").get(".published-no").first().click();
    cy.wait(3000);
  });

  it('Seq 2.1: View Showcase as unauthenticated user', () => {
    cy.visit(showcaseRoute+showcaseId);
    cy.get('#App').contains(showcaseName, { timeout: 10000 }).should('exist');
  });

  it('Seq 2.2: Comment on Showcase as authenticated user', () => {
    cy.visit(showcaseRoute+showcaseId);
    cy.get('.comment-input-box').type('This is a test comment');
    cy.get('button').contains('Submit Comment').first().click();
    cy.get('#App').contains('This is a test comment', { timeout: 10000 }).should('exist');
  });

  it('Seq 3: Edit Showcase', () => {
    cy.LoginViaApi(userEmail, adminPassword);
    cy.visit(accountRoute);
    cy.get('.links').contains('Project Showcases').click();
    cy.get("tr").contains('Edit').click();
    cy.get('.input-title[type=text]').type('Edited Showcase 1' + Date.now());
    cy.get('button[type=submit]').first().click();
    cy.get('#App').contains('Edited Showcase 1', { timeout: 10000 }).should('exist');
  });

  it('Seq 4: Delete Showcase', () => {
    cy.LoginViaApi(userEmail, adminPassword);
    cy.visit(accountRoute);
    cy.get('.links').contains('Project Showcases').click();
    cy.get("tr").contains('Delete').click();
    cy.get("tr").contains('Confirm').click();
    //cy.get('#App').contains('Showcase Deleted', { timeout: 10000 }).should('exist');
  });
});