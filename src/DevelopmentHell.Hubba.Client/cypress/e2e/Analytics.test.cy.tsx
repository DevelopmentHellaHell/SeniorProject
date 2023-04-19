/// <reference types="cypress" />
/* cy.get(), cy.contains()
** https://filiphric.com/cypress-basics-selecting-elements
** https://on.cypress.io/type
*/

import { Ajax } from "../../src/Ajax";
import { Database } from "./TestModels/Database";

let testsRoute: string = '/tests/deleteDatabaseRecords';
let admindashboard: string = 'http://localhost:3000/admin-dashboard';

describe('attempt-to-access-page', () => {
    after(async () => {
        await Ajax.post(testsRoute, { database: Database.Databases.USERS });
    });

    it('logged-out', () => {
        cy.visit(admindashboard);
        cy.get('#login-card').contains('Login');
    });

    it('logged-in', () => {
        Cypress.session.clearAllSavedSessions();
        cy.RegisterViaApi(Cypress.env('standardEmail'), Cypress.env('standardPassword'));
        cy.LoginViaApi(Cypress.env('standardEmail'), Cypress.env('standardPassword'));

        cy.visit(admindashboard);
        cy.get('#App').contains('403 Unauthorized');
    });
});