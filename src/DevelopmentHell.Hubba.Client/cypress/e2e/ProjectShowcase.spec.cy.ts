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
let adminPassword: string = 'password1234';
let testadminRoute: string = '/tests/createAdmin';
let testuserRoute: string = '/tests/createUser';
let testuserBody: { email: string, password: string } = { email: userEmail, password: adminPassword }
let testadminBody: { email: string, password: string } = { email: adminEmail, password: adminPassword }

let accountRoute: string = 'http://localhost:3000/account';
let showcaseRoute: string = 'http://localhost:3000/showcases/view?s=';
let createShowcaseRoute = '/showcases/new';
let getShowcaseRoute = '/showcases/view';
let getUserShowcasesRoute = '/showcases/user';

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

describe ('Showcase Tests', () => {
  before(async () => {
      await Ajax.post(testsRoute, { database: Database.Databases.USERS }).then((result) => {
        expect(result.status == 200 || result.status == 204).to.be.true;
      });
      await Ajax.post(testuserRoute, testuserBody).then((result) => {
        expect(result.status == 200 || result.status == 204).to.be.true;
      });
      await Ajax.post(testadminRoute, testadminBody).then((result) => {
        expect(result.status == 200 || result.status == 204).to.be.true;
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
    cy.get('.input-title[type=text]').type('Test Showcase 1');
    cy.get('input[type=file]').selectFile([testFile1]);
    cy.wait(3000);
    cy.get('button[type=submit]').click();
    cy.intercept("https://localhost:7137/showcases/new").as("createShowcase");
    cy.wait("@createShowcase", {timeout: 10000});
    cy.get('#App').contains('Test Showcase 1', { timeout: 30000 }).should('exist');
    cy.url().then((url) => {
      showcaseId = url.slice(url.search('=')+1);
    });
  });

  it('Seq 2: Publish Showcase', () => {
    cy.LoginViaApi(userEmail, adminPassword);
    cy.visit(accountRoute);
    cy.get('.links').contains('Project Showcases').click();
    cy.get("tr[key=showcase-"+showcaseId+']').contains(".published-no").click();
  });

  it('Seq 2.1: View Showcase as unauthenticated user', () => {
    cy.visit(showcaseRoute+showcaseId);
    cy.get('#App').contains('Test Showcase 1', { timeout: 10000 }).should('exist');
  });

  it('Seq 2.2: Comment on Showcase as authenticated user', () => {
    cy.visit(showcaseRoute+showcaseId);
    cy.get('.input-comment').type('This is a test comment');
    
  });

  it('Seq 3: Edit Showcase', () => {
    cy.LoginViaApi(userEmail, adminPassword);
    cy.visit(accountRoute);
    cy.get('.links').contains('Project Showcases').click();
    cy.get("tr[key=showcase-"+showcaseId+']').contains('Edit').click();
    cy.get('.input-title[type=text]').type('Edited Showcase 1');
    cy.get('button[type=submit]').click();
    cy.get('#App').contains('Edited Showcase 1', { timeout: 10000 }).should('exist');
  });

  it('Seq 4: Delete Showcase', () => {
    cy.LoginViaApi(userEmail, adminPassword);
    cy.visit(accountRoute);
    cy.get('.links').contains('Project Showcases').click();
    cy.get("tr[key=showcase-"+showcaseId+']').contains('Delete').click();
    cy.get('#App').contains('Showcase Deleted', { timeout: 10000 }).should('exist');
  });
});