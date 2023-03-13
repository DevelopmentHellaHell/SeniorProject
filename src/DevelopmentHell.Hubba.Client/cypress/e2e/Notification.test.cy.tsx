/// <reference types="cypress" />
import { start } from "repl";
import { Ajax } from "../../src/Ajax";
import { Database } from "./TestModels/Database";

/**
 * Test Notification functionality
 * User register, login, change Notification Settings, Notification Menu
 * Delete test data from database after testing
 */

describe.only('Navigate to Notification Settings', () => {

    let testsRoute: string = '/tests/deleteDatabaseRecords';
    beforeEach(() => {
        // using Custom Commands
        cy.RegisterViaApi(Cypress.env('realEmail'), Cypress.env('standardPassword'));
        cy.LoginViaApi(Cypress.env('realEmail'), Cypress.env('standardPassword'));
        cy.visit('/account');
    });

    after(async () => {
        await Ajax.post(testsRoute, { database: Database.Databases.USERS });
        await Ajax.post(testsRoute, { database: Database.Databases.NOTIFICATIONS });
    })

    it('user clicks Email Notification Settings on', () => {
        
        cy.contains("Notification Settings").click();
        cy.contains("Delivery Method").should("exist").and("be.visible");
        
        cy.get('#siteNotifications-toggle-buttons').contains("Off").click();
        cy.get('#siteNotifications-toggle-buttons').contains("Off").should('have.css', 'color', `rgb(241, 252, 250)`);
        cy.get('#siteNotifications-toggle-buttons').contains("On").click();
        cy.get('#siteNotifications-toggle-buttons').contains("On").should('have.css', 'color', `rgb(241, 252, 250)`);
        // cy.contains("Save").click();
        // // Save has cooldown of 5 seconds
        // cy.wait(5000);
        
        cy.get('#emailNotifications-toggle-buttons').contains("On").click();
        cy.get('#emailNotifications-toggle-buttons').contains("On").should('have.css', 'color', `rgb(241, 252, 250)`);
        cy.get('#emailNotifications-toggle-buttons').contains("Off").click();
        cy.get('#emailNotifications-toggle-buttons').contains("Off").should('have.css', 'color', `rgb(241, 252, 250)`);
        // cy.contains("Save").click();
        // cy.wait(5000);

        cy.get('#textNotifications-toggle-buttons').contains("On").click();
        cy.get('#textNotifications-toggle-buttons').contains("On").should('have.css', 'color', `rgb(241, 252, 250)`);
        cy.get('#textNotifications-toggle-buttons').contains("Off").click();
        cy.get('#textNotifications-toggle-buttons').contains("Off").should('have.css', 'color', `rgb(241, 252, 250)`);
        // cy.contains("Save").click();
        // cy.wait(5000);

        cy.get('#typeScheduling-toggle-buttons').contains("Off").click();
        cy.get('#typeScheduling-toggle-buttons').contains("Off").should('have.css', 'color', `rgb(241, 252, 250)`);
        cy.get('#typeScheduling-toggle-buttons').contains("On").click();
        cy.get('#typeScheduling-toggle-buttons').contains("On").should('have.css', 'color', `rgb(241, 252, 250)`);
        // cy.contains("Save").click();
        // cy.wait(5000);

        cy.get('#typeWorkspace-toggle-buttons').contains("Off").click();
        cy.get('#typeWorkspace-toggle-buttons').contains("Off").should('have.css', 'color', `rgb(241, 252, 250)`);
        cy.get('#typeWorkspace-toggle-buttons').contains("On").click();
        cy.get('#typeWorkspace-toggle-buttons').contains("On").should('have.css', 'color', `rgb(241, 252, 250)`);
        // cy.contains("Save").click();

        cy.get('#typeProjectShowcase-toggle-buttons').contains("Off").click();
        cy.get('#typeProjectShowcase-toggle-buttons').contains("Off").should('have.css', 'color', `rgb(241, 252, 250)`);
        cy.get('#typeProjectShowcase-toggle-buttons').contains("On").click();
        cy.get('#typeProjectShowcase-toggle-buttons').contains("On").should('have.css', 'color', `rgb(241, 252, 250)`);
        // cy.contains("Save").click();
        // cy.wait(5000);

        cy.get('#typeOther-toggle-buttons').contains("Off").click();
        cy.get('#typeOther-toggle-buttons').contains("Off").should('have.css', 'color', `rgb(241, 252, 250)`);
        cy.get('#typeOther-toggle-buttons').contains("On").click();
        cy.get('#typeOther-toggle-buttons').contains("On").should('have.css', 'color', `rgb(241, 252, 250)`);
        // cy.contains("Save").click();
        // cy.wait(5000);

        cy.get('#phone-number-input').clear().type(Cypress.env("realNumber")).should("have.value", "5107755205");
        cy.get(".cellphone-details-fields .dropdown-content").invoke("show");
        cy.contains("Verizon").click();
        cy.contains("Verizon").should("exist").and("be.visible");
        // cy.contains("Save").click();
        
         // force show the dropdown menu
         cy.get('.dropdown-content').invoke('show');
 
         // timer
         const startTimer = Date.now();
         cy.contains('Notification').click();
         const endTimer = Date.now() - startTimer;
         expect(endTimer).lessThan(3000);
         cy.url().should('eq', Cypress.env('baseUrl')+'/notification')

         // click to demonstrate filters
         cy.contains("Scheduling").click();
         cy.contains("Workspace").click();
         cy.contains("Project Showcase").click();
         cy.contains("Other").click();

         // deactivating filters
         cy.contains("Scheduling").click();
         cy.contains("Workspace").click();
         cy.contains("Project Showcase").click();
         cy.contains("Other").click();

         // demonstrate selecting notification and deleting it
         cy.get(".table-button-hide" ).first().click();
         cy.get(".table-button-hide" ).first().should('have.css', 'color', `rgb(77, 100, 106)`);
         cy.contains("Delete").click();

        // demonstrate clear all
         cy.contains("Clear All").click();
         cy.contains("You have no new notifications.").should("exists");
    });
})

