/// <reference types="cypress" />
/* cy.get(), cy.contains()
** https://filiphric.com/cypress-basics-selecting-elements
** https://on.cypress.io/type
*/

import Button from "../../src/components/Button/Button";
import { Ajax } from "../../src/Ajax";
import { Database } from "./TestModels/Database";


let homeUrl: string = 'http://localhost:3000/'
let registrationUrl: string = 'http://localhost:3000/registration';
let loginUrl: string = 'http://localhost:3000/login';
let accountUrl: string = 'http://localhost:3000/account';
let testsRoute: string = '/tests/deleteDatabaseRecords';




//describe.only to run a single test case
describe('working-ui', () => {
    
    before(() =>{
        //using Custom Commands
        cy.RegisterViaApi(Cypress.env('standardEmail'), Cypress.env('standardPassword'));
    })

    beforeEach(() => {
        // logging into account via session
        cy.LoginViaUI(Cypress.env('standardEmail'), Cypress.env('standardPassword'));
        cy.visit(accountUrl);
        cy.contains('Login & Security').click();
    });

    after(async () => {
        await Ajax.post(testsRoute, { database: Database.Databases.USERS });
    });
    
    it('working-delete-account-button', () => {
        cy.get('#delete-button').click();
        cy.get('#account-deletion-header')
        .contains('Account Deletion');
    });

    it('working-cancel-account-button', () => {
        cy.get('#delete-button').click();
        cy.get('#account-deletion-header')
        .contains('Account Deletion');
        cy.get('#buttons').contains("Cancel").click();
        cy.get('#login-security-header')
        .contains('Login & Security');
    });
    
});

describe('working-account-deletion', () => {
    
    before(() =>{
        //using Custom Commands
        cy.RegisterViaApi(Cypress.env('standardEmail'), Cypress.env('standardPassword'));
    })

    beforeEach(() => {
        // logging into account via session
        cy.LoginViaUI(Cypress.env('standardEmail'), Cypress.env('standardPassword'));
        cy.visit(accountUrl);
        cy.contains('Login & Security').click();
    });

    after(async () => {
        await Ajax.post(testsRoute, { database: Database.Databases.USERS });
    });

    it('working-delete-account-confirmation-button', () => {
        cy.get('#delete-button').click();
        cy.get('#account-deletion-header')
        .contains('Account Deletion');
        
        // deleting account
        cy.get('#buttons').contains("Delete").click();
        cy.url().should('eq', homeUrl);
        
        // trying to visit account url
        cy.visit(accountUrl)
        cy.url().should('eq', loginUrl);

        // trying to log in with previous credentials
        cy.get('#email').type(Cypress.env('standardEmail'));
        cy.get('#password').type(Cypress.env('standardPassword'));
        cy.contains('Submit').click()
            .then(() => {
                cy.get('.login-card .error').should('exist').and('be.visible');
            });
    });
});