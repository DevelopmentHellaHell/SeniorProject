/// <reference types="cypress" />
/* cy.get(), cy.contains()
** https://filiphric.com/cypress-basics-selecting-elements
** https://on.cypress.io/type
*/

import { Ajax } from "../../src/Ajax";
import { Database } from "./TestModels/Database";

let testsRoute: string = '/tests/deleteDatabaseRecords';
let discover: string = 'http://localhost:3000/discover';

describe('attempt-to-access-page', () => {
    after(async () => {
        await Ajax.post(testsRoute, { database: Database.Databases.USERS });
    });

    it('logged-out', () => {
        cy.visit(discover);
        cy.get('#discover-container').contains('Search');
    });

    it('logged-in', () => {
        Cypress.session.clearAllSavedSessions();
        cy.RegisterViaApi(Cypress.env('standardEmail'), Cypress.env('standardPassword'));
        cy.LoginViaApi(Cypress.env('standardEmail'), Cypress.env('standardPassword'));

        cy.visit(discover);
        cy.get('#discover-container').contains('Search');
    });
});

describe('curated-results', () => {
    let realEmail: string = Cypress.env("realEmail");
    let standardPassword: string = Cypress.env("standardPassword");
    let testsRoute: string = '/tests/deleteDatabaseRecords';

    after(async () => {
        await Ajax.post(testsRoute, { database: Database.Databases.USERS });
    });

    before(() => {
        Cypress.session.clearAllSavedSessions();
        //using Custom Commands
        cy.RegisterViaApi(Cypress.env('realEmail'), Cypress.env('standardPassword'));
    });

    it('visit-page-and-load-curated-data', () => {
        cy.visit(discover);
        cy.get("#curated-view-wrapper").contains("Listings");
        cy.get("#curated-view-wrapper").contains("Project Showcases");
        cy.get("#curated-view-wrapper").contains("Collaborators");
    });
})

describe('data-results', () => {
    let realEmail: string = Cypress.env("realEmail");
    let standardPassword: string = Cypress.env("standardPassword");
    let testsRoute: string = '/tests/deleteDatabaseRecords';

    after(async () => {
        await Ajax.post(testsRoute, { database: Database.Databases.USERS });
    });

    before(() => {
        Cypress.session.clearAllSavedSessions();
        //using Custom Commands
        cy.RegisterViaApi(Cypress.env('realEmail'), Cypress.env('standardPassword'));
    });

    it('visit-page-and-load-curated-data', () => {
        cy.visit(discover);
        cy.get("#curated-view-wrapper").contains("Listings");
        cy.get("#curated-view-wrapper").contains("Project Showcases");
        cy.get("#curated-view-wrapper").contains("Collaborators");
    });

    it('visit-page-and-search-data', () => {
        cy.visit(discover);
        cy.get("#discover-content").get("#search-input").type("woodworking");
        cy.get("#discover-content").get("#search-button").click();
        cy.get("#discover-content").contains("Results:");
    });

    it('visit-page-and-search-with-catgeory-listings', () => {
        cy.visit(discover);
        cy.get("#discover-content").get("#search-input").type("woodworking");
        cy.get('.dropdown-content').invoke('show');
        cy.get("#discover-content").get("#category").get("#category-listings").click();
        cy.get("#discover-content").get("#search-button").click();
        cy.get("#discover-content").contains("Results:");
    });

    it('visit-page-and-search-with-catgeory-project-showcases', () => {
        cy.visit(discover);
        cy.get("#discover-content").get("#search-input").type("woodworking");
        cy.get('.dropdown-content').invoke('show');
        cy.get("#discover-content").get("#category").get("#category-project-showcases").click();
        cy.get("#discover-content").get("#search-button").click();
        cy.get("#discover-content").contains("Results:");
    });

    // it('visit-page-and-search-with-catgeory-collaborators', () => {
    //     cy.visit(discover);
    //     cy.get("#discover-content").get("#search-input").type("woodworking");
    //     cy.get('#category').get('.dropdown-content').invoke('show');
    //     cy.get("#discover-content").get("#category").get("#category-collaborators").click();
    //     cy.get("#discover-content").get("#search-button").click();
    //     cy.get("#discover-content").contains("Results:");
    // });

})