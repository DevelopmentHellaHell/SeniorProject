/// <reference types="cypress" />
import { start } from "repl";
import { Ajax } from "../../src/Ajax";
import { Database } from "./TestModels/Database";

/**
 * Test logout functionality
 * User register, login, logout
 * Logout operation completes under 5s
 * Delete test data from database after testing
 */
describe('logout successfully by User NavBar', () => {
    let testsRoute: string = '/tests/deleteDatabaseRecords';
    beforeEach(() => {
        //using Custom Commands
        cy.registerViaApi(Cypress.env('realEmail'), Cypress.env('standardPassword'));
        cy.loginViaUI(Cypress.env('realEmail'), Cypress.env('standardPassword'));
        cy.visit('/');
    });
    
    after(async () => {
        await Ajax.post(testsRoute, { database: Database.Databases.USERS });
    })

    it('user clicks logout from dropdown menu, logout under 5s', () => {
        cy.pause();
        //force show the dropdown menu
        cy.get('.dropdown-content').invoke('show');

        //timer
        const startTimer = Date.now();
        cy.contains('Logout').click();
        const endTimer = Date.now() - startTimer;
        expect(endTimer).lessThan(5000);
        cy.get('.nav-user').should('not.exist');
        cy.get('#account').should('not.exist');
        cy.url().should('eq', Cypress.env('baseUrl'))
    });
});